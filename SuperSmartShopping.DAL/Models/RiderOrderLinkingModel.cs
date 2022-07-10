using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class RiderOrderLinkingModel
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int RiderId { get; set; }
        public string StoreUserId { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime RequestSentDate { get; set; }
    }
}
