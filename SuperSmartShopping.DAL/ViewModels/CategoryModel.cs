using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.ViewModels
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalProductCount { get; set; }
    }
}
