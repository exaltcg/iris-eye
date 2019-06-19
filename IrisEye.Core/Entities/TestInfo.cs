using System;

namespace IrisEye.Core.Entities
{
    public class TestInfo
    {
        public long Id { get; protected set; }
        public string TestName { get; set; }
        public string SuiteName { get; set; }
        public string AuthorLogin { get; set; }
        public string AuthorGitHubId { get; set; }
    }
}