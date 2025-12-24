using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_app_apis.Modal;
using store_app_apis.Repos.Models;
using store_app_apis.Service;

namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService userRole;
        public UserRoleController(IUserRoleService roleService) { 
            this.userRole = roleService;
        }

        [HttpPost("asignrolepermission")]
        public async Task<IActionResult> asignrolepermission(List<TblRolepermission> rolepermissions)
        {
            var data = await this.userRole.AssignRolePermission(rolepermissions);
            return Ok(data);
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await this.userRole.GetAllRoles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await this.userRole.GetAllMenus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetAllMenusByRole")]
        public async Task<IActionResult> GetAllMenusByRole(string userrole)
        {
            var data = await this.userRole.GetAllMenusByRole(userrole);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetMenusPermissionByRole")]
        public async Task<IActionResult> GetMenusPermissionByRole(string userrole,string menucode)
        {
            var data = await this.userRole.GetAllMenusPermissionByRole(userrole, menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
