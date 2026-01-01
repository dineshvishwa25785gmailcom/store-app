using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using store_app_apis.Repos;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace store_app_apis.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly StoreAppContext context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, StoreAppContext context) : base(options, logger, encoder, clock)
        {
            this.context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No Header Found");
            }
            var haedervalues = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (haedervalues != null)
            {
                var bytes = Convert.FromBase64String(haedervalues.Parameter);
                string credentials = Encoding.ASCII.GetString(bytes);
                string[] array = credentials.Split(":");
                string username = array[0];
                string password = array[1];
                var user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Password == password);
                if (user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Username) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("UnAuthorized");
                }
            }
            else
            {
                return AuthenticateResult.Fail("Empty Header");
            }
        }
    }
}
