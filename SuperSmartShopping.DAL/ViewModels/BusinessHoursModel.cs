using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class BusinessHoursModel
    {
        public int Id { get; set; }
        public int? WeekDayId { get; set; }
        public string DayName { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string OpenTime12Hour { get; set; }
        public string CloseTime12Hour { get; set; }
        public bool? IsClosed { get; set; }
        public string CurrentTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
