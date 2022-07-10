using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SuperSmartShopping.API.Models;
using SuperSmartShopping.API.Providers;
using SuperSmartShopping.API.Results;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.BAL.Services;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Entity;
using SuperSmartShopping.DAL.Enums;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private IUserBusiness _userBusiness;
        private IRiderBusiness _riderBusiness;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat, IUserBusiness userBusiness, IRiderBusiness riderBusiness)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            _userBusiness = userBusiness;
            _riderBusiness = riderBusiness;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<HttpResponseMessage> Register(RegisterBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .FirstOrDefault();
                    var err = new
                    {
                        error = errors.FirstOrDefault().Exception,
                        error_description = errors.FirstOrDefault().ErrorMessage
                    };
                    return Request.CreateResponse(HttpStatusCode.BadRequest, err);
                }
                var user = new ApplicationUser();
                if (model.RoleName == RolesEnum.Admin.ToString())
                {
                    user = new ApplicationUser() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.Email, Email = model.Email, PhoneNumber = model.MobileNumber, CreatedBy = model.CreatedBy };
                }
                else if (model.RoleName == RolesEnum.Rider.ToString())
                {
                    user = new ApplicationUser() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.MobileNumber, Email = model.Email, PhoneNumber = model.MobileNumber, CreatedBy = model.CreatedBy };
                }
                else
                {
                    user = new ApplicationUser() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.UserId, Email = model.Email, PhoneNumber = model.MobileNumber, CreatedBy = model.CreatedBy };
                }
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        foreach (string error in result.Errors)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                        }
                    }
                }

                #region "Role section"
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                if (!roleManager.RoleExists(RolesEnum.SuperAdmin.ToString()))
                {
                    roleManager.Create(new IdentityRole { Name = RolesEnum.SuperAdmin.ToString() });
                }
                if (!roleManager.RoleExists(RolesEnum.Admin.ToString()))
                {
                    roleManager.Create(new IdentityRole { Name = RolesEnum.Admin.ToString() });
                }
                if (!roleManager.RoleExists(RolesEnum.Partner.ToString()))
                {
                    roleManager.Create(new IdentityRole { Name = RolesEnum.Partner.ToString() });
                }
                if (!roleManager.RoleExists(RolesEnum.User.ToString()))
                {
                    roleManager.Create(new IdentityRole { Name = RolesEnum.User.ToString() });
                }
                if (!roleManager.RoleExists(RolesEnum.Rider.ToString()))
                {
                    roleManager.Create(new IdentityRole { Name = RolesEnum.Rider.ToString() });
                }
                if (model.RoleName.ToLower() == RolesEnum.SuperAdmin.ToString().ToLower())
                {
                    UserManager.AddToRole(user.Id, RolesEnum.SuperAdmin.ToString());
                }
                else if (model.RoleName.ToLower() == RolesEnum.Admin.ToString().ToLower())
                {
                    UserManager.AddToRole(user.Id, RolesEnum.Admin.ToString());
                }
                else if (model.RoleName.ToLower() == RolesEnum.Partner.ToString().ToLower())
                {
                    UserManager.AddToRole(user.Id, RolesEnum.Partner.ToString());
                }
                else if (model.RoleName.ToLower() == RolesEnum.User.ToString().ToLower())
                {
                    UserManager.AddToRole(user.Id, RolesEnum.User.ToString());
                }
                else if (model.RoleName.ToLower() == RolesEnum.Rider.ToString().ToLower())
                {
                    UserManager.AddToRole(user.Id, RolesEnum.Rider.ToString());
                }
                #endregion

                #region "creating tedency for resturant"
                DefaultConnection db = new DefaultConnection();
                if (model.RoleName.ToLower() == RolesEnum.Admin.ToString().ToLower())
                {
                    string dbconnectionStr = ConfigurationManager.AppSettings["serverDefaultConnection"].ToString();
                    var data = UserManager.FindByEmail(model.Email);
                    string dbName = string.Format("{0}_{1}_{2}", model.Email.Substring(0, model.Email.IndexOf("@")).ToString(), DateTime.Now.Ticks, "TE");
                    tb_Tenants obj = new tb_Tenants();
                    obj.UserId = data.Id;
                    obj.TenantName = model.Email;
                    obj.TenantSchema = "default";
                    obj.TenantConnection = dbconnectionStr.Replace("{0}", dbName);
                    obj.TenantDomain = model.SubDomainName;
                    obj.TenantDatabaseName = dbName;
                    obj.CreatedBy = model.CreatedBy;
                    db.TenantControl.Add(obj);
                    db.SaveChanges();
                    using (var connection = new SqlConnection(obj.TenantConnection))
                    {
                        TenantConnection.ProvisionTenant("dbo", connection);
                    }
                }
                #endregion

                #region "sending OTP"
                if (model.RoleName.ToLower() == RolesEnum.User.ToString().ToLower())
                {
                    string otpStr = CommonManager.GenerateRandomNo().ToString();
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                    {
                        if (SMSManager.SendSMSNotification(model.MobileNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                        {
                            var res = await UserManager.FindByEmailAsync(model.Email);
                            res.OTP = otpStr;
                            res.OTPCreatedDate = DateTime.UtcNow;
                            res.CreatedBy = model.CreatedBy;
                            await UserManager.UpdateAsync(res);
                            return Request.CreateResponse(HttpStatusCode.OK, model.MobileNumber);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Your account has been created but an error occured while sending OTP, Please contact site administration.");
                        }
                    }
                    else
                    {
                        var res = await UserManager.FindByEmailAsync(model.Email);
                        res.OTP = otpStr;
                        res.OTPCreatedDate = DateTime.UtcNow;
                        res.CreatedBy = model.CreatedBy;
                        await UserManager.UpdateAsync(res);
                        return Request.CreateResponse(HttpStatusCode.OK, model.MobileNumber);
                    }
                }
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, model);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("VerifyOTP")]
        [HttpPost]
        public async Task<HttpResponseMessage> VerifyOTP(string OTP, string mobileNumber)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(mobileNumber);
                if (user != null)
                {
                    if (user.OTP == OTP.Trim())
                    {
                        TimeSpan span = DateTime.UtcNow.Subtract(user.OTPCreatedDate.Value);
                        if (span.TotalMinutes <= Convert.ToInt32(ConfigurationManager.AppSettings["MaxOTPValidateTimeInMin"].ToString()))
                        {
                            user.OTP = string.Empty;
                            user.OTPCreatedDate = null;
                            user.PhoneNumberConfirmed = true;
                            await UserManager.UpdateAsync(user);
                            return Request.CreateResponse(HttpStatusCode.OK, user);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "This OTP has been expired");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid OTP");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This is invaid OTP please contact site administrator");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }

        }

        [AllowAnonymous]
        [Route("GenerateOTP")]
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateOTP(string mobileNumber, string tedencyCreatedBy)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(mobileNumber);
                tedencyCreatedBy = !string.IsNullOrEmpty(tedencyCreatedBy) ? tedencyCreatedBy : new UserBusiness().GetUsersByRoleName(RolesEnum.SuperAdmin.ToString()).Select(c => c.UserId).FirstOrDefault();
                if (user != null && user.CreatedBy == tedencyCreatedBy)
                {
                    string otpStr = CommonManager.GenerateRandomNo().ToString();
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                    {
                        if (SMSManager.SendSMSNotification(user.PhoneNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                        {
                            var res = await UserManager.FindByNameAsync(mobileNumber);
                            res.OTP = otpStr;
                            res.OTPCreatedDate = DateTime.UtcNow;
                            await UserManager.UpdateAsync(res);
                            return Request.CreateResponse(HttpStatusCode.OK, 1);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occured while generating OTP, please contact site administrator.");
                        }
                    }
                    else
                    {
                        var res = await UserManager.FindByNameAsync(mobileNumber);
                        res.OTP = otpStr;
                        res.OTPCreatedDate = DateTime.UtcNow;
                        await UserManager.UpdateAsync(res);
                        return Request.CreateResponse(HttpStatusCode.OK, otpStr);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This user doesn't exists in our system, please try with valid user");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public PersonalInfoModel GetUserInfo()
        {
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);
            return new UserBusiness().GetPersonalInfo(user.Id);
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                        .Where(y => y.Count > 0)
                        .FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            }
            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, "1");
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        [AllowAnonymous]
        [Route("GetTenantsInfo")]
        [HttpGet]
        public HttpResponseMessage GetTenantsInfo(string domainName = "")
        {
            TenantModel obj = CommonManager.GetTenantConnection(string.Empty, string.Empty).Where(c => c.TenantDomain == domainName.ToLower().Trim()).FirstOrDefault();
            if (obj != null)
            {
                StoreModel objStoreInfo = new StoreBusiness().GetStore(obj.TenantConnection);
                objStoreInfo.UserId = obj.UserId;
                objStoreInfo.BannerList = new StoreBusiness().GetBannerImage(0, "home", obj.TenantConnection).Where(c => c.IsActive == true && c.IsDeleted == false).ToList();
                objStoreInfo.AdvImgList = new StoreBusiness().GetBannerImage(0, "advertisement", obj.TenantConnection).Where(c => c.IsActive == true && c.IsDeleted == false).ToList();
                //objStoreInfo.BannerList = new List<BannerImages>();
                return Request.CreateResponse(HttpStatusCode.OK, objStoreInfo);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new StoreModel());
        }

        [Route("UpdatePersonalInfo")]
        [HttpPost]
        public HttpResponseMessage UpdatePersonalInfo(PersonalInfoModel model)
        {
            try
            {
                int res = new UserBusiness().UpdatePersonalInfo(model);
                if (res == 1)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, 1);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This user doesn't exists in our system, please try with valid user");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage ForgotPassword(string email)
        {
            var user = UserManager.FindByName(email);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "-1");
            }
            var code = UserManager.GeneratePasswordResetTokenAsync(user.Id).Result.ToString();

            return Request.CreateResponse(HttpStatusCode.OK, string.Format("{0}_{1}", user.Id, code));

            // var callbackUrl = ConfigurationManager.AppSettings["ForgetPasswordLink"].ToString() + "?userId=" + user.Id + "&code=" + code;

            // string message = "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>";
            //string emailRes = EmailManager.SendForgetPasswordEmail(email, "admin@tawexpress.com", "ForgetPassword", message);
            //if (emailRes == "1")
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, "1");
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, emailRes);
            //}
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage ResetPassword(ResetPasswordModel model)
        {
            if (model.UserId == null || model.Code == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "-1");
            }
            //code = "ZBj13x3rVZrBF6vdTTWxqdrhFa5v73MJj8JRaxjBX/QbVyacgrPWiCs8fLDqXfJgXqO4AFlfld25hyTxk/959VQzPVnlI5kAu51nFfV1rN5XUrqmaMB/TxIieXWj378t/0xRD7GgbBB5NTWXaHCY67/5xdg5p3S3jrEnqmE+rgLWMOtr0cTps4ZO7JuDO+jraRKu/T2U3KL508xhHfE4bQ==";
            var result = UserManager.ResetPassword(model.UserId, model.Code, model.NewPassword);
            if (result.Succeeded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "1");
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, result.Errors.FirstOrDefault());
        }

        // POST api/Account/ChangePassword
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("UpdateDeviceToken")]
        [HttpPost]
        public HttpResponseMessage UpdateDeviceToken(string deviceToken)
        {
            int res = 0;
            if (!string.IsNullOrEmpty(deviceToken) && deviceToken != "null")
            {
                ApplicationUser result = UserManager.FindByName(User.Identity.Name);
                if (result != null)
                {
                    res = new RiderBusiness().UpdateDeviceToken(result.Email, deviceToken);
                }
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            else
            {
                res = -1;
                return Request.CreateResponse(HttpStatusCode.BadRequest, res);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("IsRiderOnline")]
        [HttpPost]
        public HttpResponseMessage IsRiderOnline()
        {
            bool res = false;
            ApplicationUser result = UserManager.FindByName(User.Identity.Name);
            if (result != null)
            {
                res = new RiderBusiness().IsRiderOnline(result.UserName);
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("UpdateRiderStatus")]
        [HttpPost]
        public HttpResponseMessage UpdateRiderStatus(bool status)
        {
            int res = 0;
            ApplicationUser result = UserManager.FindByName(User.Identity.Name);
            if (result != null)
            {
                res = new RiderBusiness().UpdateRiderOnlineStatus(result.Email, status);
            }
            if (res == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, status);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);
            }
        }

        [AllowAnonymous]
        [Route("RegiserAPP")]
        public async Task<HttpResponseMessage> RegiserAPP(RegisterAPPModel model)
        {
            try
            {
                bool IsExist = false;
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .FirstOrDefault();
                    var err = new
                    {
                        error = errors.FirstOrDefault().Exception,
                        error_description = errors.FirstOrDefault().ErrorMessage
                    };
                    return Request.CreateResponse(HttpStatusCode.BadRequest, err);
                }
                var user = new ApplicationUser();
                string createdByUserId = new UserBusiness().GetUsersByRoleName(RolesEnum.SuperAdmin.ToString()).Select(c => c.UserId).FirstOrDefault();
                user = new ApplicationUser() { FirstName = model.FirstName, LastName = model.LastName, UserName = model.MobileNumber, Email = string.Empty, PhoneNumber = model.MobileNumber, CreatedBy = createdByUserId };
                IdentityResult result = await UserManager.CreateAsync(user, ConfigurationManager.AppSettings["DefaultPassowrd"].ToString());
                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        foreach (string error in result.Errors)
                        {
                            if (error.Contains("already taken"))
                            {
                                user = await UserManager.FindByNameAsync(model.MobileNumber);
                                if (user.PhoneNumberConfirmed)
                                {
                                    return Request.CreateResponse(HttpStatusCode.Forbidden, "This user is already exist");
                                }
                                else
                                {
                                    IsExist = true;
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                            }
                        }
                    }
                }
                #region "Role section"
                if (!IsExist)
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                    if (!roleManager.RoleExists(RolesEnum.User.ToString()))
                    {
                        roleManager.Create(new IdentityRole { Name = RolesEnum.User.ToString() });
                    }
                    UserManager.AddToRole(user.Id, RolesEnum.User.ToString());
                }

                #endregion

                #region "sending OTP"              
                string otpStr = CommonManager.GenerateRandomNo().ToString();
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                {
                    if (SMSManager.SendSMSNotificationIND(model.MobileNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                    {
                        var res = await UserManager.FindByNameAsync(model.MobileNumber);
                        res.OTP = otpStr;
                        res.OTPCreatedDate = DateTime.UtcNow;
                        res.CreatedBy = createdByUserId;
                        await UserManager.UpdateAsync(res);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Your account has been created but an error occured while sending OTP, Please contact site administration.");
                    }
                }
                else
                {
                    var res = await UserManager.FindByNameAsync(model.MobileNumber);
                    res.OTP = otpStr;
                    res.OTPCreatedDate = DateTime.UtcNow;
                    res.CreatedBy = createdByUserId;
                    await UserManager.UpdateAsync(res);
                }
                #endregion
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, model);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GenerateOTPAPP")]
        [HttpPost]
        public async Task<HttpResponseMessage> GenerateOTPAPP(string mobileNumber, string tedencyCreatedBy = "")
        {
            try
            {
                var user = await UserManager.FindByNameAsync(mobileNumber);
                tedencyCreatedBy = !string.IsNullOrEmpty(tedencyCreatedBy) ? tedencyCreatedBy : new UserBusiness().GetUsersByRoleName(RolesEnum.SuperAdmin.ToString()).Select(c => c.UserId).FirstOrDefault();
                if (user != null && user.CreatedBy == tedencyCreatedBy)
                {
                    string otpStr = CommonManager.GenerateRandomNo().ToString();
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                    {
                        if (SMSManager.SendSMSNotificationIND(user.PhoneNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                        {
                            var res = await UserManager.FindByNameAsync(mobileNumber);
                            res.OTP = otpStr;
                            res.OTPCreatedDate = DateTime.UtcNow;
                            await UserManager.UpdateAsync(res);
                            return Request.CreateResponse(HttpStatusCode.OK, 1);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occured while generating OTP, please contact site administrator.");
                        }
                    }
                    else
                    {
                        var res = await UserManager.FindByNameAsync(mobileNumber);
                        res.OTP = otpStr;
                        res.OTPCreatedDate = DateTime.UtcNow;
                        await UserManager.UpdateAsync(res);
                        return Request.CreateResponse(HttpStatusCode.OK, otpStr);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This user doesn't exists in our system, please try with valid user");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }
        #region "Other Method"
        //// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        //[Route("ManageInfo")]
        //public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        //{
        //    IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

        //    foreach (IdentityUserLogin linkedAccount in user.Logins)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = linkedAccount.LoginProvider,
        //            ProviderKey = linkedAccount.ProviderKey
        //        });
        //    }

        //    if (user.PasswordHash != null)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = LocalLoginProvider,
        //            ProviderKey = user.UserName,
        //        });
        //    }

        //    return new ManageInfoViewModel
        //    {
        //        LocalLoginProvider = LocalLoginProvider,
        //        Email = user.UserName,
        //        Logins = logins,
        //        ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        //    };
        //}


        //// POST api/Account/AddExternalLogin
        //[Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //    {
        //        return BadRequest("External login failure.");
        //    }

        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// POST api/Account/RemoveLogin
        //[Route("RemoveLogin")]
        //public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result;

        //    if (model.LoginProvider == LocalLoginProvider)
        //    {
        //        result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
        //    }
        //    else
        //    {
        //        result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
        //            new UserLoginInfo(model.LoginProvider, model.ProviderKey));
        //    }

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

        //// GET api/Account/ExternalLogin
        ////[OverrideAuthentication]
        ////[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        ////[AllowAnonymous]
        ////[Route("ExternalLogin", Name = "ExternalLogin")]
        ////public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        ////{
        ////    if (error != null)
        ////    {
        ////        return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
        ////    }

        ////    if (!User.Identity.IsAuthenticated)
        ////    {
        ////        return new ChallengeResult(provider, this);
        ////    }

        ////    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

        ////    if (externalLogin == null)
        ////    {
        ////        return InternalServerError();
        ////    }

        ////    if (externalLogin.LoginProvider != provider)
        ////    {
        ////        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        ////        return new ChallengeResult(provider, this);
        ////    }

        ////    ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
        ////        externalLogin.ProviderKey));

        ////    bool hasRegistered = user != null;

        ////    if (hasRegistered)
        ////    {
        ////        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        ////        ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
        ////           OAuthDefaults.AuthenticationType);
        ////        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
        ////            CookieAuthenticationDefaults.AuthenticationType);
        ////        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        ////        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName, user.Id, roleManager.FindById(user.Roles.FirstOrDefault().RoleId).Name, user.FirstName, user.LastName, user.PhoneNumber, user.PhoneNumberConfirmed, user.Email, user.EmailConfirmed);
        ////        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        ////    }
        ////    else
        ////    {
        ////        IEnumerable<Claim> claims = externalLogin.GetClaims();
        ////        ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
        ////        Authentication.SignIn(identity);
        ////    }

        ////    return Ok();
        ////}

        //// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        //[AllowAnonymous]
        //[Route("ExternalLogins")]
        //public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        //{
        //    IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
        //    List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

        //    string state;

        //    if (generateState)
        //    {
        //        const int strengthInBits = 256;
        //        state = RandomOAuthStateGenerator.Generate(strengthInBits);
        //    }
        //    else
        //    {
        //        state = null;
        //    }

        //    foreach (AuthenticationDescription description in descriptions)
        //    {
        //        ExternalLoginViewModel login = new ExternalLoginViewModel
        //        {
        //            Name = description.Caption,
        //            Url = Url.Route("ExternalLogin", new
        //            {
        //                provider = description.AuthenticationType,
        //                response_type = "token",
        //                client_id = Startup.PublicClientId,
        //                redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
        //                state = state
        //            }),
        //            State = state
        //        };
        //        logins.Add(login);
        //    }

        //    return logins;
        //}



        //// POST api/Account/RegisterExternal
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        //[Route("RegisterExternal")]
        //public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var info = await Authentication.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return InternalServerError();
        //    }

        //    var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

        //    IdentityResult result = await UserManager.CreateAsync(user);
        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }
        //    return Ok();
        //}
        #endregion

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
