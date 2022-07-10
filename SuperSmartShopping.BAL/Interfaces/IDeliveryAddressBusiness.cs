using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IDeliveryAddressBusiness
    {
        int AddUpdate(DeliveryAddressesModel model, string connectionStr);
        List<DeliveryAddressesModel> Get(int deliveryAddressId, string userId, string connectionStr);
        List<DateTime> GetDeliverySlotDays(int days, string connectionStr);
        List<DeliverySlotTime> GetDeliverySlotTime(int interval, int start, int end, string connectionStr);
        decimal CalculateDeliveryAddressDistance(int deliveyAddress, string connectionStr);
    }
}
