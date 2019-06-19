using System.Collections.Generic;

namespace IrisEye.Core.Models
{
    public class CustomStepsSeparated
    {
        public string content { get; set; }
        public string expected { get; set; }
    }

    public class TestRailTest
    {
        public int id { get; set; }
        public string title { get; set; }
        public int section_id { get; set; }
        public int template_id { get; set; }
        public int type_id { get; set; }
        public int priority_id { get; set; }
        public object milestone_id { get; set; }
        public object refs { get; set; }
        public int created_by { get; set; }
        public int created_on { get; set; }
        public int updated_by { get; set; }
        public int updated_on { get; set; }
        public object estimate { get; set; }
        public object estimate_forecast { get; set; }
        public int suite_id { get; set; }
        public object custom_test_case_owner { get; set; }
        public int custom_automation_status { get; set; }
        public object custom_test_objective { get; set; }
        public string custom_preconds { get; set; }
        public object custom_steps { get; set; }
        public object custom_expected { get; set; }
        public List<CustomStepsSeparated> custom_steps_separated { get; set; }
        public object custom_mission { get; set; }
        public object custom_goals { get; set; }
    }   
}