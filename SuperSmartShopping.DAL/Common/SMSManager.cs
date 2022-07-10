using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Notify.V1.Service;
using NotificationResource = Twilio.Rest.Notify.V1.Service.NotificationResource;

namespace SuperSmartShopping.DAL.Common
{
    public static class SMSManager
    {
        public static bool SendSMSNotification(string toNumber, string message)
        {
            try
            {
                toNumber = CommonManager.RemoveSpecialCharacters(toNumber);
                toNumber = ConfigurationManager.AppSettings["CountryCode"].ToString() + toNumber;
                string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].ToString();
                string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
                TwilioClient.Init(accountSid, authToken);
                var response = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["TwilioPhoneNumber"].ToString()),
                    to: new Twilio.Types.PhoneNumber(toNumber)
                );
                return true;
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }

        public static bool SendSMSNotificationIND(string toNumber, string message)
        {
            try
            {
                toNumber = CommonManager.RemoveSpecialCharacters(toNumber);
                toNumber = ConfigurationManager.AppSettings["INDCountryCode"].ToString() + toNumber;
                string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].ToString();
                string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
                TwilioClient.Init(accountSid, authToken);
                var response = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["TwilioPhoneNumber"].ToString()),
                    to: new Twilio.Types.PhoneNumber(toNumber)
                );
                return true;
            }
            catch (Exception ex)
            {
                CommonManager.LogError(MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }

        public static Dictionary<string, string> SendBulkSMSNotification(Dictionary<string, string> toNumber, string message, string userId)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            try
            {
                Parallel.ForEach(toNumber, item =>
                {
                    string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].ToString();
                    string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
                    TwilioClient.Init(accountSid, authToken);

                    string phoneNumber = string.Empty;
                    phoneNumber = CommonManager.RemoveSpecialCharacters(item.Key.ToString());
                    phoneNumber = ConfigurationManager.AppSettings["CountryCode"].ToString() + phoneNumber;
                    var response = MessageResource.Create(
                            body: message,
                            from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["TwilioPhoneNumber"].ToString()),
                            to: new Twilio.Types.PhoneNumber(phoneNumber),
                            statusCallback: new Uri(ConfigurationManager.AppSettings["MessageStatusCalbackUrl"].ToString() + userId)
                );
                    obj.Add(response.Sid, response.To);
                });
                //foreach (var item in toNumber)
                //{

                //}
            }
            catch (Exception ex)
            {
                obj.Add("Error", ex.Message.ToString());
            }
            return obj;
        }

        public static Dictionary<string, string> Bulktesting(Dictionary<string, string> toNumber, string message, string userId)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            try
            {

                string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"].ToString();
                string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
                TwilioClient.Init(accountSid, authToken);

                List<string> _toNumbers = toNumber.Values.ToList();

                var notification = NotificationResource.Create(
                        accountSid,
                        identity: _toNumbers,
                        body: message,
                        deliveryCallbackUrl: ConfigurationManager.AppSettings["MessageStatusCalbackUrl"].ToString() + userId
                        );
            }
            catch (Exception ex)
            {
                obj.Add("Error", ex.Message.ToString());
            }
            return obj;
        }

        public static void SendPushNotificationMessage(string deviceToken, string title, string message)
        {
            string serverKey = ConfigurationManager.AppSettings["FCMSeverKey"].ToString();
            var notificationInputDto = new
            {
                to = deviceToken,
                notification = new
                {
                    body = message,
                    title = title,
                    icon = "",
                    type = ""
                },
                data = new
                {
                    key1 = "value1",
                    key2 = "value2"
                }
            };
            try
            {
                var result = "";
                var webAddr = "https://fcm.googleapis.com/fcm/send";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(notificationInputDto));
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
