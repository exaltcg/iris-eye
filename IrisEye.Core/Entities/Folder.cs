using System;
using System.Collections.Generic;

namespace IrisEye.Core.Entities
{
    public class Folder
    {
        public long Id { get; protected set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        
        public IList<Run> Runs { get; set; }
    }
}