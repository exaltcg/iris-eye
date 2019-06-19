using System;
using System.Collections.Generic;

namespace IrisEye.Core.Entities
{
    public class TestSuite
    {
        public long Id { get; protected set; }
        public int TestRailId { get; set; }
        public int GitHubProjectId { get; set; }
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime AddedOn { get; set; }
        
        public IList<TestCase> TestCases { get; set; }
    }
}