using CraftworkManager.Data;
using CraftworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var order = await DbContext.Orders.FirstOrDefaultAsync(p => p.Id == id && p.userId == userId);

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
            var orders = await DbContext.Orders.Where(p => p.userId == userId).OrderBy(p => p.DeadlineOn).ToListAsync();
            return View(orders);
        }
    }
}
