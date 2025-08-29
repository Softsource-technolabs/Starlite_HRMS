using StarLine.Core.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class LeaveTypeModel : BaseEntity
    {
        [DisplayName("Leave Type Name")]
        [Required(ErrorMessage = "Leave Type Name required")]
        public string TypeName { get; set; } = null!;
        [DisplayName("Description")]
        public string Description { get; set; } = null!;
        [DisplayName("Max Days per Month")]
        [Required(ErrorMessage = "No of Days Per Month Required")]
        public int MaxDaysPerMonth { get; set; }
        [DisplayName("Is Leave carry forward?")]
        public bool? IsCarryForward { get; set; }
        [DisplayName("Is Gender Specified?")]
        public int GenderContraint { get; set; }
        public string genderName => GenderContraint == 0 ? "No Specified" : GenderContraint == 1 ? "Male" : "Female";
    }
}
