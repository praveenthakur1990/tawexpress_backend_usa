using SuperSmartShopping.BAL.Services;
using SuperSmartShopping.DAL.Common;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool isOnline { get; private set; }
        protected StoreModel storeInfo { get; private set; }
        protected TenantModel tenantsInfo { get; private set; }
        // protected SubscriptionInfoModel subscriptionInfo { get; private set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {         
            tenantsInfo = DAL.Common.CommonManager.GetTenantConnection(SessionManager.LoginResponse.UserId, string.Empty).ToList().FirstOrDefault();
            if (new BusinessHoursBusiness().GetBusinessHours(tenantsInfo.TenantConnection).Count() == 0)
            {
                new BusinessHoursBusiness().GetBusinessHours(tenantsInfo.TenantConnection);
            }
            storeInfo = new StoreBusiness().GetStore(tenantsInfo.TenantConnection);
            ViewBag.StoreName = storeInfo.Name;
            ViewBag.Address = storeInfo.Address;
            ViewBag.MobileNumber = storeInfo.Mobile;
            //subscriptionInfo = new SubscriptionBusiness().GetActiveSubscriptionInfo(tenantsInfo.TenantConnection);
            //if (subscriptionInfo == null)
            //{
            //    if (filterContext.RouteData.Values["controller"].ToString() == "Setting" && filterContext.RouteData.Values["action"].ToString() == "Subscription")
            //    {

            //    }
            //    else
            //    {
            //        filterContext.Result = new RedirectResult("/Admin/Setting/Subscription");
            //    }
            //}
            //ViewBag.IsOnline = storeInfo.IsOnline;
            ViewBag.LogoImg = storeInfo.LogoPath;
            ViewBag.CurrencySymbol = storeInfo.CurrencySymbol;
            ViewBag.CountryMobileCode = storeInfo.CountryCode == "IN" ? "+91" : storeInfo.CountryCode == "ES" ? "+34" : "+1";
            if (!string.IsNullOrEmpty(storeInfo.PickupAddresses))
            {
                ViewBag.PickUpAddress = new List<string>();
                var listOfSpeakers = storeInfo.PickupAddresses.Split('_').ToList();
                foreach (var speaker in listOfSpeakers)
                {
                    ViewBag.PickUpAddress.Add(speaker);
                }
            }

            //ViewBag.BannerImg = storeInfo.BannerImg == null || storeInfo.BannerImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/menu/img/restaurant-bg.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.BannerImg;

            //ViewBag.IsBannerImg = CommonManager.IsPhoto(restutantInfo.BannerImg == null || restutantInfo.BannerImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/menu/img/restaurant-bg.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.BannerImg);

            //ViewBag.PromotionalImg = restutantInfo.PromotionalImg == null || restutantInfo.PromotionalImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/menu/img/Offer-1.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.PromotionalImg;

            //ViewBag.IsPromotionalImg = CommonManager.IsPhoto(restutantInfo.PromotionalImg == null || restutantInfo.PromotionalImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/menu/img/Offer-1.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.PromotionalImg);

            //ViewBag.EmptyCartImg = restutantInfo.EmptyCartImg == null || restutantInfo.EmptyCartImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/menu/img/cart-image.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.EmptyCartImg;
            //ViewBag.OfflineImg = restutantInfo.OfflineImg == null || restutantInfo.OfflineImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/img/sorry-closed.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.OfflineImg;
            //ViewBag.MinOrderImg = restutantInfo.MinOrderImg == null || restutantInfo.MinOrderImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/img/addMoreItem.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.MinOrderImg;
            //ViewBag.LocationFarAwayImg = restutantInfo.LocationFarAwayImg == null || restutantInfo.LocationFarAwayImg == string.Empty ? ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + "/Content/img/outOfDeliveryArea.png" : ConfigurationManager.AppSettings["APPBaseUrl"].ToString() + restutantInfo.LocationFarAwayImg;

            //ViewBag.LabelText = restutantInfo.CustomLabel != null ? restutantInfo.CustomLabel.LabelText : string.Empty;
            //ViewBag.UrlText = restutantInfo.CustomLabel != null ? restutantInfo.CustomLabel.UrlText : string.Empty;
            //ViewBag.IsActive = restutantInfo.CustomLabel != null ? Convert.ToBoolean(restutantInfo.CustomLabel.IsActive) : false;
            ViewBag.DeliveryCharges = storeInfo.DeliveryCharges;
            ViewBag.Tax = storeInfo.TaxRate;
            ViewBag.CashOnDeliveryEnable = storeInfo.CashOnDeliveryEnable;
            //ViewBag.Subdomain=storeInfo.dom
            base.OnActionExecuting(filterContext);
        }
    }
}