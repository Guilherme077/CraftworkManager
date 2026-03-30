using CraftworkManager.Data;
using CraftworkManager.Models;
using CraftworkManager.Models.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CraftworkManager.Controllers
{
    public class FinanceController : Controller
    {

        private readonly DBContext DbContext;
        public FinanceController(DBContext dBContext) { DbContext = dBContext; }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var lastTransactions = await DbContext.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToListAsync();

            var allTransactions = await DbContext.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthTransactions = allTransactions
                .Where(t => t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .ToList();

            var shipmentIds = await DbContext.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income && t.ShipmentId.HasValue)
                .Select(t => t.ShipmentId.Value)
                .ToListAsync();

            var shipmentsWithoutIncome = await DbContext.Shipments
                .Where(s => s.Status != ShipmentStatus.Cancelled && !shipmentIds.Contains(s.Id) && (s.DeliveredOn.HasValue? s.DeliveredOn > DateTime.Today.AddDays(-14) : true) && s.Order.userId == userId)
                .Include(s => s.Order)
                .Include(s => s.Order.OrderItems)
                .ThenInclude(oi => oi.BaseProduct)
                .ToListAsync();

            var dashboardData = new FinanceDashboardViewModel
            {
                LastTransactions = lastTransactions,
                TotalProfit = allTransactions.Sum(t => t.GetSignedAmount()),
                MonthlyIncome = monthTransactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount),
                MonthlyExpenses = monthTransactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount),
                ShipmentsWithoutIncome = shipmentsWithoutIncome
            };

            return View(dashboardData);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TransactionsHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await DbContext.Transactions.Where(t => t.UserId == userId).Include(t => t.Shipment).ThenInclude(s => s.Order).ToListAsync();
            return View(transactions);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddExpense()
        {
            Transaction expenseTemplate = new Transaction()
            {
                Type = TransactionType.Expense,
                Date = DateTime.Now
            };
            return View(expenseTemplate);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddIncome(Guid? shipmentId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shipment = await DbContext.Shipments.Where(s => s.Order.userId == userId).Include(s => s.Order).Include(s => s.Order.OrderItems).ThenInclude(oi => oi.BaseProduct).FirstOrDefaultAsync(s => s.Id == shipmentId);
            var shipmentIds = await DbContext.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income && t.ShipmentId.HasValue)
                .Select(t => t.ShipmentId)
                .ToListAsync();

            ViewBag.PendingShipments = DbContext.Shipments.Include(s => s.Order).Where(s => s.Order.userId == userId && s.Status != ShipmentStatus.Cancelled && !shipmentIds.Contains(s.Id)).Include(s => s.Order.OrderItems).ThenInclude(oi => oi.BaseProduct).Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.getFullDescription()
            });
            var incomeTemplate = new Transaction()
            {
                Description = shipment != null ? $"Venda de {shipment.Order.getSmallDescription()} para {shipment.Order.ClientName}" : "",
                Shipment = shipment,
                ShipmentId = shipmentId,
                Type = TransactionType.Income,
                Date = DateTime.Now,
                Amount = shipment != null ? shipment.getTotalPrice() : 0
            };
            return View(incomeTemplate);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTransaction(Transaction transaction)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            transaction.UserId = userId;
            if (transaction.ShipmentId.HasValue)
            {
                var shipment = await DbContext.Shipments.Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == transaction.ShipmentId.Value);
                if (shipment.Order.userId != userId)
                {
                    return BadRequest("Invalid shipment selected.");
                }
            }
            DbContext.Transactions.Add(transaction);
            await DbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
