using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface ICategoryBusiness
    {
        int AddUpdateCategory(Category model, string connectionStr);
        List<Category> GetCategories(string type, string connectionStr);
        int MarkCategoryAsDeleted(int categoryId, string connectionStr);
        List<CategoryModel> GetSubCategoriesByCatId(int categoryId, string connectionStr);
    }
}
