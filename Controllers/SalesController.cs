using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {

        private readonly InventoryDbContext _context;
        public SalesController(InventoryDbContext context)
        {
            _context=context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateReport(int orderId)
        {
            // Fetch order details based on orderId from the Order table
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                ModelState.AddModelError(string.Empty, "Order not found.");
                return View("Index");
            }

            // Fetch product details based on productId from the Product table
            var product = _context.Products.FirstOrDefault(p => p.ProductId == order.ProductId);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View("Index");
            }

            // Calculate total amount based on product price and order quantity
            int totalAmount = product.Price * order.Quantity;

            // Create a new sales record
            var sale = new Sale
            {
                OrderId = orderId,
                SaleDate = DateTime.Now,
                TotalAmount = totalAmount
            };

            // Save the sales record to the database
            _context.Sales.Add(sale);
            _context.SaveChanges();

            return RedirectToAction(nameof(SalesReport));
        }
        public IActionResult SalesReport()
        {
            // Fetch sales record based on saleId
            var sale = _context.Sales.ToList();
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
    }
}
