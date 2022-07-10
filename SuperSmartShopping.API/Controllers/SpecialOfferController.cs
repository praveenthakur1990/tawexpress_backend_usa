using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/SpecialOffer")]
    public class SpecialOfferController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private ISpecialOfferBusiness _specialOfferBusiness;
        public SpecialOfferController(ISpecialOfferBusiness specialOfferBusiness)
        {
            _specialOfferBusiness = specialOfferBusiness;
        }

        [Route("AddUpdate")]
        [HttpPost]
        public HttpResponseMessage AddUpdate(SpecialOfferModel obj)
        {
            try
            {              
                string thumbnailFileName = string.Empty, thumbnailFullPath = string.Empty, thumbnailDirectoryPath = string.Empty, thumbnailFileUploadPath = string.Empty;          
                if (CommonManager.IsBase64(obj.BannerImagePath))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.BannerImagePath);
                    thumbnailFileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.SpecialOfferBannerImage.ToString() + "/";
                    thumbnailDirectoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + thumbnailFileUploadPath);
                    thumbnailFileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                    thumbnailFullPath = thumbnailDirectoryPath + thumbnailFileName;
                    File.WriteAllBytes(thumbnailFullPath, imageBytes);
                    obj.BannerImagePath = "/" + thumbnailFileUploadPath + thumbnailFileName;
                }
                else if (!string.IsNullOrEmpty(obj.BannerImagePath))
                {
                    obj.BannerImagePath = obj.BannerImagePath.Substring(obj.BannerImagePath.IndexOf("/Upload"));
                }

                int res = _specialOfferBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GetSpecialOffer")]
        [HttpGet]
        public HttpResponseMessage GetWeeklyCircular(int id, string userId, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<SpecialOfferModel> objList = _specialOfferBusiness.Get(id, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductBySpecialOfferId")]
        [HttpGet]
        public HttpResponseMessage GetProductBySpecialOfferId(string categoryId, string userId, int specialOfferId = 0)
        {
            List<ProductDashboardModel> objList = _specialOfferBusiness.GetProductBySpecialOfferId(categoryId, specialOfferId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
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

                int res = _specialOfferBusiness.AddUpdateProduct(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty).FirstOrDefault().TenantConnection);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [Route("GetSpecialOfferDates")]
        [HttpGet]
        public HttpResponseMessage GetWeeklyCircularDates(string userId)
        {
            List<SpecialOfferDatesModel> objList = _specialOfferBusiness.GetSpecialOfferDates(CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductCatSpecialOfferId")]
        [HttpGet]
        public HttpResponseMessage GetProductCatSpecialOfferId(string userId, int specialOfferId = 0)
        {
            List<ProductDashboardModel> objList = _specialOfferBusiness.GetProductBySpecialOfferId(string.Empty, specialOfferId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);

            List<SpecialOfferProductCatModel> objCatList = objList.Select(c => new SpecialOfferProductCatModel
            {
                Id = c.CategoryId,
                Name = c.CategoryName
            }).GroupBy(c => c.Id).Select(y => y.First()).OrderBy(c => c.Name).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objCatList);
        }

    }
}
