namespace StarLine.Core.Common
{
    public class UserSessionModel
    {
        public long UserId { get; set; }
        public string AspNetId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string UserImage { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
    }
}
