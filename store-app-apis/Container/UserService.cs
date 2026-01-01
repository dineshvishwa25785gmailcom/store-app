using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using store_app_apis.Helper;
using store_app_apis.Modal;
using store_app_apis.Repos;
using store_app_apis.Repos.Models;
using store_app_apis.Service;

namespace store_app_apis.Container
{
    public class UserService : IUserService
    {
        private readonly Repos.StoreAppContext context;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        public UserService(Repos.StoreAppContext learndata, IMapper mapper, IEmailService emailService)
        {
            this.context = learndata;
            this.mapper = mapper;
            this.emailService = emailService;
        }
        public async Task<APIResponse> ConfirmRegister(int userid, string username, string otptext)
        {
            APIResponse response = new APIResponse();
            bool otpresponse = await ValidateOTP(username, otptext);
            if (!otpresponse)
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid OTP";
                return response;
            }
            else
            {
                var _tempdata = await this.context.TblTempusers.FirstOrDefaultAsync(x => x.Id == userid);
                var _user = new TblUser
                {
                    Name = _tempdata.Name,
                    Username = _tempdata.Code,
                    Email = _tempdata.Email,
                    Phone = _tempdata.Phone,
                    Password = _tempdata.Password,
                    Failattempt = 0,
                    Isactive = true,
                    Islocked = false,
                    Role = "user"

                };
                await this.context.TblUsers.AddAsync(_user);
                await this.context.SaveChangesAsync();
                await UpdatePWDManager(username, _tempdata.Password);
                response.Result = "pass";
                response.ErrorMessage = "User Created Successfully";

            }

            return response;
        }

        public async Task<APIResponse> UserRegistration(UserRegister userRegister)
        {
            APIResponse response = new APIResponse();
            int userid = 0;
            try
            {
                //check if user already exists
                var _user = await this.context.TblUsers.Where(item => item.Username == userRegister.UserName).ToListAsync();
                if (_user != null && _user.Count > 0)
                {
                    response.Result = "fail";
                    response.ErrorMessage = "User already exists";
                    return response;
                }
                //check if user email already exists
                var _useremail = await this.context.TblUsers.Where(item => item.Email == userRegister.UserName).ToListAsync();
                if (_useremail != null && _useremail.Count > 0)
                {
                    response.Result = "fail";
                    response.ErrorMessage = "This is duplicate image";
                    return response;
                }
                if (userRegister != null)
                {
                    var _tempuser = new TblTempuser
                    {
                        Code = userRegister.UserName,
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Phone = userRegister.Phone,
                        Password = userRegister.Password
                    };
                    await this.context.TblTempusers.AddAsync(_tempuser);
                    await this.context.SaveChangesAsync();
                    userid = _tempuser.Id;
                    string OtpText = GenerateOtp();
                    await UpdateOtp(userRegister.UserName, OtpText, "register");
                    await SendOtpMail(userRegister.Email, OtpText, userRegister.Name);
                    response.Result = "pass";
                    response.ErrorMessage = userid.ToString();
                }
                else
                {
                    response.Result = "fail";
                }
            }
            catch (Exception ex)
            {
                response = new APIResponse();
            }
            return response;

        }

        private async Task UpdateOtp(string username, string otptext, string otptype)
        {
            var _otp = new TblOtpManager
            {
                Username = username,
                Otptext = otptext,
                Otptype = otptype,
                Expiration = DateTime.Now.AddMinutes(5),
                Createddate = DateTime.Now
            };
            await this.context.TblOtpManagers.AddAsync(_otp);
            await this.context.SaveChangesAsync();
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(0, 1000000).ToString("D6");
        }

        private async Task SendOtpMail(string useremail, string otptext, string name)
        {
            //send mail code
            var mailrequest = new Mailrequest();
            mailrequest.ToEmail = useremail;
            mailrequest.Subject = "OTP for registration";
            mailrequest.Body = GenerateEmailBody(name, otptext);
            await this.emailService.SendEmail(mailrequest); 

        }
        private string GenerateEmailBody(string name, string otptext)
        {
            string body = "<div style='width:100%;background-color:grey;'> <p>Dear " + name + ",</p>";
            body += "<p>Your OTP for registration is: <strong>" + otptext + "</strong></p>";
            body += "<p>Thank you.</p>";
            body += "</div>";
            return body;
        }
        private async Task<bool> ValidateOTP(string username, string otptext)
        {
            bool response = false;
            var _data = await this.context.TblOtpManagers.FirstOrDefaultAsync(x => x.Username == username && x.Otptext == otptext && x.Expiration > DateTime.Now);
            if (_data != null)
            {
                response = true;
            }
            return response;

        }

