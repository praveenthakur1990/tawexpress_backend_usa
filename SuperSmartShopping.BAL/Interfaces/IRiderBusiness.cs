using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IRiderBusiness
    {
        List<StoreListModel> GeStoreNames();
        int AddUpdate(RiderModel model);
        List<RiderModel> GetRiders(int id, PaginationModel model);
        int UpdateDeviceToken(string emailAddress, string deviceToken);
        List<RiderModel> GetStoreLinkedRiders(string storeUserId);
        int SendOrderRequestToRider(int orderId, int riderId, string storeUserId);
        List<OrderInfoModel> GetRiderOrders(string userName, string type, PaginationModel model);
        bool IsRiderOnline(string userName);
        int UpdateRiderOnlineStatus(string emailAddress, bool isOnline);

        int UpdateRiderOrderStatus(string userName, int orderId, string storeUserId);
    }
}
