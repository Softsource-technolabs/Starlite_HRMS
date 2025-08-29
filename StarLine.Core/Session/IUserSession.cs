using StarLine.Core.Common;

namespace StarLine.Core.Session
{
    public interface IUserSession
    {
        UserSessionModel Current { get; set; }
    }
}