        private async Task UpdatePWDManager(string username, string password)
        {
            var _otp = new TblPwdManger
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now
            };
            await this.context.TblPwdMangers.AddAsync(_otp);
            await this.context.SaveChangesAsync();
        }

        public async Task<APIResponse>  ResetPassword(string username, string oldpassword, string newpassword)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(x => x.Username == username && x.Password == oldpassword && x.Isactive == true);
            if (_user != null)
            {
                var _pwd = await ValidatepwdHistory(username, newpassword);
                if (_pwd)
                {
                    response.Result = "fail";
                    response.ErrorMessage = "Don't use the same password that used in last 3 transaction.";
                    return response;
                }
                else
                {
                    _user.Password = newpassword;
                    await this.context.SaveChangesAsync();
                    await UpdatePWDManager(username, newpassword);
                    response.Result = "pass";
                    response.ErrorMessage = "Password updated successfully";
                }
            }
            else
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid username or password";
            }
            return response;
        }


        private async Task<bool> ValidatepwdHistory(string username, string password)
        {
            bool response = false;
            var _pwd = await this.context.TblPwdMangers.Where(x => x.Username == username).OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd != null && _pwd.Count > 0)
            {
                var validate = _pwd.Where(x => x.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }
            return response;
        }

       public async  Task<APIResponse> ForgotPassword(string username)
        {
            APIResponse response = new APIResponse();  
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(x => x.Username == username && x.Isactive == true);
            if (_user != null)
            {
                string otptext = GenerateOtp();
                await UpdateOtp(username, otptext, "forgotpassword");
                await SendOtpMail(_user.Email, otptext, _user.Name);
                response.Result = "pass";
                response.ErrorMessage = "OTP sent to your registered email id";
            }

            else
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid username or password";
            }
                return response;
        }

       public async Task<APIResponse> UpdatePassword(string username, string password, string otptext)
        {
            APIResponse response = new APIResponse();
            bool otpvalidation = await ValidateOTP(username, otptext);
            if (!otpvalidation)
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid OTP";
                return response;
            }
            else
            {
                bool pwdhistory = await ValidatepwdHistory(username, password);
                if (pwdhistory)
                {
                    response.Result = "fail";
                    response.ErrorMessage = "Don't use the same password that used in last 3 transaction.";
                    return response;
                }
                else
                {
                    var _user = await this.context.TblUsers.FirstOrDefaultAsync(x => x.Username == username && x.Isactive == true);
                    if (_user != null)
                    {
                        _user.Password = password;
                        await this.context.SaveChangesAsync();
                        await UpdatePWDManager(username, password);
                        response.Result = "pass";
                        response.ErrorMessage = "Password updated successfully";
                    }
                    else
                    {
                        response.Result = "fail";
                        response.ErrorMessage = "Invalid username or password";
                    }
                }
            }
                return response;
        }

       public async  Task<APIResponse> UpdateStatus(string username, bool userstatus)
        {
           APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(x => x.Username == username);
            if (_user != null)
            {
                _user.Isactive = userstatus;
                await this.context.SaveChangesAsync();
                response.Result = "pass";
                response.ErrorMessage = "User status updated successfully";
            }
            else
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid username";
            }
            return response;
        }

       public async Task<APIResponse> UpdateRole(string username, string userrole)
        {
            APIResponse response = new APIResponse();
            var _user = await this.context.TblUsers.FirstOrDefaultAsync(x => x.Username == username);
            if (_user != null)
            {
                _user.Role = userrole;
                await this.context.SaveChangesAsync();
                response.Result = "pass";
                response.ErrorMessage = "User role updated successfully";
            }
            else
            {
                response.Result = "fail";
                response.ErrorMessage = "Invalid username";
            }
            return response;
        }

        public async Task<List<UserModel>> Getall()
        {
            try
            {
                var _data = await this.context.TblUsers.ToListAsync();
                return this.mapper.Map<List<TblUser>, List<UserModel>>(_data);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Log.Error(ex, "Error retrieving all users");
                return new List<UserModel>(); // Return an empty list in case of an error
            }
        }

        public async Task<UserModel> Getbycode(string code)
        {
            UserModel _response = new UserModel();
            var _data = await this.context.TblUsers.FindAsync(code);
            if (_data != null)
            {
                _response = this.mapper.Map<TblUser, UserModel>(_data);
            }
            return _response;
        }
    }
}
