using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class PaginationModel
    {
        [DefaultValue(0)]
        public int PageNumber { get; set; }
        [DefaultValue(0)]
        public int PageSize { get; set; }
        [DefaultValue("")]
        public string SearchStr { get; set; }
    }
}
