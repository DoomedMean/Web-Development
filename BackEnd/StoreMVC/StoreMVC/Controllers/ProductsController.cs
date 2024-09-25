using Microsoft.AspNetCore.Mvc;
using StoreMVC.Models;
using StoreMVC.Services;

namespace StoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;
		private readonly IWebHostEnvironment environment;

		public ProductsController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
			this.environment = environment;
		}

        public IActionResult Index()
        {
            var products = _context.Products.OrderByDescending(p => p.Id).ToList();

            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }

		[HttpPost] public IActionResult Create(ProductDto productDto)
		{
            if(productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The Image File is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //Save image file to root
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //Save to database
            Product product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreateAt = DateTime.Now
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Products");
		}

		public IActionResult Edit(int id)
		{
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Produts");
            }

            // Create ProductsDto from Product
            var productdto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");

			return View(productdto);
		}

        [HttpPost]
		public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
				ViewData["ProductId"] = product.Id;
				ViewData["ImageFileName"] = product.ImageFileName;
				ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");
				return View(productDto);
            }

            //Update image if not null
            string newFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //Delete old file
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            //Update Product to database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            _context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete (int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            _context.Products.Remove(product);
            _context.SaveChanges(true);
            return RedirectToAction("Index", "Products");
        }
	}
}
