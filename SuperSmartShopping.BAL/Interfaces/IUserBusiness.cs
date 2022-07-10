using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IUserBusiness
    {
        int UpdatePersonalInfo(PersonalInfoModel model);
        PersonalInfoModel GetPersonalInfo(string userId);
        List<PersonalInfoModel> GetUsersByRoleName(string roleName);
    }
}
