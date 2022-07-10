using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class SpecialOfferCatInfo
    {
        [Key]
        public int SpecialOfferCatId { get; set; }
        public int SpecialOfferId { get; set; }
        public int CategoryId { get; set; }
        public int SubCatgoryId { get; set; }
    }
}
