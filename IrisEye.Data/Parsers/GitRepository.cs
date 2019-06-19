using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IrisEye.Core.Entities;
using LibGit2Sharp;
using Newtonsoft.Json;

namespace IrisEye.Data.Parsers
{
    public class GitRepository
    {
        private readonly Repository _repository;
        private readonly string _localPath;

        public GitRepository()
        {
            _localPath = Path.GetTempPath() + "IrisRepository";
            if (Directory.Exists(_localPath))
            {
                Directory.Delete(_localPath, true);
            }
            Directory.CreateDirectory(_localPath);
            Repository.Clone("https://github.com/mozilla/iris.git", _localPath);
            _repository = new Repository(_localPath);
            var dev = _repository.Branches["origin/dev"];
            var branch = _repository.CreateBranch("dev", dev.Tip);
            Commands.Checkout(_repository, branch);
        }
        
        public IEnumerable<TestInfo> GetTestInfos(IEnumerable<string> existingItems)
        {
            var ret = new List<TestInfo>();
            var directories = Directory.GetDirectories(_localPath + "/iris/tests");
            var enumerable = existingItems.ToList();
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.py");
                foreach (var file in files.Where(p => !p.Contains("__init__.py")))
                {
                    var fileName = file.Split('/').Last();
                    if (enumerable.Exists(p => p == fileName)) continue;
                    var testInfo = new TestInfo
                    {
                        SuiteName = directory.Split('/').Last(),
                        TestName = fileName
                    };
                    var result = _repository.Commits.QueryBy(file.Replace(_localPath + "/", ""),
                        new CommitFilter {SortBy = CommitSortStrategies.Topological}).ToList();
                    var firstCommit = result.LastOrDefault();
                    if (firstCommit != null)
                    {
                        testInfo.AuthorLogin = firstCommit.Commit.Author.Name;
                    }
                    else
                    {
                        testInfo.AuthorLogin = "";
                    }

                    ret.Add(testInfo);
                }
            }

            var json = JsonConvert.SerializeObject(ret);
            var temp = Path.GetTempFileName();
            File.WriteAllText(temp, json);
            return ret;
        }
    }
}