using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class PickupAvailabilityModel
    {
        public string PickupDate { get; set; }
        public string HourOpenTime { get; set; }
        public string HourCloseTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class PickUpDateVM
    {
        public string PickUpDate { get; set; }
        public int WeekDayId { get; set; }
    }

    public class PickUpTimeVM
    {
        public string PickUpTime { get; set; }
        public int WeekDayId { get; set; }
    }
}
