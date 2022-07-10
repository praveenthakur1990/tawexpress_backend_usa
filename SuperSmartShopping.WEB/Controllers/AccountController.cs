using Newtonsoft.Json;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.WEB.Controllers
{
    public class AccountController : Controller
    {
        readonly string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            var kvpList = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("UserName", frm["email"].ToString()),
            new KeyValuePair<string, string>("Password", frm["password"].ToString()),
            new KeyValuePair<string, string>("grant_type", "password")
            };
            FormUrlEncodedContent rqstBody = new FormUrlEncodedContent(kvpList);
            using (var client = new HttpClient())
            {
                string url = apiBaseUrl + MethodEnum.Login.GetDescription().ToString();
                HttpResponseMessage messge = client.PostAsync(url, rqstBody).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(result);
                    SessionManager.LoginResponse = response;
                    return Content(response.RoleName.ToLower());
                }
                else
                {
                    Errorresponse response = JsonConvert.DeserializeObject<Errorresponse>(result);
                    return Content(response.error_description.ToString());
                }
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + SessionManager.LoginResponse.AccessToken);
                string url = apiBaseUrl + MethodEnum.Logout.GetDescription().ToString();
                HttpResponseMessage messge = client.PostAsync(url, null).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                SessionManager.LoginResponse = null;
                return Content("1");
            }
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ForgetPassword(string email)
        {
            using (var client = new HttpClient())
            {
                string res = string.Empty;
                string url = apiBaseUrl + MethodEnum.ForgetPassword.GetDescription().ToString() + "?email=" + email;
                HttpResponseMessage messge = client.PostAsync(url, null).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<string>(result);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = result.Split('_')[0], code = result.Split('_')[1] }, protocol: Request.Url.Scheme);
                    string message = "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>";
                    string emailRes = EmailManager.SendForgetPasswordEmail(email, "admin@tawexpress.com", "ForgetPassword", message);
                    res = emailRes;
                }
                else
                {
                    res = JsonConvert.DeserializeObject<string>(result);
                }
                return Content(res);
            }
        }

        public ActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                ViewBag.Error = "Invalid code";
            }
            else
            {
                ViewBag.UserId = userId;
                ViewBag.Code = code;
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPassword(string userId, string code, string newPassword)
        {
            using (var client = new HttpClient())
            {
                string res = string.Empty;
                string url = apiBaseUrl + MethodEnum.ResetPassword.GetDescription().ToString();
                ResetPasswordModel obj = new ResetPasswordModel
                {
                    Code = code,
                    UserId = userId,
                    NewPassword = newPassword                    
                };
                var httpContent = CommonManager.CreateHttpContent(obj);
                HttpResponseMessage messge = client.PostAsync(url, httpContent).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    res = "1";
                }
                else
                {
                    res = JsonConvert.DeserializeObject<string>(result);
                }
                return Content(res);
            }
        }
    }
}