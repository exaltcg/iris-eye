using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IrisEye.Core.Entities;
using IrisEye.Core.Models;

namespace IrisEye.Data.Parsers
{
    public class GitParser
    {
        private readonly WebClient _webClient;
        private const string BaseUrl = "https://github.com/mozilla/iris/tree/dev/iris/tests";
        
        public GitParser()
        {
            _webClient = new WebClient();
        }

        public async Task<List<Suit>> ParseSuitsAsync()
        {
            var ret = new List<Suit>();
            var response = await _webClient.DownloadStringTaskAsync(BaseUrl);
            var rootDocument = new HtmlDocument();
            rootDocument.LoadHtml(response);

            var folders = rootDocument.DocumentNode.SelectNodes("//td[@class='content']/span/a[not(contains(.,'__'))]");
            foreach (var folder in folders)
            {
                var newSuite = new Suit
                {
                    Name = folder.Attributes["title"].Value,
                    Tests = new List<string>()
                };
                var subFolderContent = await _webClient.DownloadStringTaskAsync(BaseUrl + "/" + newSuite.Name);
                var subDocument = new HtmlDocument();
                subDocument.LoadHtml(subFolderContent);
                var testsNodes = subDocument.DocumentNode.SelectNodes("//td[@class='content']/span/a[contains(.,'.py')]");
                foreach (var test in testsNodes)
                {
                    newSuite.Tests.Add(test.InnerText);
                }

                ret.Add(newSuite);
            }
            
            
            return ret;
        }

        public async Task<List<TestInfo>> ParseTestInfoAsync()
        {
            var ret = new List<TestInfo>();
            var response = await _webClient.DownloadStringTaskAsync(BaseUrl);
            var rootDocument = new HtmlDocument();
            rootDocument.LoadHtml(response);

            var folders = rootDocument.DocumentNode.SelectNodes("//td[@class='content']/span/a[not(contains(.,'__'))]");
            foreach (var folder in folders)
            {
                var folderName = folder.Attributes["title"].Value;
                var subFolderContent = await _webClient.DownloadStringTaskAsync(BaseUrl + "/" + folderName);
                var subDocument = new HtmlDocument();
                subDocument.LoadHtml(subFolderContent);
                var testsNodes = subDocument.DocumentNode.SelectNodes("//td[@class='content']/span/a[contains(.,'.py')]");
                foreach (var test in testsNodes.Where(p=>!p.InnerText.Contains("__")))
                {
                    var newTestInfo = new TestInfo {TestName = test.InnerText, SuiteName = folderName};
                    var testContent = await _webClient.DownloadStringTaskAsync(BaseUrl.Replace("tree","commits") + "/" + folderName + "/" + test.InnerText);
                    var testContentDocument = new HtmlDocument();
                    testContentDocument.LoadHtml(testContent);
                    var author =
                        testContentDocument.DocumentNode.SelectSingleNode(
                            "//a[@class='commit-author tooltipped tooltipped-s user-mention']");

                    if (author != null)
                        newTestInfo.AuthorLogin = author.InnerText;
                    
                    var raw = (BaseUrl + "/" + folderName + "/" + test.InnerText)
                        .Replace("tree/", "")
                        .Replace("github", "raw.githubusercontent");
                    
                    ret.Add(newTestInfo);

                }

            }

            return ret;

        }
    }
}