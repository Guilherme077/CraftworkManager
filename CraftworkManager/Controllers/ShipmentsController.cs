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
            var shipments = await DbContext.Shipments.Where(s => s.Order.userId == userId).Include(s => s.Order).Where(s => s.Status == ShipmentStatus.Pending || s.Status == ShipmentStatus.Shipped).OrderBy(s => s.ShippedOn).ToListAsync();

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Shipment viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shipment = await DbContext.Shipments.Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == viewModel.Id && s.Order.userId == userId);

            if(shipment is not null)
            {
                shipment.TrackingCode = viewModel.TrackingCode;
                shipment.TrackingUrl = viewModel.TrackingUrl;
                shipment.TransportCompany = viewModel.TransportCompany;
                shipment.Notes = viewModel.Notes;
                shipment.ShippingCost = viewModel.ShippingCost;
                shipment.ShippingCostIncludedOnPrice = viewModel.ShippingCostIncludedOnPrice;
                shipment.Address = viewModel.Address;
                shipment.City = viewModel.City;
                shipment.State = viewModel.State;
                shipment.ZipCode = viewModel.ZipCode;
                shipment.Country = viewModel.Country;

                await DbContext.SaveChangesAsync();
                _toast.AddSuccessToastMessage("Alterações aplicadas!");
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid id, ShipmentStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shipment = await DbContext.Shipments.Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == id && s.Order.userId == userId);
            if (shipment is not null)
            {
                shipment.Status = status;
                if (status == ShipmentStatus.Shipped)
                {
                    shipment.ShippedOn = DateTime.Now;
                    shipment.Order.Status = OrderStatus.Shipped;
                    await DbContext.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = id });
                }
                else if (status == ShipmentStatus.Delivered)
                    shipment.DeliveredOn = DateTime.Now;
                else if (status == ShipmentStatus.Cancelled)
                    shipment.Order.Status = OrderStatus.Cancelled;
                else if (status == ShipmentStatus.Failed || status == ShipmentStatus.Returned)
                    shipment.Order.Status = OrderStatus.ReadyToShip;
       
                await DbContext.SaveChangesAsync();
                _toast.AddSuccessToastMessage("Status atualizado!");
            }
            return RedirectToAction("Index");
        }
    }
}
