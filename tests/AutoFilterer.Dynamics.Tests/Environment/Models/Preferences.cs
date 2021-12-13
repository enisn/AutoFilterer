using System;

namespace AutoFilterer.Dynamics.Tests.Environment.Models;

public class Preferences
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string GivenName { get; set; }
    public int SecurityLevel { get; set; }
}
