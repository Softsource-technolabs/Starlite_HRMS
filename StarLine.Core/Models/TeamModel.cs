using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class TeamModel : BaseEntity
    {
        [Display(Name = "Team Name")]
        [Required(ErrorMessage = "Team name required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Team Name length can be 2-150 characters")]
        public string Name { get; set; } = null!;
        [Display(Name = "Department")]
        [Required(ErrorMessage = "Select Department")]
        public long DepartmentId { get; set; }
        [Display(Name = "Team Type")]
        public int TeamType { get; set; }
        [Display(Name = "Description")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Team Name length can be 2-500 characters")]
        public string Description { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string TeamTypeName => CommonFunctions.GetDisplayName<TeamType>(TeamType);
    }
}
