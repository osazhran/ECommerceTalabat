using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }

        //public int DelivaryMethodId { get; set; } // Foreign Key[1]
        public virtual DeliveryMethod? DelivaryMethod { get; set; } // Navigational Property [ONE]
        public virtual ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); // Navigational Property [MANY]
        public decimal SubTotal { get; set; }

        //[NotMapped]
        //public decimal Total => SubTotal + DelivaryMethod.Cost;
        public decimal GetTotal() // GET لازم يبقي اسمها 
            => SubTotal + DelivaryMethod.Cost;

        public string PaymentIntentId { get; set; }

        public Order()
        {
            
        }
        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod? delivaryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DelivaryMethod = delivaryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }
    }
}
