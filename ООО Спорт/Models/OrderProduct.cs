using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ООО_Спорт.Models
{
    public partial class OrderProduct
    {
        public int OrderId { get; set; }
        public string ProductArticleNumber { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product ProductArticleNumberNavigation { get; set; }
    }
}
