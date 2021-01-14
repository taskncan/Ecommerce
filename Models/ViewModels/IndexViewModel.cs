using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceHomework.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<ItemViewModel> Items { get; set; }

        public int TotalMovieCount { get; set; }

        public int Page { get; set; }

        public bool IsLoggedIn { get; set; }

        public List<String> Categories { get; set; }
    }
}
