using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IStoreBusiness
    {
        int AddUpdate(StoreModel model, string connectionStr);
        StoreModel GetStore(string connectionStr);
        int AddUpdateStripeAPIKey(string publishablekey, string secretkey, string connectionStr);
        int UpdateDeliveryChargesTaxes(decimal deliveryCharges, decimal tax, bool isEnableCashOnDelivery, string connectionStr);
        int UpdateDeliveryAreaSetting(decimal minOrderedAmt, decimal deliveryAreaMiles, string connectionStr);
        int AddBannerImage(List<BannerImages> model, string connectionStr);
        List<BannerImages> GetBannerImage(int id, string bannerType, string connectionStr);
        int MarkAsActiveInActive(int id, bool status, string connectionStr);
        int MarkBannerAsDeleted(int categoryId, string connectionStr);
    }
}
