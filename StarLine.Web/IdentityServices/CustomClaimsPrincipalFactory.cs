using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StarLine.Core.Common;
using StarLine.Infrastructure.Repositories.Employees;
using System.Security.Claims;

namespace StarLine.Web.IdentityServices
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomClaimsPrincipalFactory(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmployeeRepository employeeRepository, IOptions<IdentityOptions> options, IHttpContextAccessor httpContextAccessor) : base(userManager, roleManager, options)
        {
            _roleManager = roleManager;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            string imageUrl = "/UserAvtars/default.png";
            var identity = await base.GenerateClaimsAsync(user);
            var employee = await _employeeRepository.GetEmployeeByEmail(user.Email);
            if (!string.IsNullOrEmpty(employee.Data.UserImages))
            {
                imageUrl = "/UserAvtars/" + employee.Data.UserImages;
            }
            var request = _httpContextAccessor.HttpContext?.Request;
            string imagefilePath = $"{request.Scheme}://{request.Host}" + imageUrl;

            // Fetch user details from the repository
            var identityRole = await _roleManager.FindByIdAsync(employee.Data.RoleId);
            // Override the default "Name" claim
            identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));
            identity.AddClaim(new Claim(ClaimTypes.Name, employee.Data.FirstName + " " + employee.Data.LastName));
            var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
            identity.AddClaims(roleClaims);
            identity.AddClaim(new Claim("UserId", employee.Data.Id.ToString()));
            identity.AddClaim(new Claim("FirstName", employee.Data.FirstName));
            identity.AddClaim(new Claim("LastName", employee.Data.LastName));
            identity.AddClaim(new Claim("RoleName", identityRole.Name));
            identity.AddClaim(new Claim("Email", employee.Data.Email));
            identity.AddClaim(new Claim("AspNetUser", user.Id));
            identity.AddClaim(new Claim("Avtar", imagefilePath));
            return identity;
        }
    }
}
