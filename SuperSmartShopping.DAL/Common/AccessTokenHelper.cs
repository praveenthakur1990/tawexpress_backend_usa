using Newtonsoft.Json;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static SuperSmartShopping.DAL.Enums.EnumHelper;

namespace SuperSmartShopping.DAL.Common
{
    public static class AccessTokenHelper
    {
        readonly static string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public static void GenerateAccessToken()
        {
            var kvpList = new List<KeyValuePair<string, string>>
            {
            new KeyValuePair<string, string>("refresh_token", SessionManager.LoginResponse.RefreshToken),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
            };
            FormUrlEncodedContent rqstBody = new FormUrlEncodedContent(kvpList);
            using (var client = new HttpClient())
            {
                string url = apiBaseUrl + MethodEnum.Login.GetDescription().ToString();
                HttpResponseMessage messge = client.PostAsync(url, rqstBody).Result;
                string result = messge.Content.ReadAsStringAsync().Result;
                if (messge.IsSuccessStatusCode)
                {
                    LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(result);
                    SessionManager.LoginResponse = response;
                }
            }
        }
    }
}
