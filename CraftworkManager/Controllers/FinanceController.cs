using CraftworkManager.Data;
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
            var lastTransactions = await DbContext.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).Take(10).ToListAsync();
            return View(lastTransactions);
        }

        [Authorize]
        [HttpGet]
        public IActionResult TransactionsHistory()
        {
            return View();
        }
        
    }
}
