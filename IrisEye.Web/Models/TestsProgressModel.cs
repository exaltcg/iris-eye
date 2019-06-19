using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace IrisEye.Web.Models
{    
    public class TestsProgressModel
    {
        public string label { get; set; }
        public List<List<object>> data { get; set; }
    }

}