using StarLine.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StarLine.Core.Models
{
    public class EmployeeModel : BaseEntity
    {
        public string AspNetUserId { get; set; } = null!;
        [Display(Name = "Employee Code")]
        [Required(ErrorMessage = "Employee code required")]
        public string EmployeeCode { get; set; } = null!;
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name required")]
        public string FirstName { get; set; } = null!;
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; } = null!;
        [Display(Name = "Gender")]
        public int? Gender { get; set; }
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth required")]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address required")]
        public string Email { get; set; } = null!;
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select department")]
        public long DepartmentId { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please select designation")]
        public long DesignationId { get; set; }
        [Display(Name = "Reporting Manager")]
        [Required(ErrorMessage = "Please select manager")]
        public long ReportingManagerId { get; set; }
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Joining date required")]
        public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Display(Name = "Employment Type")]
        [Required(ErrorMessage = "Please select employeement type")]
        public int EmploymentType { get; set; }
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }
        [Display(Name = "Experience (in year)")]
        public decimal ExperienceInYears { get; set; }
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [Display(Name = "Emergency Contact Person Name")]
        public string EmergencyContactName { get; set; }
        [Display(Name = "Emergency Contact Person Number")]
        public string EmergencyContactNumber { get; set; }
        [Display(Name = "Current Address")]
        public string CurrentAddress { get; set; }
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Please select role")]
        public string RoleId { get; set; } = null!;
        public string UserImages { get; set; }
        public bool LoginFirst { get; set; } = false;
        public string RoleName { get; set; } = null!;
    }

    public class EmployeeDetailsModel : BaseEntity
    {

        public string EmployeeCode { get; set; } = null!;
        [JsonIgnore]
        public string FirstName { get; set; } = null!;
        [JsonIgnore]
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        [JsonIgnore]
        public int? Gender { get; set; }
        public string GenderName => CommonFunctions.GetDisplayName<Genders>((int)Gender);
        public string DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public long ReportingManagerId { get; set; }
        public string JoiningDate { get; set; }
        [JsonIgnore]
        public int EmploymentType { get; set; }
        public string EmploymentTypeName => CommonFunctions.GetDisplayName<EmploymentType>((int)EmploymentType);
        public string Qualification { get; set; }
        public decimal ExperienceInYears { get; set; }
        public string LicenseNumber { get; set; }
        public int BloodGroup { get; set; }
        public string BloodGroupName => CommonFunctions.GetDisplayName<BloodGroup>((int)BloodGroup);
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string UserImages { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
