using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Models;
using System;
using System.Text.RegularExpressions;

namespace MVCDemo.Controllers
{
    public class UserController : Controller
    {
        private readonly MVCDemoDbContext mvcDemoContext;

        public UserController(MVCDemoDbContext mvcDemoContext)
        {
            this.mvcDemoContext = mvcDemoContext;
        }
        public IActionResult GetUser(int id)
        {
            var user = mvcDemoContext.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                return Json(user);
            }
            else
            {
                return NotFound(); // Return 404 Not Found if the user is not found
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(string Name, string Email, string Phone, string Password, string CfmPassword)
        {
            if (Email == null || Phone == null || Name == null || Password == null || CfmPassword == null)
            {
                TempData["ErrorMessage"] = "Please enter enough information";
                return RedirectToAction("Register", "Home");
            }
            if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                TempData["ErrorMessage"] = "Invalid email";
                return RedirectToAction("Register", "Home");
            }
            if (!(Regex.IsMatch(Phone, @"^[0-9]+$")) || Phone.Length < 10 || Phone.Length > 12)
            {
                TempData["ErrorMessage"] = "Invalid phone number";
                return RedirectToAction("Register", "Home");
            }
            if (!Password.Equals(CfmPassword))
            {
                TempData["ErrorMessage"] = "Password did not match";
                return RedirectToAction("Register", "Home");
            }
            User user = new User()
            {
                Id = 0,
                Name= Name,
                Email = Email,
                Password = Password,
                phoneNumber = Phone,
                Role = 2
            };
            mvcDemoContext.Users.Add(user);
            await mvcDemoContext.SaveChangesAsync();
            HttpContext.Session.SetString("User", user.ToString());
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int userId, string name, string phone)
        {
            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (!(Regex.IsMatch(phone, @"^[0-9]+$")) || phone.Length < 10 || phone.Length > 12)
            {
                TempData["ErrorMessage"] = "Invalid phone number";
                return RedirectToAction("Profile", "User");
            }
            if (user == null || user.Role != 1)
            {
                RedirectToAction("/Home/Index");
            }
            var user_ = await mvcDemoContext.Users.FindAsync(userId);
            if (user_ == null)
            {
                return NotFound();
            }
            user_.Name = name;
            user_.phoneNumber = phone;
            HttpContext.Session.SetString("User", user_.ToString());
            await mvcDemoContext.SaveChangesAsync();
            return RedirectToAction("Profile","User");
        }
        [HttpPost]
        public IActionResult checkLogin(LoginModel loginUser)
        {
            var user = mvcDemoContext.Users.FirstOrDefault(u => u.Email == loginUser.Email && u.Password == loginUser.Password);
            if (user != null)
            {
                HttpContext.Session.SetString("User",user.ToString());
                if (user.Role == 1)
                {
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("","");
            }
        }
        public IActionResult logout() { 
        
            HttpContext.Session.Remove("User");
            return RedirectToAction("", "");

        }
        public IActionResult Profile()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return View();
        }
        public async Task<IActionResult> Orders()
        {
            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            ViewBag.user = user;
            ViewBag.context = mvcDemoContext;
            if (user == null)
            {
                return RedirectToAction("", "");
            }
            // Retrieve the orders based on the provided userId
            var orders = await mvcDemoContext.Orders
                .Include(o => o.User)
                .Include(o => o.Details)
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            return View(orders);
        }
    }
}
