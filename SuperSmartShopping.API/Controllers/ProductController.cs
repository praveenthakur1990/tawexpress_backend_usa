using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.Models;
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
    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private IProductBusiness _productBusiness;
        public ProductController(IProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
        }

        [Route("AddUpdate")]
        [HttpPost]
        public HttpResponseMessage AddUpdate([FromBody] Product obj)
        {
            try
            {
                string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                if (CommonManager.IsBase64(obj.Image))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.Image);
                    fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.ProductImage.ToString() + "/";
                    directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                    fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                    fullPath = directoryPath + fileName;
                    File.WriteAllBytes(fullPath, imageBytes);
                    obj.Image = "/" + fileUploadPath + fileName;
                }
                else if (!string.IsNullOrEmpty(obj.Image))
                {
                    obj.Image = obj.Image.Substring(obj.Image.IndexOf("/Upload"));
                }
                int res = _productBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty)[0].TenantConnection);
                if (res > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex, obj);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        [Route("GetProducts")]
        [HttpGet]
        public HttpResponseMessage GetProducts(string userId, int productId = 0, int pageNumber = 1, int pageSize = 20, string searchStr = "")
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<ProductModel> objList = _productBusiness.GetProducts(productId, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [Route("MarkeAsDelete")]
        [HttpPost]
        public HttpResponseMessage MarkeAsDelete(int productId, string userId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _productBusiness.MarkProductAsDeleted(productId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection));
        }

        [AllowAnonymous]
        [Route("GetProductsForDashboard")]
        [HttpGet]
        public HttpResponseMessage GetProductsForDahboard(int categoryId, int limit, string userId)
        {
            List<ProductDashboardModel> objList = _productBusiness.GetProductForDashboard(categoryId, limit, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductByCategoryIdAPP")]
        [HttpGet]
        public HttpResponseMessage GetProductByCategoryIdAPP(string categoryId, string subCategoryIds, string brandIds, string userId)
        {
            List<ProductDashboardModel> objList = _productBusiness.GetProductByCategoryIdAPP(categoryId, subCategoryIds, brandIds, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductBySearch")]
        [HttpGet]
        public HttpResponseMessage GetProductBySearch(string searchStr, string userId)
        {
            List<ProductModel> objList = _productBusiness.GetProducts(0, new PaginationModel
            {
                PageNumber = 1,
                PageSize = -1,
                SearchStr = searchStr
            }, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList.OrderBy(c => c.Name).ToList());
        }

        [Route("GetTagsList")]
        [HttpGet]
        public HttpResponseMessage GetTagsList(int productId, string userId)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = 1;
            obj.PageSize = -1;
            obj.SearchStr = string.Empty;
            List<ProductModel> objList = _productBusiness.GetProducts(0, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).Where(c => c.Id != productId && c.IsCategoryActive == true && c.IsPublished == true).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetRelatedProductsById")]
        [HttpGet]
        public HttpResponseMessage GetRelatedProductsById(int productId, int limit, string userId)
        {
            List<ProductDashboardModel> objList = _productBusiness.GetRelatedProductByIdAPP(productId, limit, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetProductsForApp")]
        [HttpGet]
        public HttpResponseMessage GetProductsForApp(string userId, int productId = 0, int pageNumber = 1, int pageSize = 20, string searchStr = "")
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<ProductModel> objList = _productBusiness.GetProducts(productId, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }
    }
}
