using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string OrderBy { get; set; }
        public string OrderType { get; set; }
        public int? DeliveryAddressId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? DeliveryEndTime { get; set; }
        public DateTime? OrderdDate { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? DeliveryCharges { get; set; }
    }
}
