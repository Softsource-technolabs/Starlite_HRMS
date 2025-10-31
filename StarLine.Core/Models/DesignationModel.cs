using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class DesignationModel : BaseEntity
    {
        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select Department")]
        public long DepartmentId { get; set; }
        [Display(Name = "Designation Name")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Designation Name must be between 2 to 150 characters")]
        public string DesignationName { get; set; } = null!;
        [Display(Name = "Designation description")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Designation Description can be null but must be between 10 to 500 characters")]
        public string Description { get; set; }
        [Display(Name = "Is Trainer")]
        [Required(ErrorMessage ="Please select is trainer")]
        public bool? IsTrainer { get; set; }
        public string DepartmentName { get; set; }
        [Display(Name = "Level")]
        [Required(ErrorMessage ="Please select Level")]
        public int HierarchyLevel { get; set; }
    }
}
