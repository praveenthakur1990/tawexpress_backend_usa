using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IMessageResourcesBusiness
    {
        int AddMessageResources(MessageResource model, string userId);
        List<MessageResourcesViewModel> GetMessagesLogs(string userId, int pageNumber, int pageSize, string searchStr);
    }
}
