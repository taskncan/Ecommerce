using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceHomework.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CardItem> CardItems { get; set; }

        public PaymentDetails PaymentDetail { get; set; }

        public User UserDetails { get; set; }
    }
}
