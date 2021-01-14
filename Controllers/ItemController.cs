using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EcommerceHomework.AppContext;
using EcommerceHomework.Models;
using EcommerceHomework.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHomework.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        // GET: Item/Details/5
        public ActionResult Details(int id)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            ItemViewModel model = null;

            var item = _context.GetItem(id);

            if (item != null)
            {
                model = new ItemViewModel()
                {
                    Id = item.Id,
                    Category = item.Category,
                    Description = item.Description,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoBytes = item.Photo
                };
                return View(model);
            }
            return View("Index");
        }

        // GET: Item/Create
        public ActionResult Create()
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);

            if (currentUser != default(User) && currentUser.Role == UserRole.ADMIN)
            {
                return View();
            }
            else
            {
                return View("/Home/Login", new User());
            }
        }

        // POST: Item/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemViewModel item)
        {
            try
            {
                if (item.Photo != null)
                {
                    if (item.Photo.Length > 0)
                    {
                        byte[] p1 = null;
                        using (var fs1 = item.Photo.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                        Item newItem = new Item()
                        {
                            Name = item.Name,
                            Category = item.Category,
                            Description = item.Description,
                            Photo = p1,
                            Price = item.Price
                        };
                        _context.AddItem(newItem);
                        item.PhotoBytes = p1;
                        return View("Details", item);
                    }
                }

                return View("Create", item);
            }
            catch
            {
                return View();
            }
        }

        // GET: Item/Edit/5
        public ActionResult Edit(int id)
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            if (currentUser != default(User) && currentUser.Role == UserRole.ADMIN)
            {
                var item = _context.GetItem(id);

                ItemViewModel model = new ItemViewModel()
                {
                    Id = item.Id,
                    Category = item.Category,
                    Description = item.Description,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoBytes = item.Photo
                };

                return View(model);
            }
            else
            {
                return View("/Home/Login", new User());

            }
        }

        // POST: Item/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemViewModel item)
        {
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            try
            {
                byte[] p1 = null;

                if (item.Photo != null)
                {
                    if (item.Photo.Length > 0)
                    {
                        using var fs1 = item.Photo.OpenReadStream();
                        using var ms1 = new MemoryStream();
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                }

                Item newItem = new Item()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category,
                    Description = item.Description,
                    Price = item.Price,
                    Photo = p1
                };

                _context.EditItem(id, newItem);

                return Redirect("/Home/Index");
            }
            catch
            {
                return Redirect("Home/Index");
            }
        }


        // GET: Item/Delete/5
        public ActionResult Delete(int id)
        {
            var currentUser = HttpContext.Session.Get<User>(SessionExtensions.UserKey);
            DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

            if (currentUser != default(User) && currentUser.Role == UserRole.ADMIN)
            {
                var item = _context.GetItem(id);

                ItemViewModel model = new ItemViewModel()
                {
                    Id = item.Id,
                    Category = item.Category,
                    Description = item.Description,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoBytes = item.Photo
                };

                return View(model);
            }
            else
            {
                return View("/Home/Login", new User());

            }
        }

        // POST: Item/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletion(int id)
        {
            try
            {
                DbContext _context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;

                _context.DeleteItem(id);

                return Redirect("/Home/Index");
            }
            catch
            {
                return View();
            }
        }
    }
}