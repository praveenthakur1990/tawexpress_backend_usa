using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class OrderStatusLogs
    {
        [Key]
        public int? OrderId { get; set; }
        public string Status { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDateTime { get; set; }
    }
}
