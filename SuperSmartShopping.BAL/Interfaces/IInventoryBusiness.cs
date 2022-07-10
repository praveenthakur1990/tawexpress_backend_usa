using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IInventoryBusiness
    {
        int Add(StockModel model, string connectionStr);
        List<StockModel> GetInventorybyProductId(int productId, PaginationModel pageModel, string connectionStr);
    }
}
