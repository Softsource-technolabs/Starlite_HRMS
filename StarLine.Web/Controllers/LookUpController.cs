using Microsoft.AspNetCore.Mvc;
using StarLine.Infrastructure.Repositories.Lists;

namespace StarLine.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class LookUpController(ILookUpRepository lookUpRepository) : ControllerBase
    {
        private readonly ILookUpRepository _lookUpRepository = lookUpRepository;

        [HttpGet]
        [Route("departments")]
        public async Task<IActionResult> GetAllDepartments([FromQuery] string searchText) => Ok(await _lookUpRepository.GetAllDepartment(searchText));

        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetAllRoles([FromQuery] string searchText) => Ok(await _lookUpRepository.GetAllRoles(searchText));

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] string searchText) => Ok(await _lookUpRepository.GetAllUsers(searchText));
    }
}
