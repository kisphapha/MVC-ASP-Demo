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
    public class ProductsController : Controller
    {
        private readonly MVCDemoDbContext _context;

        public ProductsController(MVCDemoDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return _context.Products != null ? 
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'MVCDemoDbContext.Products'  is null.");
        }
        
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Picture")] Product product)
        {
            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if(user == null || user.Role != 1)
            {
                RedirectToAction("/Home/Index");
            }

            _context.Add(product);
            await _context.SaveChangesAsync();         
            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Picture")] Product product)
        {
            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (user == null || user.Role != 1)
            {
                RedirectToAction("/Home/Index");
            }
            if (id != product.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
            
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            User user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            if (user == null || user.Role != 1)
            {
                RedirectToAction("/Home/Index");
            }
            if (_context.Products == null)
            {
                return Problem("Entity set 'MVCDemoDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
