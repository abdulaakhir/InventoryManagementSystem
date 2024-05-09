using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{

    [Authorize]
    public class OrderController : Controller
    {
        private readonly InventoryDbContext _context;

        public OrderController(InventoryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
           // var orders = _context.Orders.ToList();

            var orders = _context.Orders.Include(o => o.Product).ToList();

            return View(orders );
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult PlaceOrder()
        {
            ViewBag.ProductList = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(Order order)
        {
            var product = _context.Products.Find(order.ProductId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                ViewBag.ProductList = new SelectList(_context.Products, "ProductId", "Name");
                return View(order);
            }

            // Product found, place order
            order.OrderDate = DateTime.Now;
            order.Status = "Pending"; // Assuming initial status is "Pending"

            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult PayOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null && order.Status == "Pending")
            {
                order.Status = "Successful";
                _context.SaveChanges();
                return Ok(); // Status code 200 - Success
            }
            return NotFound(); // Status code 404 - Order not found or already paid
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.ProductList = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    // Handle exception
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProductList = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
