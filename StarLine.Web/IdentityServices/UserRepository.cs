using Microsoft.AspNetCore.Identity;
using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Web.IdentityServices
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StarliteEmailService _starLiteEmailService;
        public UserRepository(UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, RoleManager<IdentityRole> roleManager, StarliteEmailService starLiteEmailService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
            _starLiteEmailService = starLiteEmailService;
        }

        public async Task<ApiPostResponse<IdentityUser>> RegisterUser(RegisterModel model)
        {
            var user = CreateUser();
            user.UserName = model.EmailAddress;
            user.Email = model.EmailAddress;
            user.NormalizedUserName = model.EmailAddress;
            user.NormalizedEmail = model.EmailAddress;
            user.LockoutEnabled = false;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            string password = CommonFunctions.GenerateRandomPassword(8);

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);

                var isRoleAssigned = await _userManager.IsInRoleAsync(user, role.Name);
                if (!isRoleAssigned)
                    await _userManager.AddToRoleAsync(user, role.Name);

                await _starLiteEmailService.SendConfirmationAccountEmail(user, password, model.FullName);
                var userDetails = await _userManager.FindByEmailAsync(model.EmailAddress);
                return new ApiPostResponse<IdentityUser> { Data = userDetails, Success = true, Message = "User registration completed" };
            }
            return new ApiPostResponse<IdentityUser> { Success = false, Message = "User registration failed" };
        }
        public Task<BaseApiResponse> ForgotPassword(ForgotPasswordModel model)
        {
            throw new NotImplementedException();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        public async Task<IdentityUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckUserRole(string AspNetUserId, string RoleId)
        {
            var user = await _userManager.FindByIdAsync(AspNetUserId);
            var newRole = await _roleManager.FindByIdAsync(RoleId);
            var userCurrentRole = await _userManager.GetRolesAsync(user);

            if (!userCurrentRole.Contains(newRole.Name))
            {
                if (userCurrentRole.Any())
                    await _userManager.RemoveFromRoleAsync(user, userCurrentRole.FirstOrDefault());

                await _userManager.AddToRoleAsync(user, newRole.Name);
            }
            return true;
        }
    }
}
