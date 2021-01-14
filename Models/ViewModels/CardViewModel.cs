using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceHomework.Models.ViewModels
{
    public class CardViewModel
    {
        public List<CardItem> CardItems { get; set; }
    }

    public class CardItem
    {
        public int Id { get; set; }

        public Item Item { get; set; }
    }
}
