using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using store_app_apis.Modal;
using store_app_apis.Repos;
using store_app_apis.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace store_app_apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly StoreAppContext context;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshHandler refresh;
        public AuthorizeController(StoreAppContext context, IOptions<JwtSettings> options, IRefreshHandler refresh)
        {
            this.context = context;
            this.jwtSettings = options.Value;
            this.refresh = refresh;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
        {
            var user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == userCred.UserName && item.Password == userCred.Password);
            if (user != null)
            {
                // Generate Token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.ASCII.GetBytes(this.jwtSettings.securitykey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,user.Username),
                        new Claim(ClaimTypes.Role,user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(3000),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenhandler.CreateToken(tokendesc);
                var finaltoken = tokenhandler.WriteToken(token);
                
                //return Ok(finaltoken);// this direct response to the client
                return Ok(new TokenResponse() { Token = finaltoken, RefreshToken = await this.refresh.GenerateToken(userCred.UserName),UserRole= user.Role});
            }
            else
            {
                return Unauthorized();
            }
            return Ok("");
        }


        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            var _refreshtoken = await this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Refreshtoken == token.RefreshToken);
            if (_refreshtoken != null)
            {
                // Generate Token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.ASCII.GetBytes(this.jwtSettings.securitykey);
                SecurityToken securityToken;
                var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,/// If this false it means you will get Unable to verify the first certificate in post man and a-string-secret-at-least-256-bits-long
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);
                var _token = securityToken as JwtSecurityToken;
                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principal.Identity?.Name;
                    var _existdata = await this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Userid == username && item.Refreshtoken == token.RefreshToken);
                    if (_existdata != null)
                    {
                        var _newtoken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.jwtSettings.securitykey)), SecurityAlgorithms.HmacSha256)
                        );

                        var _finaltoken = tokenhandler.WriteToken(_newtoken);
                        return Ok(new TokenResponse() { Token = _finaltoken, RefreshToken = await this.refresh.GenerateToken(username), UserRole = token.UserRole });
                    }
                    else { return Unauthorized(); }

                }
                else
                {
                    return Unauthorized();
                }

                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //        new Claim(ClaimTypes.Name,user.Username),
                //        new Claim(ClaimTypes.Role,user.Role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256Signature)
                //};
                //var token = tokenhandler.CreateToken(tokendesc);
                //var finaltoken = tokenhandler.WriteToken(token);
                ////return Ok(finaltoken);// this direct response to the client
                //return Ok(new TokenResponse() { Token = finaltoken, RefreshToken = await this.refresh.GenerateToken(userCred.UserName) });
            }
            else
            {
                return Unauthorized();
            }
            return Ok("");
        }
    }
}
