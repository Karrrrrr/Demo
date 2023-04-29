using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ООО_Спорт.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProduct = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDeliveryDate { get; set; }
        public string OrderPickupPoint { get; set; }

        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
