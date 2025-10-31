using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class TrainingModel : BaseEntity
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Duration (in Minutes)")]
        public int Duration { get; set; }
        [Display(Name = "Validity (in Months)")]
        public int ValidityMonths { get; set; }
    }
}
