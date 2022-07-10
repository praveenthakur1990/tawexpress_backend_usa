using SuperSmartShopping.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.BAL.Interfaces
{
    public interface IUnitMeasurementBusiness
    {
        int AddUpdateUnitMeasurement(UnitMeasurement model, string connectionStr);
        List<UnitMeasurement> GetUnitMeasurements(int id, bool isShowInAdmin, string connectionStr);
    }
}
