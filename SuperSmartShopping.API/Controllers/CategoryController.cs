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
    [RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        readonly string appPhysicalPath = ConfigurationManager.AppSettings["BackendPhysicalPath"].ToString();
        private ICategoryBusiness _categoryBusiness;
        public CategoryController(ICategoryBusiness categoryBusiness)
        {
            _categoryBusiness = categoryBusiness;
        }

        [Route("AddUpdateCategory")]
        [HttpPost]
        public HttpResponseMessage AddUpdateCategory([FromBody] Category obj)
        {
            try
            {
                string fileName = string.Empty, fullPath = string.Empty, directoryPath = string.Empty, fileUploadPath = string.Empty;
                if (CommonManager.IsBase64(obj.ImagePath))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.ImagePath);
                    fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.CategoryImage.ToString() + "/";
                    directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                    fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                    fullPath = directoryPath + fileName;
                    File.WriteAllBytes(fullPath, imageBytes);
                    obj.ImagePath = "/" + fileUploadPath + fileName;
                }
                else if (!string.IsNullOrEmpty(obj.ImagePath))
                {
                    obj.ImagePath = obj.ImagePath.Substring(obj.ImagePath.IndexOf("/Upload"));
                }

                if (CommonManager.IsBase64(obj.BannerImg))
                {
                    byte[] imageBytes = Convert.FromBase64String(obj.BannerImg);
                    fileUploadPath = DirectoryPathEnum.Upload.ToString() + "/" + obj.CreatedBy + "/" + DirectoryPathEnum.CategoryBannerImage.ToString() + "/";
                    directoryPath = CommonManager.CreatePathIfMissing(appPhysicalPath + "/" + fileUploadPath);
                    fileName = CommonManager.CreateUniqueFileName(ExtensionEnum.PNGExtension.GetDescription().ToString());
                    fullPath = directoryPath + fileName;
                    File.WriteAllBytes(fullPath, imageBytes);
                    obj.BannerImg = "/" + fileUploadPath + fileName;
                }
                else if (!string.IsNullOrEmpty(obj.BannerImg))
                {
                    obj.BannerImg = obj.BannerImg.Substring(obj.BannerImg.IndexOf("/Upload"));
                }

                int res = _categoryBusiness.AddUpdateCategory(obj, CommonManager.GetTenantConnection(obj.CreatedBy, string.Empty)[0].TenantConnection);
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

        [AllowAnonymous]
        [Route("GetCategories")]
        [HttpGet]
        public HttpResponseMessage GetCategories(string type, string userId, string callingBy = "")
        {
            List<Category> objList = new List<Category>();
            if (!string.IsNullOrEmpty(callingBy))
            {
                objList = _categoryBusiness.GetCategories(type, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).ToList();
            }
            else
            {
                objList = _categoryBusiness.GetCategories(type, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).Where(c => c.ParentId == 0).ToList();
            }
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }


        [Route("GetCategoryById")]
        [HttpGet]
        public HttpResponseMessage GetCategoryById(int categoryId, string userId)
        {
            Category obj = _categoryBusiness.GetCategories("All", CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).Where(c => c.Id == categoryId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }


        [Route("MarkeAsDelete")]
        [HttpPost]
        public HttpResponseMessage MarkeAsDelete(int categoryId, string userId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _categoryBusiness.MarkCategoryAsDeleted(categoryId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection));
        }

       
        [Route("GetSubCategories")]
        [HttpGet]
        public HttpResponseMessage GetSubCategories(string type, string userId)
        {
            List<Category> objList = _categoryBusiness.GetCategories(type, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).Where(c => c.ParentId > 0).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }

        [AllowAnonymous]
        [Route("GetSubCategoriesApp")]
        [HttpGet]
        public HttpResponseMessage GetSubCategoriesApp(int categoryId, string userId)
        {
            List<CategoryModel> objList = _categoryBusiness.GetSubCategoriesByCatId(categoryId, CommonManager.GetTenantConnection(userId, string.Empty)[0].TenantConnection).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, objList);
        }
    }
}
