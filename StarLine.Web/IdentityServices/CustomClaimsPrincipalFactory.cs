using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StarLine.Infrastructure.Repositories.Employees;
using System.Security.Claims;

namespace StarLine.Web.IdentityServices
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomClaimsPrincipalFactory> _logger;

        public CustomClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmployeeRepository employeeRepository,
            IOptions<IdentityOptions> options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomClaimsPrincipalFactory> logger)
            : base(userManager, roleManager, options)
        {
            _roleManager = roleManager;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            try
            {
                var employee = await _employeeRepository.GetEmployeeByEmail(user.Email);
                if (employee?.Data == null)
                {
                    _logger.LogWarning("No employee record found for user {Email}", user.Email);
                    return identity;
                }

                // Avatar URL resolution
                string imageUrl = string.IsNullOrEmpty(employee.Data.UserImages)
                    ? "/UserAvtars/default.png"
                    : "/UserAvtars/" + employee.Data.UserImages;

                string imagefilePath = imageUrl;
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request != null)
                {
                    imagefilePath = $"{request.Scheme}://{request.Host}{imageUrl}";
                }

                // Replace default name claim
                var nameClaim = identity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                    identity.RemoveClaim(nameClaim);

                identity.AddClaim(new Claim(ClaimTypes.Name, $"{employee.Data.FirstName} {employee.Data.LastName}"));

                // Replace default role claim
                var roleClaim = identity.FindFirst(ClaimTypes.Role);
                if (roleClaim != null)
                    identity.RemoveClaim(roleClaim);

                var identityRole = await _roleManager.FindByIdAsync(employee.Data.RoleId);
                if (identityRole != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, identityRole.Name));
                    var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                    identity.AddClaims(roleClaims);
                }
                else
                {
                    _logger.LogWarning("Role not found for RoleId {RoleId}", employee.Data.RoleId);
                }

                // Add custom claims
                identity.AddClaim(new Claim("UserId", employee.Data.Id.ToString()));
                identity.AddClaim(new Claim("FirstName", employee.Data.FirstName));
                identity.AddClaim(new Claim("LastName", employee.Data.LastName));
                identity.AddClaim(new Claim("Email", employee.Data.Email));
                identity.AddClaim(new Claim("AspNetUser", user.Id));
                identity.AddClaim(new Claim("Avtar", imagefilePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating claims for user {UserId}", user.Id);
            }

            return identity;
        }
    }
}
