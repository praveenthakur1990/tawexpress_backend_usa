using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class RiderStoreLinking
    {
        [Key]
        public int Id { get; set; }
        public int? RiderId { get; set; }
        public string StoreId { get; set; }
    }
}
