using System.ComponentModel.DataAnnotations;

namespace StarLine.Core.Models
{
    public class LoginModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Please enter Email Address")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter valid Email Address")]
        public string emailAddress { get; set; } = null!;
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter Password")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Please enter valid Email Address")]
        public string password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public string EmailAddress { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }

    public class ForgotPasswordModel
    {
        public string EmailAddress { get; set; } = null!;
    }

    public class ResetPasswordModel
    {
        public string EmailAddress { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
