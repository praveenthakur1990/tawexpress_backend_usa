using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class QuickPageModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string PageContent { get; set; }
        public bool IsActive { get; set; }
        public bool IsRedirectToUrl { get; set; }
        public string CreatedBy { get; set; }
    }
}
