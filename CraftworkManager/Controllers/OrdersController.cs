using CraftworkManager.Data;
using CraftworkManager.Models;
using CraftworkManager.Models.Logs;
using CraftworkManager.Models.LogsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CraftworkManager.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DBContext DbContext;
        private readonly IToastNotification _toast;
        public OrdersController(DBContext dBContext, IToastNotification toast) { DbContext = dBContext; _toast = toast; }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var order = new Order
            {
                CreatedOn = DateTime.Now,
                LastUpdateOn = DateTime.Now,
                DeadlineOn = DateTime.Now.AddDays(1),
                Status = OrderStatus.Pending,
                Url = null,
                WithNF = false,
                PaymentWay = Payment.Other,
                ClientName = "Cliente Padrão",
                ClientAddress = "Endereço Padrão",
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await DbContext.Orders.AddAsync(order);

            var OrderHistory = new OrderStatusHistory
            {
                Order = order,
                OrderId = order.Id,
                ChangedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ModificationType = ModificationType.Created,
                Status = order.Status,
                ChangedOn = DateTime.Now
            };

            await DbContext.OrderStatusHistory.AddAsync(OrderHistory);
            await DbContext.SaveChangesAsync();
            return RedirectToAction("Edit", "Orders", new { id = order.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await DbContext.Orders.Include(o => o.OrderItems).ThenInclude(o => o.BaseProduct).FirstOrDefaultAsync(o => o.Id == id && o.userId == userId);

            ViewBag.PaymentList = Enum.GetValues(typeof(Payment))
            .Cast<Payment>()
            .Select(s => new SelectListItem
            {
                Value = ((int)s).ToString(),
                Text = s.GetAttribute<DisplayAttribute>().Name
            });

            ViewBag.StatusHistory = await DbContext.OrderStatusHistory.Where(h => h.OrderId == id).Include(h => h.Order).OrderByDescending(h => h.ChangedOn).ToListAsync();

            return View(order);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Order viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await DbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == viewModel.Id && o.userId == userId);

            if (order is not null)
            {
                order.DeadlineOn = viewModel.DeadlineOn;
                order.ClientName = viewModel.ClientName;
                order.ClientAddress = viewModel.ClientAddress;
                order.PaymentWay = viewModel.PaymentWay;
                order.Url = viewModel.Url;
                order.WithNF = viewModel.WithNF;
                order.Notes = viewModel.Notes;
                order.Discount = viewModel.Discount;
                order.Raise = viewModel.Raise;
                order.Taxes = viewModel.Taxes;
                order.LastUpdateOn = DateTime.Now;

                await DbContext.SaveChangesAsync();
                _toast.AddInfoToastMessage("Pedido editado! Preço do Pedido: R$" + order.getTotalPrice());
            }

            return RedirectToAction("Edit", "Orders", new { id = order.Id });
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> List()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var orders = await DbContext.Orders.Where(p => p.userId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.BaseProduct).OrderBy(p => p.DeadlineOn).ToListAsync();
        //    return View(orders);
        //}

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await DbContext.Orders.Where(p => p.userId == userId && p.Status != OrderStatus.Cancelled && p.Status != OrderStatus.Shipped).Include(o => o.OrderItems).ThenInclude(oi => oi.BaseProduct).OrderBy(p => p.DeadlineOn).ToListAsync();
            return View(orders);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddItem(Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.Products = DbContext.Products.Where(p => p.userId == userId).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            var vm = new AddItemViewModel { OrderId = orderId };
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItem(AddItemViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await DbContext.Products
                .FirstOrDefaultAsync(p => p.Id == viewModel.ProductId && p.userId == userId);

            if (product is null)
                return BadRequest();

            var order = await DbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == viewModel.OrderId && o.userId == userId);

            if (order is null)
                return BadRequest();

            var item = new OrderItem
            {
                OrderId = viewModel.OrderId,
                Order = order,
                BaseProductId = viewModel.ProductId,
                BaseProduct = product,
                Quantity = viewModel.Quantity,
                Cost = viewModel.Cost,
                Price = viewModel.Price,
                Description = viewModel.Description
            };

            await DbContext.OrderItems.AddAsync(item);
            await DbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "Orders",
                new { id = item.OrderId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditItem(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await DbContext.OrderItems
                .Include(i => i.BaseProduct)
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == id && i.Order.userId == userId);
            if (item is null)
                return NotFound();

            return View(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditItem(OrderItem viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await DbContext.OrderItems
                .Include(i => i.BaseProduct)
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == viewModel.Id && i.Order.userId == userId);
            if (item is null)
                return NotFound();

            item.Quantity = viewModel.Quantity;
            item.Cost = viewModel.Cost;
            item.Price = viewModel.Price;
            item.Description = viewModel.Description;

            await DbContext.SaveChangesAsync();
            return RedirectToAction("Edit", "Orders",
                new { id = item.OrderId });
        }

        //TODO: Refactor to change to HttpPost
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteItem(Guid id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await DbContext.OrderItems.Include(i => i.Order).FirstOrDefaultAsync(i => i.Id == id && i.Order.userId == userId);

            if(item is null)
            {
                return NotFound();
            }

            DbContext.OrderItems.Remove(item);
            await DbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "Orders", new { id = item.OrderId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid id, OrderStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await DbContext.Orders.Where(p => p.userId == userId).FirstOrDefaultAsync(o => o.Id == id);
            order.Status = status;
            var OrderHistory = new OrderStatusHistory
            {
                Order = order,
                OrderId = order.Id,
                ChangedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ModificationType = ModificationType.StatusChanged,
                Status = order.Status,
                ChangedOn = DateTime.Now
            };
            await DbContext.OrderStatusHistory.AddAsync(OrderHistory);
            await DbContext.SaveChangesAsync();
            
            if (status == OrderStatus.ReadyToShip)
            {
                return RedirectToAction("CreateShipmentByOrder", "Shipments", new { orderId = order.Id});
            }

            _toast.AddSuccessToastMessage("Status do pedido atualizado para " + status.ToString());
            return RedirectToAction("List", "Orders");
        }
    }
}
