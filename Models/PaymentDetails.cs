using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceHomework.Models
{
    public class PaymentDetails
    {
        public string Name { get; set; }

        public string CreditCard { get; set; }

        public DateTime LastTime { get; set; }

        public int CVV { get; set; }

        public PaymentType Type { get; set; }
    }

    public enum PaymentType
    {
        CreditCard = 0,
        Cash = 1
    }
}
