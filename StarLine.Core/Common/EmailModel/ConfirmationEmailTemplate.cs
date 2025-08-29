namespace StarLine.Core.Common.EmailModel
{
    public class ConfirmationEmailTemplate
    {
        public string Subject { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string ConfirmationLink { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
