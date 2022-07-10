using SuperSmartShopping.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IPlanBusiness
    {
        int AddUpdatePlan(PlanModel model);
        List<PlanModel> GetPlans(int planId);
    }
}
