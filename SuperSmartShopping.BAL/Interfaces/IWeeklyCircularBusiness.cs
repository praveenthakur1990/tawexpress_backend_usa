using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IWeeklyCircularBusiness
    {
        int AddUpdate(WeeklyCircularModel model, string connectionStr);
        List<WeeklyCircularModel> Get(int id, PaginationModel model, string connectionStr);
        List<WeeklyCircularDatesModel> GetWeeklyCircularDates(string connectionStr);
        int AddUpdateProduct(WeeklyCircularCatInfoModel model, string connectionStr);
        List<ProductDashboardModel> GetProductByWeeklyCircularId(string categoryId, int weeklyCircularId, string connectionStr);

        int AddUpdateSubscriber(WeeklyCircularSubscriberModel model, string connectionStr);
        List<WeeklyCircularSubscriberModel> GetWeeklyCircularSubscriber(string mobileNumber, PaginationModel pageModel, string connectionStr);
        int UpdateSubscriberOTP(string mobileNumber, string otp, string connectionStr);
        Dictionary<string, string> GetSubscriberPhoneNumber(string[] subscriberIds, string connectionStr);
    }
}
