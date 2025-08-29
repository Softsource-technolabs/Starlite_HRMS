using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class DepartmentModel : BaseEntity
    {
        [Display(Name = "Department Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Department Name must be between 2 to 50 characters")]
        public string DepartmentName { get; set; } = null!;
        [Display(Name = "Department description")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Department Description can be null but must be between 10 to 500 characters")]
        public string Description { get; set; }
    }
}
