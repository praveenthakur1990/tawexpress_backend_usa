using SuperSmartShopping.BAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Api.V2010.Account;

namespace SuperSmartShopping.WEB.Areas.Admin.Controllers
{
    public class MessageStatusController : Controller
    {
        readonly IMessageResourcesBusiness _messageResourcesBusiness;
        public MessageStatusController(IMessageResourcesBusiness messageResourcesBusiness)
        {
            _messageResourcesBusiness = messageResourcesBusiness;
        }
        [HttpPost]
        public ActionResult Index(string id)
        {
            // Log the message id and status
            var smsSid = Request.Form["SmsSid"];
            var messageStatus = Request.Form["MessageStatus"];
            var logMessage = $"\"{smsSid}\", \"{messageStatus}\"";
            MessageResource objMsgResourceResponse = MessageResource.Fetch(pathSid: smsSid);
            _messageResourcesBusiness.AddMessageResources(objMsgResourceResponse, id);
            Trace.WriteLine(logMessage);
            return Content("");
        }
    }
}