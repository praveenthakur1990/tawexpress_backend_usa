using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/WeeklyCircular")]
    public class WeeklyCircularController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private IWeeklyCircularBusiness _weeklyCircularBusiness;
        public WeeklyCircularController(IWeeklyCircularBusiness weeklyCircularBusiness)
        {
            _weeklyCircularBusiness = weeklyCircularBusiness;
        }

        [Route("AddUpdate")]
        [HttpPost]
        public HttpResponseMessage AddUpdate(WeeklyCircularModel obj)
        {
            try
            {
                string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                string thumbnailFileName = string.Empty, thumbnailFullPath = string.Empty, thumbnailDirectoryPath = string.Empty, thumbnailFileUploadPath = string.Empty;
                if (CommonManager.IsBase64(obj.PdfFilePath))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.PdfFilePath);
                    fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.WeeklyCircularPdf.ToString() + "/";
                    directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                    fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PdfExtension.GetDescription().ToString());
                    fullPath = directoryPath + fileName;
                    File.WriteAllBytes(fullPath, imageBytes);
                    obj.PdfFilePath = "/" + fileUploadPath + fileName;
                }
                else if (!string.IsNullOrEmpty(obj.PdfFilePath))
                {
                    obj.PdfFilePath = obj.PdfFilePath.Substring(obj.PdfFilePath.IndexOf("/Upload"));
                }


                if (CommonManager.IsBase64(obj.ThumbnailImgPath))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.ThumbnailImgPath);
                    thumbnailFileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.WeeklyCircularImg.ToString() + "/";
                    thumbnailDirectoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + thumbnailFileUploadPath);
                    thumbnailFileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                    thumbnailFullPath = thumbnailDirectoryPath + thumbnailFileName;
                    File.WriteAllBytes(thumbnailFullPath, imageBytes);
                    obj.ThumbnailImgPath = "/" + thumbnailFileUploadPath + thumbnailFileName;
                }
                else if (!string.IsNullOrEmpty(obj.ThumbnailImgPath))
                {
                    obj.ThumbnailImgPath = obj.ThumbnailImgPath.Substring(obj.ThumbnailImgPath.IndexOf("/Upload"));
                }

                int res = _weeklyCircularBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GetWeeklyCircular")]
        [HttpGet]
        public HttpResponseMessage GetWeeklyCircular(int id, string userId, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<WeeklyCircularModel> objList = _weeklyCircularBusiness.Get(id, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("AddUpdateProduct")]
        [HttpPost]
        public HttpResponseMessage AddUpdateProduct(WeeklyCircularCatInfoModel obj)
        {
            try
            {
                DataTable table = CommonManager.ToDataTable(obj.ProductList.Select(c => new { ProductId = c.ProductId, OfferType = c.OfferType, OfferValue = c.OfferValue }).ToList());
                obj.ProductListDataTable = table;

                int res = _weeklyCircularBusiness.AddUpdateProduct(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GetProductByWeeklyCircularId")]
        [HttpGet]
        public HttpResponseMessage GetProductByWeeklyCircularId(string categoryId, string userId, int weeklyCircularId = 0)
        {
            List<ProductDashboardModel> objList = _weeklyCircularBusiness.GetProductByWeeklyCircularId(categoryId, weeklyCircularId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductCatWeeklyCircularId")]
        [HttpGet]
        public HttpResponseMessage GetProductCatWeeklyCircularId(string userId, int weeklyCircularId = 0)
        {
            List<ProductDashboardModel> objList = _weeklyCircularBusiness.GetProductByWeeklyCircularId(string.Empty, weeklyCircularId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            List<WeeklyCircularProductCatModel> objCatList = objList.Select(c => new WeeklyCircularProductCatModel
            {
                Id = c.CategoryId,
                Name = c.CategoryName
            }).GroupBy(c => c.Id).Select(y => y.First()).OrderBy(c => c.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objCatList);
        }

        [AllowAnonymous]
        [Route("GetWeeklyCircularDates")]
        [HttpGet]
        public HttpResponseMessage GetWeeklyCircularDates(string userId)
        {
            List<WeeklyCircularDatesModel> objList = _weeklyCircularBusiness.GetWeeklyCircularDates(CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("SaveWeeklyCircularSubscriber")]
        [HttpPost]
        public HttpResponseMessage SaveWeeklyCircularSubscriber(WeeklyCircularSubscriberModel obj)
        {
            try
            {
                int res = _weeklyCircularBusiness.AddUpdateSubscriber(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                if (res == 1)
                {
                    string otpStr = CommonManager.GenerateRandomNo().ToString();
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                    {
                        if (SMSManager.SendSMSNotification(obj.MobileNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                        {
                            obj.Otp = otpStr;
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occured while generating OTP, please contact site administrator.");
                        }
                    }
                    else
                    {
                        obj.Otp = otpStr;
                    }

                    _weeklyCircularBusiness.UpdateSubscriberOTP(obj.MobileNumber, otpStr, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                }

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("VerifyWeeklyCircularSubscriber")]
        [HttpPost]
        public HttpResponseMessage VerifyOTP(string OTP, string mobileNumber, string userId)
        {
            try
            {
                var subscriber = _weeklyCircularBusiness.GetWeeklyCircularSubscriber(mobileNumber, new PaginationModel { PageNumber = 1, PageSize = -1, SearchStr = string.Empty }, CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection).FirstOrDefault();
                if (subscriber != null)
                {
                    if (subscriber.Otp == OTP.Trim())
                    {
                        TimeSpan span = DateTime.UtcNow.Subtract(subscriber.OtpGenerateDate.Value);
                        if (span.TotalMinutes <= Convert.ToInt32(ConfigurationManager.AppSettings["MaxOTPValidateTimeInMin"].ToString()))
                        {
                            int res = _weeklyCircularBusiness.AddUpdateSubscriber(subscriber, CommonManager.GetTenantConnection(subscriber.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                            return Request.CreateResponse(HttpStatusCode.OK, res);
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
        [Route("ValidateSubscriber")]
        [HttpPost]
        public HttpResponseMessage ValidateSubscriber(string mobileNumber, string userId)
        {
            try
            {
                var subscriber = _weeklyCircularBusiness.GetWeeklyCircularSubscriber(mobileNumber, new PaginationModel { PageNumber = 1, PageSize = -1, SearchStr = string.Empty }, CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection).FirstOrDefault();

                if (subscriber != null)
                {
                    string otpStr = CommonManager.GenerateRandomNo().ToString();
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendOtp"].ToString()) == true)
                    {
                        if (SMSManager.SendSMSNotification(mobileNumber, ConfigurationManager.AppSettings["OTPSendMessage"].ToString().Replace("{0}", otpStr)))
                        {

                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "An error occured while generating OTP, please contact site administrator.");
                        }
                    }
                    _weeklyCircularBusiness.UpdateSubscriberOTP(mobileNumber, otpStr, CommonManager.GetTenantConnection(userId, string.Empty).FirstOrDefault().TenantConnection);
                    return Request.CreateResponse(HttpStatusCode.OK, 1);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetWeeklyCircularSubscribers")]
        [HttpGet]
        public HttpResponseMessage GetWeeklyCircularSubscribers(string userId, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<WeeklyCircularSubscriberModel> objList = _weeklyCircularBusiness.GetWeeklyCircularSubscriber(string.Empty, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }
    }
}
