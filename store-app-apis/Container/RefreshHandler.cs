using Microsoft.EntityFrameworkCore;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using store_app_apis.Service;
using System.Security.Cryptography;

namespace store_app_apis.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly Repos.StoreAppContext context;
        public RefreshHandler(Repos.StoreAppContext context)
        {
            this.context = context;
        }
        public async Task<string> GenerateToken(string userid)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var Existtoken = this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.Userid == userid).Result;
                if (Existtoken != null)
                {
                    Existtoken.Refreshtoken = refreshtoken;
                    await this.context.SaveChangesAsync();
                }
                else
                {
                  await this.context.TblRefreshtokens.AddAsync(new TblRefreshtoken
                  {
                      Userid = userid,
                      Tokenid = new Random().Next().ToString(),
                      Refreshtoken = refreshtoken
                  });
                }
                await this.context.SaveChangesAsync();
                return refreshtoken;

            }
        }
    }
}
