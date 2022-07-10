using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class MessageResources
    {
        [Key]
        public int Id { get; set; }
        public string AccountSid { get; set; }
        public string ApiVersion { get; set; }
        public string Body { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateSent { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Direction { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string From { get; set; }
        public double? Price { get; set; }
        public string PriceUnit { get; set; }
        public string Sid { get; set; }
        public string Status { get; set; }
        public string To { get; set; }
        public string Uri { get; set; }
    }
}
