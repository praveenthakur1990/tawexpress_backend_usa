using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface ISpecialOfferBusiness
    {
        int AddUpdate(SpecialOfferModel model, string connectionStr);
        List<SpecialOfferModel> Get(int id, PaginationModel model, string connectionStr);
        List<ProductDashboardModel> GetProductBySpecialOfferId(string categoryId, int specialOfferId, string connectionStr);
        int AddUpdateProduct(WeeklyCircularCatInfoModel model, string connectionStr);
        List<SpecialOfferDatesModel> GetSpecialOfferDates(string connectionStr);
    }
}
