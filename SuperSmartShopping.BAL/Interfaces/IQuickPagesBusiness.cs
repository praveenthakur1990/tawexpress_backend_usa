using SuperSmartShopping.DAL.Models;
using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IQuickPagesBusiness
    {
        List<QuickPageModel> GetQuickPages(string connectionStr);
        int AddUpdateQuickPages(QuickPageModel model, string connectionStr);
    }
}
