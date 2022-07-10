using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string CapturedId { get; set; }
        public decimal? CapturedAmt { get; set; }
        public string Currency { get; set; }
        public string Email_name { get; set; }
        public string Funding { get; set; }
        public string NetWorkStatus { get; set; }
        public string SellerMessage { get; set; }
        public bool? Paid { get; set; }
        public string PaymentMethod { get; set; }
        public string Card_brand { get; set; }
        public string Country { get; set; }
        public string Network { get; set; }
        public string Last4 { get; set; }
        public int? Exp_Month { get; set; }
        public int? Exp_Year { get; set; }
        public string Status { get; set; }
        public string Receipt_url { get; set; }
        public string FailureCode { get; set; }
        public string FailureMessage { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
