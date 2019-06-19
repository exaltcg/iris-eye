using System;

namespace IrisEye.Core.Entities
{
    public class TestCaseHistory
    {
        public long Id { get; protected set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public SystemUser Author { get; set; }
        public string Message { get; set; }
    }
}