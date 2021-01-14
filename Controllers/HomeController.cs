using EcommerceHomework.AppContext;
using EcommerceHomework.Models;
using EcommerceHomework.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;

namespace EcommerceHomework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            List<Item> items = _context.GetAllItems();

            IndexViewModel viewModel = new IndexViewModel()
            {
                Items = items.ConvertAll<ItemViewModel>(x => new ItemViewModel()
                {
                    Category = x.Category,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    PhotoBytes = x.Photo,
                    Price = x.Price
                }),
                Categories = _context.GetCategories()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CategoryFilter(string categories)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            List<Item> items = _context.GetFilteredItems(categories);

            IndexViewModel viewModel = new IndexViewModel()
            {
                Items = items.ConvertAll<ItemViewModel>(x => new ItemViewModel()
                {
                    Category = x.Category,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    PhotoBytes = x.Photo,
                    Price = x.Price
                }),
                Categories = _context.GetCategories()
            };
            return View("Index",viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
