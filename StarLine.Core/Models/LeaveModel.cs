using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StarLine.Core.Models
{
    public class LeaveModel : BaseEntity
    {
        [Display(Name = "Employee Name")]
        public long EmployeeId { get; set; }
        [Display(Name = "Leave Type")]
        [Required(ErrorMessage = "Select Leave Type")]
        public long LeaveTypeId { get; set; }
        [Display(Name = "Leave Duration")]
        [Required(ErrorMessage = "Select Leave Duration")]
        public LeaveDuration LeaveDuration { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "Select From Date")]
        public DateOnly FromDate { get; set; }
        [Display(Name = "To date")]
        [Required(ErrorMessage = "Select To Date")]
        public DateOnly ToDate { get; set; }
        [Display(Name = "Total Days")]
        public decimal TotalDays { get; set; }
        [Display(Name = "Leave Status")]
        public LeaveStatus Status { get; set; }
        [Display(Name = "Short Description")]
        [Required(ErrorMessage = "Enter short Description")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Short description length must be between 2 to 50 characters")]
        public string ShortDescription { get; set; }
        [Display(Name = "Rejection Reason")]
        public string RejectReason { get; set; }
        public string LeaveTypeName { get; set; }
        public string EmployeeName { get; set; }
        public string statusName => CommonFunctions.GetDisplayName<LeaveStatus>((int)Status);
        public string durationName => CommonFunctions.GetDisplayName<LeaveDuration>((int)LeaveDuration);
    }
}
