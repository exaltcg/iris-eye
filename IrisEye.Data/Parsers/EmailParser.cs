using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IrisEye.Core.Entities;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace IrisEye.Data.Parsers
{
    public class EmailParser:IDisposable
    {
        private readonly ImapClient _client;
        private readonly LogParser _log;
        private readonly List<TestInfo> _testInfos;

        public EmailParser(List<TestInfo> testInfos)
        {
            _testInfos = testInfos;
            _client = new ImapClient();
            _client.AuthenticationMechanisms.Remove ("XOAUTH2");
            _client.ServerCertificateValidationCallback = (s,c,h,e) => true;
            _client.Connect ("imap.gmail.com", 993, true, CancellationToken.None);
            _client.Authenticate("", ""); //Login and password for email agent
            _log = new LogParser(_testInfos);
        }
        
        private static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public List<Run> GetReportAsync()
        {
            var ret = new List<Run>();
            var inbox = _client.Inbox;
            inbox.Open (FolderAccess.ReadWrite);
            var query = SearchQuery
                .SubjectContains("Iris Test Report")
                .And(SearchQuery.NotSeen);
            foreach (var uid in inbox.Search(query))
            {
                var message = inbox.GetMessage(uid);
                if (!message.Attachments.Any()) continue;
                if (!(message.Attachments.First() is MimePart mimePart)) continue;
                using (var stream = new MemoryStream())
                {
                    mimePart.Content.DecodeTo(stream);
                    var fileContent = StreamToString(stream);
                    _log.LoadFromText(fileContent, message.Subject.Contains("Win7")? "7" : "");
                    ret.Add(_log.Parse());
                }
                
                inbox.AddFlags(uid, MessageFlags.Seen, true);
            }

            return ret;
        } 

        public void Dispose()
        {
            _client.Disconnect(true);
            _client.Dispose();
        }
    }
}