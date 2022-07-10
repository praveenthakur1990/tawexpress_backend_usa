using Newtonsoft.Json;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Store")]
    public class StoreController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private IStoreBusiness _storeBusiness;
        public StoreController(IStoreBusiness storeBusiness)
        {
            _storeBusiness = storeBusiness;
        }
        [Route("AddUpdateStore")]
        [HttpPost]
        public HttpResponseMessage AddUpdateStore(StoreModel obj)
        {
            string emailAddress = string.Empty, subdomain = string.Empty;
            if (obj.Id == 0)
            {
                emailAddress = obj.Email.Split('_')[0].ToString();
                subdomain = obj.Email.Split('_')[1].ToString();
            }
            else
            {
                emailAddress = obj.Email;
                subdomain = string.Empty;
            }
            try
            {
                var kvpList = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("Email", emailAddress),
            new KeyValuePair<string, string>("Password", "@Password1"),
            new KeyValuePair<string, string>("ConfirmPassword", "@Password1"),
            new KeyValuePair<string, string>("RoleName", RolesEnum.Admin.ToString()),
            new KeyValuePair<string, string>("SubDomainName",subdomain.Replace(" ", "").ToLower()),
            new KeyValuePair<string, string>("CreatedBy",obj.CreatedBy),
            new KeyValuePair<string, string>("MobileNumber",obj.Mobile),
            new KeyValuePair<string, string>("FirstName",obj.ContactPersonName!=null ? obj.ContactPersonName.Split(' ')[0]:string.Empty),
            new KeyValuePair<string, string>("LastName",obj.ContactPersonName!=null && obj.ContactPersonName.Split(' ').Length > 1 ? obj.ContactPersonName.Split(' ')[1]:string.Empty)

            };
                FormUrlEncodedContent rqstBody = new FormUrlEncodedContent(kvpList);
                using (var client = new HttpClient())
                {
                    HttpResponseMessage messge = null;
                    string result = string.Empty;
                    string url = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString() + MethodEnum.Register.GetDescription().ToString();
                    if (obj.Id == 0)
                    {
                        messge = client.PostAsync(url, rqstBody).Result;
                        result = messge.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        messge = new HttpResponseMessage();
                        messge = Request.CreateResponse(HttpStatusCode.OK);
                    }

                    if (messge.IsSuccessStatusCode)
                    {
                        #region "Saving uploaded document files"
                        string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                        if (!string.IsNullOrEmpty(obj.GSTFilePath) && CommonManager.IsBase64(obj.GSTFilePath) == true)
                        {
                            byte[] imageBytes = Convert.FromBase64String(obj.GSTFilePath);
                            fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + emailAddress + "/" + DirectoryPathEnum.StoreDoc.ToString() + "/" + DirectoryPathEnum.GSTDOc.ToString() + "/";
                            directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                            string[] files = Directory.GetFiles(directoryPath);
                            foreach (string file in files)
                            {
                                File.Delete(file);
                            }
                            fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PdfExtension.GetDescription().ToString());
                            fullPath = directoryPath + fileName;
                            File.WriteAllBytes(fullPath, imageBytes);
                            obj.GSTFilePath = "/" + fileUploadPath + fileName;
                        }


                        if (!string.IsNullOrEmpty(obj.LogoPath) && CommonManager.IsBase64(obj.LogoPath) == true)
                        {
                            byte[] imageBytes = Convert.FromBase64String(obj.LogoPath);
                            fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + emailAddress + "/" + DirectoryPathEnum.Logo.ToString() + "/";
                            directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                            string[] files = Directory.GetFiles(directoryPath);
                            foreach (string file in files)
                            {
                                File.Delete(file);
                            }
                            fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                            fullPath = directoryPath + fileName;
                            File.WriteAllBytes(fullPath, imageBytes);
                            obj.LogoPath = "/" + fileUploadPath + fileName;
                        }
                        #endregion

                        string connectionStr = CommonManager.GetTenantConnection(string.Empty, emailAddress)[0].TenantConnection;
                        obj.Email = emailAddress;
                        _storeBusiness.AddUpdate(obj, connectionStr);
                        return Request.CreateResponse(HttpStatusCode.OK, "success");
                    }
                    else
                    {
                        Errorresponse response = JsonConvert.DeserializeObject<Errorresponse>(result);
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, response == null ? "An error occured while registering the store, please try again later." : response.error_description);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, obj);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetStore")]
        [HttpGet]
        public HttpResponseMessage GetStore(string userId)
        {
            StoreModel obj = _storeBusiness.GetStore(CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }


        [AllowAnonymous]
        [Route("GetStoreApp")]
        [HttpGet]
        public HttpResponseMessage GetStoreApp(string userId)
        {
            StoreModel obj = _storeBusiness.GetStore(CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }


    }
}
