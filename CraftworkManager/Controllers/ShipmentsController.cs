using CraftworkManager.Data;
using CraftworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CraftworkManager.Controllers
{
    public class ShipmentsController : Controller
    {
        private readonly DBContext DbContext;
        private readonly IToastNotification _toast;
        public ShipmentsController(DBContext dBContext, IToastNotification toast) { DbContext = dBContext; _toast = toast; }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shipments = await DbContext.Shipments.Where(s => s.Order.userId == userId).Include(s => s.Order).OrderBy(s => s.ShippedOn).ToListAsync();

            return View(shipments);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateShipmentByOrder(Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderToShip = await DbContext.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == orderId && o.userId == userId);
            Shipment shipment = new Shipment()
            {
                OrderId = orderToShip.Id,
                CreatedAt = DateTime.Now,
                Status = ShipmentStatus.Pending
            };
            await DbContext.Shipments.AddAsync(shipment);
            await DbContext.SaveChangesAsync();
            return RedirectToAction("Edit", "Shipments", new { id = shipment.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shipment = await DbContext.Shipments.Where(s => s.Order.userId == userId).Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == id);
            return View(shipment);
        }

    }
}
