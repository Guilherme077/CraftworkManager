using CraftworkManager.Data;
using CraftworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace CraftworkManager.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DBContext DbContext;
        public OrdersController(DBContext dBContext) { DbContext = dBContext; }

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
                Text = s.ToString()
            });

            return View(order);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Order viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await DbContext.Orders.FindAsync(viewModel.Id);

            if (order is not null && order.userId == userId)
            {
                order.DeadlineOn = viewModel.DeadlineOn;
                order.ClientName = viewModel.ClientName;
                order.ClientAddress = viewModel.ClientAddress;
                order.PaymentWay= viewModel.PaymentWay;
                order.Url = viewModel.Url;
                order.WithNF = viewModel.WithNF;
                order.LastUpdateOn = DateTime.Now;

                await DbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", "Orders", new { id = order.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await DbContext.Orders.Where(p => p.userId == userId).Include(o => o.OrderItems).ThenInclude(oi => oi.BaseProduct).OrderBy(p => p.DeadlineOn).ToListAsync();
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
    }
}
