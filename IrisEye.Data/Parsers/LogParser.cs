using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;
using IrisEye.Data.Extensions;

namespace IrisEye.Data.Parsers
{
    public class LogParser
    {
        private readonly List<TestInfo> _testInfos;
        private string _textContent;
        private string _fileName;


        public LogParser(List<TestInfo> testInfos)
        {
            _testInfos = testInfos;
        }
        private string GetHash()
        {
            var sb = new StringBuilder();
            using (var hash = SHA256.Create())            
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(_textContent));
                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        
        public void LoadFromFile(string filePath, string fileName)
        {
            _textContent = File.ReadAllText(filePath);
            _fileName = fileName;
        }
        
        public Run Parse()
        {
            _textContent = _textContent.Replace("\r", "");
            var run = new Run();
            const string separator = "[iris.test_runner] Executing:";
            const string stacktraceSeparator =
                ">> Traceback (most recent call last):";
            run.ReportHash = GetHash();
            var blocks = _textContent.Split(separator);

            run.Build = blocks.First().GetBetween("Selected build: ");
            run.BetaChannel = blocks.First().GetBetween("beta channel is: "); 
            run.Environment = _textContent.GetBetween("Platform: ", ",");
            if (run.Environment == "win" && _fileName.Contains("7"))
            {
                run.Environment = "win7";
            }
            run.Errors = _textContent.GetBetween("Errors: ", " ").ToInt();
            run.Failed = _textContent.GetBetween("Failed: ", ",").ToInt();
            run.Passed = _textContent.GetBetween("Passed: ", ",").ToInt();
            run.Blocked = _textContent.GetBetween("Blocked: ", ")").ToInt();
            run.Skipped = _textContent.GetBetween("Skipped: ", " ").ToInt();
            run.Total = _textContent.GetBetween("Total: ", " ").ToInt();
            if (run.Total==0) throw new Exception("Report contains no results.");
            run.Version = _textContent.GetBetween("Version: ", ",");
            run.AddedOn = DateTime.Now;
            DateTime.TryParse(_textContent.Split('[')[0].Trim().Replace(",","."), out var reportTime);
            run.ReportTime = reportTime;
            run.FailedTests = _textContent.GetBetween("did not pass:", "---").Replace("\r","").Split("\n");
            run.FailedTests = run.FailedTests.ToList().Where(p => !string.IsNullOrEmpty(p)).ToArray();
           
            run.Tests = new List<Test>();
           
            foreach (var block in blocks.Skip(1))
            {
                var newTest = new Test
                {
                    Name = block.GetBetween("- [", "]"), 
                    Description = block.GetBetween("]: "),
                    Steps = new List<Step>()
                };
                //find out test suits name:
                var suiteName = _testInfos.FirstOrDefault(p => p.TestName == newTest.Name+".py");
                newTest.SuiteName = suiteName?.SuiteName;

                foreach (var step in block.Split("\n").Where(p=>p.Contains(">>>")).Where(p=>!p.Contains("[INFO]"))) 
                {
                    var newStep = new Step();
                    var dTime = step.Split(" [")[0].Replace(',','.').Trim();
                    if (!DateTime.TryParse(dTime, out var issuedDt))
                    {
                        Console.WriteLine("SNAP!");
                    }

                    newStep.Issued = issuedDt;
                    if (step.Contains("<<< Step"))
                    {
                    newStep.Message = step.GetBetween(": ");
                    }
                    else
                    {
                        newStep.Message = step.GetBetween("ERROR <<< ");
                    }
                    newStep.IsPassed = step.GetBetween(">>> ", " <") == "PASSED";
                   
                    if (!newStep.IsPassed && block.Contains(stacktraceSeparator))
                    {
                        var stacktraceBlock = block.Split(stacktraceSeparator)[1];
                        stacktraceBlock = stacktraceBlock.Substring(0, stacktraceBlock.IndexOf("---", StringComparison.Ordinal));
                        stacktraceBlock = string.Join('\n', stacktraceBlock.Split('\n').Skip(1));
                        newStep.Stacktrace = stacktraceBlock;
                        newTest.Steps.Add(newStep);
                        break;
                    }
                    newTest.Steps.Add(newStep);
                    
                   
                }

                if (block.Contains(">>> ERROR <<<"))
                {
                    newTest.Status = TestStatus.Error;
                } else if (block.Contains(">>> FAILED <<<"))
                {
                    newTest.Status = TestStatus.Failed;
                }
                else
                {
                    newTest.Status = TestStatus.Passed;
                }
                run.Tests.Add(newTest);
            }
            return run;
        }

        public void LoadFromText(string textContent, string fileName)
        {
            _textContent = textContent;
            _fileName = fileName;
        }
    }
}