using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Session;
using StarLine.Infrastructure.Repositories.Employees;

namespace StarLine.Web.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    public class EmployeeController(IEmployeeRepository employeeRepository, IUserSession userSession, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IUserSession _userSession = userSession;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public async Task<IActionResult> ViewProfile()
        {
            var response = await _employeeRepository.GetEmployeeDetailsById(_userSession.Current.UserId);
            string imageUrl = "/UserAvtars/default.png";
            if (!string.IsNullOrEmpty(response.UserImages))
            {
                imageUrl = "/UserAvtars/" + response.UserImages;
            }
            var request = _httpContextAccessor.HttpContext?.Request;
            string imagefilePath = $"{request.Scheme}://{request.Host}" + imageUrl;
            response.UserImages = imagefilePath;
            return View(response);
        }
    }
}
