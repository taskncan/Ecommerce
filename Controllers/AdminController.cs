using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommerceHomework.AppContext;
using EcommerceHomework.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHomework.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Orders()
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);

            if (currentUser != default(User) && currentUser.Role == UserRole.ADMIN)
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                var userOrders = _context.GetAllOrders();

                return View(userOrders);
            }
            else
            {
                return Redirect("/Home/Index");
            }

        }

        [HttpPost]
        public IActionResult ChangeStatus(int orderId,OrderStatus status)
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);

            if (currentUser != default(User) && currentUser.Role == UserRole.ADMIN)
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                var userOrders = _context.ChangeOrderStatus(orderId,(int)status);

                var allOrders = _context.GetAllOrders();

                return View("Orders", allOrders);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        public IActionResult Register()
        {
            return View(new User());
        }
    }
}