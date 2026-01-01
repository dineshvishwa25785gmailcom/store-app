using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using store_app_apis.Service;

namespace store_app_apis.Container
{
    public class UserRoleService : IUserRoleService
    {
        private readonly Repos.StoreAppContext context;
        public UserRoleService(Repos.StoreAppContext learndata)
        {
            this.context = learndata;
        }
        public async Task<APIResponse> AssignRolePermission(List<TblRolepermission> _data)
        {
            APIResponse response = new APIResponse();
            int processcount = 0;
            try
            {

                using (var dbtransaction = await this.context.Database.BeginTransactionAsync())

                {
                    if (_data.Count > 0)
                    {
                        _data.ForEach(item =>
                        {
                            var userdata = this.context.TblRolepermissions.FirstOrDefault(item1 => item1.Userrole == item.Userrole && item1.Menucode == item.Menucode);
                            if (userdata != null)
                            {
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Haveedit = item.Haveedit;
                                userdata.Havedelete = item.Havedelete;
                                processcount++;
                            }
                            else if (item.Id > 0)
                            {
                                var userdata1 = this.context.TblRolepermissions.FirstOrDefault(item1 => item1.Id == item.Id);
                                if (userdata1 != null)
                                {
                                    this.context.TblRolepermissions.Remove(userdata1);
                                }
                            }
                            else
                            {
                                this.context.TblRolepermissions.Add(item);
                                processcount++;
                            }

                        });
                        if (_data.Count == processcount)
                        {
                            await this.context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            response.Result = "pass";
                            response.ErrorMessage = "Data saved successfully";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                            response.Result = "fail";
                            response.ErrorMessage = "Failed to save";
                        }
                    }
                    else
                    {
                        response.Result = "fail";
                        response.ErrorMessage = "No data to save";
                    }
                }

            }
            catch (Exception ex)
            {
                response.Result = "fail";
                response.ErrorMessage = "Failed exception : No data to save";
            }

            return response;
        }

        public async Task<List<TblMenu>> GetAllMenus()
        {
            return await this.context.TblMenus.ToListAsync();
        }

        public async Task<List<TblRole>> GetAllRoles()
        {
            return await this.context.TblRoles.ToListAsync();
        }

       public async Task<List<AppMenus>> GetAllMenusByRole(string userrole)
        {
            List<AppMenus> appMenus = new List<AppMenus>();
            var accessdata = (from menu in this.context.TblRolepermissions.Where(o => o.Userrole == userrole && o.Haveview) join m in this.context.TblMenus on menu.Menucode equals m.Code into _jointable from p in _jointable.DefaultIfEmpty() select new { code = menu.Menucode, name = p.Name }).ToList();
            if (accessdata.Any())
            {
                //foreach (var item in accessdata)
                //{
                //    AppMenus appMenu = new AppMenus();
                //    appMenu.Menucode = item.code;
                //    appMenu.Menuname = item.name;
                //    appMenus.Add(appMenu);
                //}
                accessdata.ForEach(item =>
                {
                    appMenus.Add(new AppMenus
                    {
                        Menucode = item.code,
                        Menuname = item.name
                    });
                });
            }
            return appMenus;
        }

        public async Task<MenuPermission> GetAllMenusPermissionByRole(string userrole, string menucode)
        {
            MenuPermission menuPermission = new MenuPermission();
            var _data = await this.context.TblRolepermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Menucode == menucode);
            if (_data != null)
            {
                {
                    menuPermission.Menucode = _data.Menucode;
                    menuPermission.Haveview = _data.Haveview;
                    menuPermission.Haveadd = _data.Haveadd;
                    menuPermission.Haveedit = _data.Haveedit;
                    menuPermission.Havedelete = _data.Havedelete;
                }
            }
            return menuPermission;
        }
    }
}
