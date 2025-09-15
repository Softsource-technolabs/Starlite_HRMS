using StarLine.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class BranchModel : BaseEntity
    {
        [Display(Name ="Branch Code")]
        [Required(ErrorMessage ="Enter branch code")]
        [StringLength(10,MinimumLength =2,ErrorMessage ="Branch code length must be 2 to 10 characters")]
        public string BranchCode { get; set; }
        [Display(Name = "Branch Name")]
        [Required(ErrorMessage = "Enter branch name")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name length must be 2 to 100 characters")]
        public string BranchName { get; set; }
        [Display(Name = "Branch Full Address")]
        [Required(ErrorMessage = "Enter Address")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Address length must be 2 to 500 characters")]
        public string Address { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter branch City")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City length must be 2 to 50 characters")]
        public string City { get; set; }
        [Display(Name = "State")]
        [Required(ErrorMessage = "Enter branch state")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State length must be 2 to 50 characters")]
        public string State { get; set; }
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Enter branch country")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Country length must be 2 to 50 characters")]
        public string Country { get; set; }
        [Display(Name = "Contract Number")]
        [Required(ErrorMessage = "Enter branch contact no")]
        [StringLength(10, ErrorMessage = "Contact No length must be 2 to 50 characters")]
        [RegularExpression("^(?:\\+?[1-9]\\d{1,14}|0\\d{2,4}-?\\d{6,8}|[6-9]\\d{9})$", ErrorMessage ="Enter valid mobile no or Land line No")]
        public string ContactNumber { get; set; }
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Enter email Address")]
        [RegularExpression("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$",ErrorMessage ="Please enter valid Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Parent Branch")]
        public int? ParentBranchId { get; set; }
    }
}
