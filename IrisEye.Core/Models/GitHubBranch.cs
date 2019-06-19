using System;
using System.Collections.Generic;

namespace IrisEye.Core.Models
{

public class RequiredStatusChecks
{
    public string enforcement_level { get; set; }
    public List<object> contexts { get; set; }
}

public class Protection
{
    public bool enabled { get; set; }
    public RequiredStatusChecks required_status_checks { get; set; }
}

public class GitHubBranch
{
    public string name { get; set; }
    public Commit commit { get; set; }
    public Links _links { get; set; }
    public bool @protected { get; set; }
    public Protection protection { get; set; }
    public string protection_url { get; set; }
}
}