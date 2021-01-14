using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceHomework.Models
{
    public class Order
    {
        public int Id { get; set; }

        public User User { get; set; }

        public DateTime OrderTime { get; set; }

        public double Price { get; set; }

        public List<Item> Items { get; set; }

        public OrderStatus Status { get; set; }

        public PaymentType PaymentType { get; set; }
    }

    public enum OrderStatus
    {
        Processing = 1,
        InTransit = 2 ,
        Delivered = 3,
        Cancelled = 4,
    }
}
