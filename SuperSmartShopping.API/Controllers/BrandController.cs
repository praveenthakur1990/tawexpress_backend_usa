using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SuperSmartShopping.API.Models;
using SuperSmartShopping.BAL.Interfaces;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace SuperSmartShopping.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Brand")]
    public class BrandController : ApiController
    {
        private IBrandBusiness _brandBusiness;
        public BrandController(IBrandBusiness brandBusiness)
        {
            _brandBusiness = brandBusiness;
        }

        [Route("AddUpdateBrand")]
        [HttpPost]
        public HttpResponseMessage AddUpdateBrand([FromBody] BrandModel obj)
        {
            try
            {
                int res = _brandBusiness.AddUpdate(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty)[0].TenantConnection);
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

        [Route("GetBrands")]
        [HttpGet]
        public HttpResponseMessage GetBrands(int brandId, string userId, int pageNumber, int pageSize, string searchStr)
        {            
            PaginationModel obj = new PaginationModel();
            obj.PageNumber = pageNumber;
            obj.PageSize = pageSize;
            obj.SearchStr = searchStr;
            List<BrandModel> objList = _brandBusiness.GetBrands(brandId, obj, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetBrandsForAPP")]
        [HttpGet]
        public HttpResponseMessage GetBrandsForAPP(int categoryId, string subCategoryIds, string userId)
        {
            List<BrandModel> objList = _brandBusiness.GetBrandsForAPP(categoryId, !string.IsNullOrEmpty(subCategoryIds) ? subCategoryIds : string.Empty, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection);
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }



    }
}
