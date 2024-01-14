using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Models;

namespace MVCDemo.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MVCDemoDbContext _context;

        public OrdersController(MVCDemoDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return _context.Orders != null ?
                          View(await _context.Orders.Include(o => o.User).ToListAsync()) :
                          Problem("Entity set 'MVCDemoDbContext.Orders'  is null.");
        }
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Details) // Include the Details navigation property
                .Include(o => o.User) // Include the User navigation property
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.order = order;
            return View(_context);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, decimal total, DateTime createdDate, string status, Dictionary<int, int> orderDetails)
        {
            // Create the order
            Order order = new Order
            {
                UserId = userId,
                Total = (int) total,
                CreatedDate = createdDate,
                Status = status
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            foreach (var item in orderDetails)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    Id = 0,
                    ProductId = item.Key,
                    OrderId = order.Id,
                    Quantity = item.Value
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("RemoveAll", "Cart");
        }
        [HttpPost]

        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            string[] statuses = new string[4] ;
            statuses[0] = "Processing";  
            statuses[1] = "Approved and Preparing";
            statuses[2] = "Shipping";
            statuses[3] = "Success";

            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (user == null || user.Role != 1)
            {
                RedirectToAction("/Home/Index");
            }
            // Find the order by its ID
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Update the order status
            order.Status = statuses[Array.IndexOf(statuses,status) + 1];
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
