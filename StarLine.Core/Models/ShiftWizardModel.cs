using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class ShiftWizardModel
    {
        public ShiftGroupModel ShiftGroup { get; set; }
        public List<ShiftModel> Shifts { get; set; }
        [Display(Name = "Sequence No")]
        [Required(ErrorMessage = "Select shift Sequence")]
        public int SequenceNo { get; set; }
        [Display(Name = "Rotate shift after")]
        public int? RotationDays { get; set; }

    }

    public class ShiftGroupModel : BaseEntity
    {
        [Display(Name = "Group Code")]
        [Required(ErrorMessage = "Group code required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Group Code Name length must be between 2 to 20 characters")]
        public string GroupCode { get; set; }
        [Display(Name = "Group Name")]
        [Required(ErrorMessage = "Group name required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group Code Name length must be between 2 to 100 characters")]
        public string GroupName { get; set; }
        [Display(Name = "Shift Rotation Type")]
        [Required(ErrorMessage = "Shift Rotation required")]
        public int RotationType { get; set; }
        [Display(Name = "Description")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Group Code Name length must be between 2 to 200 characters")]
        public string Description { get; set; }
        [Display(Name = "Effective From")]
        [Required(ErrorMessage = "Effective from date required")]
        public DateOnly? EffectiveFrom { get; set; }
        [Display(Name = "Effective To")]
        public DateOnly? EffectiveTo { get; set; }
        public string RotationTypeName => CommonFunctions.GetDisplayName<ShiftRotationType>(RotationType);

    }

    public class ShiftModel : BaseEntity
    {
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Shift code required")]
        public string ShiftCode { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Shift name required")]
        public string ShiftName { get; set; }
        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Shift Start Time required")]
        public TimeOnly? StartTime { get; set; }
        [Display(Name = "End Time")]
        [Required(ErrorMessage = "Shift End Time required")]
        public TimeOnly? EndTime { get; set; }
        [Display(Name = "Working Hours")]
        [Required(ErrorMessage = "Shift Working Hours required")]
        public decimal WorkingHours { get; set; }
        [Display(Name = "Is Night Shift?")]
        [Required(ErrorMessage = "Please slect option")]
        public bool? IsNightShift { get; set; }
        [Display(Name = "Flexible Time (in Mins)")]
        public int? GracePeriodMins { get; set; }
    }

    public class ShiftGroupMappingModel : BaseEntity
    {
        public long ShiftGroupId { get; set; }
        public long ShiftId { get; set; }
        [Display(Name ="Sequence No")]
        [Required(ErrorMessage = "Select shift Sequence")]
        public int SequenceNo { get; set; }
        [Display(Name = "Rotate shift after")]
        public int? RotationDays { get; set; }
    }
}
