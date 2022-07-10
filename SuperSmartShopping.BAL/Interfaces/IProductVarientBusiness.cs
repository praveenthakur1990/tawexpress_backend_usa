using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
   public interface IProductVarientBusiness
    {
        int AddUpdate(ProductVarientsModel model, string connectionStr);
        List<ProductVarientsModel> GetProductsVarients(int id, int productId, PaginationModel pageModel, string connectionStr);
    }
}
