using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using store_app_apis.Modal;
using store_app_apis.Service;
using System.Data;
using System.Diagnostics;

namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService service)

        {
            this.userService = service;
        }

        [HttpPost("userregistration")]
        public async Task<IActionResult> UserRegistration(UserRegister userRegister)
        {
            var data =await this.userService.UserRegistration(userRegister);
            return Ok(data);
        }

        [HttpPost("confirmregisteration")]
        public async Task<IActionResult> confirmregisteration(Confirmpassword _data)
        {
            var data = await this.userService.ConfirmRegister(_data.userid, _data.username, _data.otptext);
            return Ok(data);
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> resetpassword(Resetpassword _data)
        {
            var data = await this.userService.ResetPassword(_data.username, _data.oldpassword, _data.newpassword);
            return Ok(data);
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> forgotpassword(string username )
        {
            var data = await this.userService.ForgotPassword(username);
            return Ok(data);
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> updatepassword(Updatepassword _data)
        {
            var data = await this.userService.UpdatePassword(_data.username, _data.password, _data.otptext);
            return Ok(data);
        }

        [HttpPost("updatestatus")]
        public async Task<IActionResult> updatestatus(Updatestatus _data)
        {
            var data = await this.userService.UpdateStatus(_data.username, _data.status);
            return Ok(data);
        }
        [HttpPost("updaterole")]
        public async Task<IActionResult> updaterole(UpdateRole _data)
        {
            var data = await this.userService.UpdateRole(_data.username, _data.role);
            return Ok(data);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await this.userService.Getall();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("Getbycode")]
        public async Task<IActionResult> Getbycode(string code)
        {
            var data = await this.userService.Getbycode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
