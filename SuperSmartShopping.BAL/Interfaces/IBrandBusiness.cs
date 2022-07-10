using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IBrandBusiness
    {
        int AddUpdate(BrandModel model, string connectionStr);
        List<BrandModel> GetBrands(int productId, PaginationModel pageModel, string connectionStr);
        List<BrandModel> GetBrandsForAPP(int categoryId, string subCategoryIds, string connectionStr);
    }
}
