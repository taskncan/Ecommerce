using EcommerceHomework.AppContext;
using EcommerceHomework.Models;
using EcommerceHomework.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRecommender.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        public IActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Username) && !string.IsNullOrWhiteSpace(user.Password))
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                bool isValid = _context.CheckUser(user.Username, user.Password);
                if (isValid)
                {
                    var currentUSer = _context.GetUserByName(user.Username);

                    if (HttpContext.Session.Get<User>(SessionExtensions.UserKey) == default(User))
                    {
                        currentUSer.Password = "";
                        HttpContext.Session.Set<User>(SessionExtensions.UserKey, currentUSer);
                    }
                }
                else
                {
                    ViewBag.JavaScriptFunction = "showError();";
                    return View(user);
                }
            }

            return Redirect("/Home/Index");
        }

        public IActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult Logout()
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);

            if (currentUser != default(User))
            {
                HttpContext.Session.Set<User>(SessionExtensions.UserKey, null);
            }

            return View("Login", new User());
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Username) && !string.IsNullOrWhiteSpace(user.Password))
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                var userCheck = _context.GetUserByName(user.Username);

                if (userCheck == null)
                {
                    _context.AddUser(user);
                }
                else
                {
                    return Redirect("Register");
                }
            }

            return View("Login", user);
        }

        public async Task<IActionResult> Cart()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);
            var cart = _context.GetUserBasket(currentUser.Username);

            return View(new CardViewModel() { CardItems = cart });
        }

        [HttpPost]
        public bool AddItemToBasket(int itemId, string username)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            if (_context.AddItemToUserBasket(itemId, username))
            {
                return true;
            }
            else
                return false;
        }

        public async Task<IActionResult> DeleteCartItem(int id)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            _context.DeleteCartItem(id);
            return Redirect("/User/Cart");
        }

        public async Task<IActionResult> Orders()
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);

            if (currentUser != default(User))
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                var userOrders = _context.GetUserOrders(currentUser.Username);

                return View(userOrders);
            }

            return View("Login", new User());
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            _context.ChangeOrderStatus(orderId, (int)OrderStatus.Cancelled);

            return Redirect("Orders");
        }

        public async Task<IActionResult> Checkout()
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);
            var cart = _context.GetUserBasket(currentUser.Username);
            var check = new CheckoutViewModel()
            {
                CardItems = cart,
                PaymentDetail = new PaymentDetails(),
                UserDetails = currentUser
            };
            return View(check);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(PaymentDetails details)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);
            var cart = _context.GetUserBasket(currentUser.Username);

            var orderItems = cart.Select(x => x.Item).ToList();
            var totalPrice = orderItems.Sum(x => x.Price);

            Order order = new Order()
            {
                Items = orderItems,
                Price = totalPrice,
                Status = OrderStatus.Processing,
                OrderTime = System.DateTime.Now,
                User = currentUser,
                PaymentType = details.Type
            };

            var status = _context.CreateOrder(order);
            if (status)
            {
                cart.ForEach(cItem =>
                {
                    _context.DeleteCartItem(cItem.Id);
                });

                return Redirect("/User/Orders");
            }
            else
            {
                return Redirect("Checkout");
            }
        }

    }
}
