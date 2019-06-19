using System;

namespace IrisEye.Core.Entities
{
    public class TestCaseComment
    {
        public long Id { get; protected set; }
        public string Message { get; set; }
        public TestCase TestCase { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public SystemUser AddedBy { get; set; }
        public SystemUser AddressedTo { get; set; }
        
        public bool IsRead { get; set; }
    }
}