using CraftworkManager.Data;
using CraftworkManager.Models;
using CraftworkManager.Models.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income)
                .Select(t => t.Shipment.Id)
                .ToListAsync();

            var shipmentsWithoutIncome = await DbContext.Shipments
                .Where(s => s.Status != ShipmentStatus.Cancelled && !shipmentIds.Contains(s.Id))
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
        public IActionResult TransactionsHistory()
        {
            return View();
        }
    }
}
