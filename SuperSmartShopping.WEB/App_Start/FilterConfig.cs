using SuperSmartShopping.DAL.Common;
using System;
using System.Web;
using System.Web.Mvc;

namespace SuperSmartShopping.WEB
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var _encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(_encodingsAccepted)) return;

            _encodingsAccepted = _encodingsAccepted.ToLowerInvariant();
            var _response = filterContext.HttpContext.Response;

            if (_encodingsAccepted.Contains("deflate"))
            {
                _response.AppendHeader("Content-encoding", "deflate");
                _response.Filter = new System.IO.Compression.DeflateStream(_response.Filter, System.IO.Compression.CompressionMode.Compress);
            }
            else if (_encodingsAccepted.Contains("gzip"))
            {
                _response.AppendHeader("Content-encoding", "gzip");
                _response.Filter = new System.IO.Compression.GZipStream(_response.Filter, System.IO.Compression.CompressionMode.Compress);
            }
        }
    }
    public class SuperAdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (SessionManager.LoginResponse != null && (SessionManager.LoginResponse.RoleName.ToLower() == DAL.Enums.EnumHelper.RolesEnum.SuperAdmin.ToString().ToLower()))
            {
                if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                {
                    AccessTokenHelper.GenerateAccessToken();
                    if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                    {
                        return authorize;
                    }
                }
                authorize = true;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }
    }
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (SessionManager.LoginResponse != null && SessionManager.LoginResponse.RoleName.ToLower() == DAL.Enums.EnumHelper.RolesEnum.Admin.ToString().ToLower())
            {
                if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                {
                    AccessTokenHelper.GenerateAccessToken();
                    if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                    {
                        return authorize;
                    }
                }
                authorize = true;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }
    }
    public class SuperAdminOnlyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (SessionManager.LoginResponse != null && (SessionManager.LoginResponse.RoleName.ToLower() == DAL.Enums.EnumHelper.RolesEnum.SuperAdmin.ToString().ToLower()))
            {
                if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                {
                    AccessTokenHelper.GenerateAccessToken();
                    if (DateTime.Now > Convert.ToDateTime(SessionManager.LoginResponse.Expires))
                    {
                        return authorize;
                    }
                }
                authorize = true;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result = new RedirectResult("/Account/Login");
            }
        }
    }
}
