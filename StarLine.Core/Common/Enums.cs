
using System.ComponentModel.DataAnnotations;

public enum Genders
{
    [Display(Name = "Male")]
    Male = 1,
    [Display(Name = "Female")]
    Female
}

public enum HolidayTypes
{
    National = 1,
    Festival,
    Company,
    Plant,
    Other
}

public enum LeaveStatus
{
    [Display(Name = "Pending")]
    Pending = 1,
    [Display(Name = "Approve")]
    Approved,
    [Display(Name = "Reject")]
    Rejected
}

public enum EmploymentType
{
    [Display(Name = "Full Time")]
    FullTime = 1,
    [Display(Name = "Part Time")]
    PartTime,
    [Display(Name = "Contract")]
    Contract,
    [Display(Name = "Temporary")]
    Temporary,
    [Display(Name = "Intern")]
    Intern,
    [Display(Name = "Consultant")]
    Consultant,
    [Display(Name = "Remote")]
    Remote
}

public enum BloodGroup
{
    [Display(Name = "A + tve")]
    APositive = 1,
    [Display(Name = "A - tve")]
    ANegative,
    [Display(Name = "B + tve")]
    BPositive,
    [Display(Name = "B - tve")]
    BNegative,
    [Display(Name = "AB + tve")]
    ABPositive,
    [Display(Name = "AB - tve")]
    ABNegative,
    [Display(Name = "O + tve")]
    OPositive,
    [Display(Name = "O - tve")]
    ONegative
}

public enum TeamType
{
    [Display(Name = "Department-Based")]
    Department = 1,
    [Display(Name = "Project-Based")]
    Project,
    [Display(Name = "Compliance & Audit")]
    Compliance,
    [Display(Name = "Plant/Shift-Based")]
    Plant,
    [Display(Name = "Cross-Functional")]
    Functional,
}

public enum TeamMemberRole
{
    [Display(Name = "Team Leader")]
    TeamLead = 1,
    [Display(Name = "Team Member")]
    Member
}
