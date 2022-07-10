using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IProductBusiness
    {
        int AddUpdate(Product model, string connectionStr);
        List<ProductModel> GetProducts(int productId, PaginationModel pageModel, string connectionStr);
        int MarkProductAsDeleted(int productId, string connectionStr);
        List<ProductDashboardModel> GetProductForDashboard(int categoryId, int limit, string connectionStr);
        List<ProductDashboardModel> GetProductByCategoryIdAPP(string categoryId, string subCategoryIds, string brandIds, string connectionStr);
        List<ProductDashboardModel> GetRelatedProductByIdAPP(int productId, int limit, string connectionStr);

    }
}
