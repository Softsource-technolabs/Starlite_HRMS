using Microsoft.AspNetCore.Identity;
using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Web.IdentityServices
{
    public interface IUserRepository
    {
        Task<ApiPostResponse<IdentityUser>> RegisterUser(RegisterModel model);
        Task<BaseApiResponse> ForgotPassword(ForgotPasswordModel model);
        Task<IdentityUser> GetUserById(string id);
        Task<IdentityUser> GetUserByEmail(string email);
        Task<bool> CheckUserRole(string AspNetUserId, string RoleId);
    }
}
