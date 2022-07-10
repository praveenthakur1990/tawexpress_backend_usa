using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IBusinessHoursBusiness
    {
        List<BusinessHoursModel> GetBusinessHours(string connectionStr);
        int AddUpdateBusinessHours(string businessHourksonStr, string connectionStr);
    }
}
