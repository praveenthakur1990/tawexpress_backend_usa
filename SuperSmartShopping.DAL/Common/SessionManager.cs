using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SuperSmartShopping.DAL.Common
{
    public static class SessionManager
    {
        public static LoginResponse LoginResponse
        {
            get
            {
                if (HttpContext.Current.Session["LoginResponse"] != null)
                {
                    return (LoginResponse)HttpContext.Current.Session["LoginResponse"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["LoginResponse"] = value;
            }
        }

        public static string OTPNumber
        {
            get
            {
                if (HttpContext.Current.Session["OTPNumber"] != null)
                {
                    return HttpContext.Current.Session["OTPNumber"].ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session["OTPNumber"] = value;
            }
        }

    }
}
