using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Employee
{
    public long Id { get; set; }

    public string AspNetUserId { get; set; }

    public string EmployeeCode { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int? Gender { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public long DepartmentId { get; set; }

    public long? DesignationId { get; set; }

    public long ReportingManagerId { get; set; }

    public DateOnly JoiningDate { get; set; }

    public int EmploymentType { get; set; }

    public string Qualification { get; set; }

    public decimal? ExperienceInYears { get; set; }

    public string LicenseNumber { get; set; }

    public string BloodGroup { get; set; }

    public string EmergencyContactName { get; set; }

    public string EmergencyContactNumber { get; set; }

    public string CurrentAddress { get; set; }

    public string PermanentAddress { get; set; }

    public string UserImages { get; set; }

    public bool? LoginFirst { get; set; }

    public long ShiftId { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual AspNetUser AspNetUser { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Department Department { get; set; }

    public virtual Designation Designation { get; set; }

    public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

    public virtual Shift Shift { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<TrainingAssignment> TrainingAssignments { get; set; } = new List<TrainingAssignment>();

    public virtual ICollection<TrainingCertificate> TrainingCertificates { get; set; } = new List<TrainingCertificate>();

    public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();

    public virtual ICollection<TransferRequest> TransferRequests { get; set; } = new List<TransferRequest>();
}
