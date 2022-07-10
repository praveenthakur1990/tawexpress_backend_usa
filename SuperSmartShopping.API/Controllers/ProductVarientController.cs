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
    [RoutePrefix("api/ProductVarient")]
    public class ProductVarientController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private IProductVarientBusiness _productVarientBusiness;
        public ProductVarientController(IProductVarientBusiness productVarientBusiness)
        {
            _productVarientBusiness = productVarientBusiness;
        }

        [Route("AddUpdate")]
        [HttpPost]
        public HttpResponseMessage AddUpdate([FromBody] ProductVarientsModel obj)
        {
            try
            {
                string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                if (CommonManager.IsBase64(obj.Image))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.Image);
                    fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + "/" + obj.ProductId + "/" + DirectoryPathEnum.ProductVarientImage.ToString() + "/";
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
                int res = _productVarientBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty)[0].TenantConnection);
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

        [Route("GetProductVarients")]
        [HttpGet]
        public HttpResponseMessage GetProducts(int id, int productId, string userId, int pageNumber, int pageSize, string searchStr)
        {
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<ProductVarientsModel> objList = _productVarientBusiness.GetProductsVarients(id, productId, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

    }
}
