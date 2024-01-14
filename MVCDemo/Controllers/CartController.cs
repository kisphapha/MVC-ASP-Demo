using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Models;
using Newtonsoft.Json;
using System;

namespace MVCDemo.Controllers
{
    public class CartController : Controller
    {
        private readonly MVCDemoDbContext mvcDemoContext;

        public CartController(MVCDemoDbContext mvcDemoContext)
        {
            this.mvcDemoContext = mvcDemoContext;
        }
        private Product? GetProduct(int id)
        {
            return mvcDemoContext.Products.FirstOrDefault(x => x.Id == id);
        }
        public IActionResult Shop()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            ViewBag.products = mvcDemoContext.Products.ToList();
            return View();
        }
        public IActionResult AddToCart(int ProductId)
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrWhiteSpace(cart) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(cart);

            if (cartItems.ContainsKey(ProductId))
            {
                cartItems[ProductId] += 1;
            }
            else
            {
                cartItems.Add(ProductId, 1);
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
            return RedirectToAction("Shop", "Cart");
        }
        [HttpPost]
        public IActionResult ChangeQuantity(int ProductId, int mode)
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrWhiteSpace(cart) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(cart);

            if (cartItems.ContainsKey(ProductId))
            {
                if (cartItems[ProductId] > 1 + (mode - 1))
                    cartItems[ProductId] += 1 - mode * 2;
            }
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
            return RedirectToAction("Cart", "Cart");
        }
        public IActionResult RemoveFromCart(int ProductId)
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrWhiteSpace(cart) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(cart);

            if (cartItems.ContainsKey(ProductId))
            {
                cartItems.Remove(ProductId);
            }
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
            return RedirectToAction("Cart", "Cart");
        }

        public IActionResult RemoveAll()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Shop", "Cart");
        }

        public IActionResult Cart()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            string cartStr = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrWhiteSpace(cartStr) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(cartStr);
            List<CartDataModel> products = new List<CartDataModel>();
            foreach (var item in cartItems)
            {
                int productId = item.Key;
                int quantity = item.Value;
                Product? pro = GetProduct(productId);
                CartDataModel cartItem = new CartDataModel()
                {
                    Product = pro,
                    Quantity = quantity
                };
                products.Add(cartItem);
            }
            ViewBag.cart = products;
            return View();
        }
    }
}
