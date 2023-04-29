using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ООО_Спорт.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProduct = new HashSet<OrderProduct>();
        }

        public string ProductArticleNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public string ProductPhoto { get; set; }
        public string ProductManufacturer { get; set; }
        public decimal ProductCost { get; set; }
        public byte? ProductDiscountAmount { get; set; }
        public int ProductQuantityInStock { get; set; }
        public string ProductStatus { get; set; }

        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
