using StarLine.Core.Common;

namespace StarLine.Core.Session
{
    public class UserSession : IUserSession
    {
        public UserSessionModel Current { get; set; } = new UserSessionModel();
    }
}
