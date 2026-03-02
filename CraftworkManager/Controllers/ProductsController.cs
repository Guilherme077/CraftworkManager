using CraftworkManager.Data;
using CraftworkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var products = await DbContext.Products.Where(p => p.userId == userId).ToListAsync();

            return View(products);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await DbContext.Products.FindAsync(id);

            if(product is not null && product.userId == userId){
                return View(product);
            }else { 
                return View(null);
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Product viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await DbContext.Products.FindAsync(viewModel.Id);

            if(product is not null && product.userId == userId)
            {
                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.StandardCost = viewModel.StandardCost;
                product.StandardPrice = viewModel.StandardPrice;

                await DbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Products");
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetProductInfo(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = DbContext.Products
            .Where(p => p.Id == productId && p.userId == userId)
            .Select(p => new
            {
                price = p.StandardPrice,
                cost = p.StandardCost,
                description = p.Description,
                name = p.Name

            })
            .FirstOrDefault();

            if(product is null)
            {
                return NotFound();
            }

            return Json(product);
        }
    }
}
