using System.Web.Mvc;

namespace SuperSmartShopping.WEB.Areas.SecurePanel
{
    public class SecurePanelAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SecurePanel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SecurePanel_default",
                "SecurePanel/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}