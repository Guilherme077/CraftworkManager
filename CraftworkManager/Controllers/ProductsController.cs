using CraftworkManager.Data;
using CraftworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CraftworkManager.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DBContext DbContext;

        public ProductsController(DBContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel viewModel)
        {
            var product = new Product
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                StandardCost = viewModel.StandardCost,
                StandardPrice = viewModel.StandardPrice,
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await DbContext.Products.AddAsync(product);
            await DbContext.SaveChangesAsync();

            return View();
        }
    }
}
