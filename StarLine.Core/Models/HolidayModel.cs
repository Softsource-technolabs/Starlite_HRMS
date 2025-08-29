using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class HolidayModel : BaseEntity
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Holiday Name required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name can be mini 2 and Max 150 characters")]
        public string Name { get; set; } = null!;
        [Display(Name = "Date")]
        public DateOnly HolidayDate { get; set; }
        [Display(Name = "Type")]
        [Required(ErrorMessage = "Please select Holiday Type")]
        public HolidayTypes HolidayType { get; set; }
        [Display(Name = "Other Type name")]
        [RequiredIf("HolidayType", "5", ErrorMessage = "Other Type Name is required when Holiday Type is 'Other'.")]
        public string? HolidayTypename { get; set; }
        [Display(Name = "Is public Holiday")]
        public bool? Ismandatory { get; set; }
        public string? typeName => HolidayType == HolidayTypes.Other ? HolidayTypename : Enum.GetName(typeof(HolidayTypes), HolidayType);
    }
}
