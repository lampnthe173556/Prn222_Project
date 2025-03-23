﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcormerProjectPRN222.Models;

namespace EcormerProjectPRN222.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : AdminBaseController
    {
        private readonly MyProjectClothingContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(MyProjectClothingContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Products";
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new {
                    p.ProductId,
                    p.ProductName,
                    p.Price,
                    p.Img,
                    p.Description,
                    p.Status,
                    p.CategoryId,
                    CategoryName = p.Category.CategoryName
                })
                .ToListAsync();
            return Json(new { data = products });
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }
            return Json(new { success = true, data = product });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Price,Description,CategoryId,Status")] Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        
                        product.Img = "/uploads/products/" + uniqueFileName;
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product created successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error creating product" });
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ProductId,ProductName,Price,Description,CategoryId,Status")] Product product, IFormFile? imageFile = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                    if (existingProduct == null)
                    {
                        return Json(new { success = false, message = "Product not found" });
                    }

                    // Create new product with existing properties
                    var updatedProduct = new Product
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        Status = product.Status,
                        Img = existingProduct.Img // Preserve existing image path
                    };

                    // Only update image if a new one is provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingProduct.Img))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.Img.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        
                        updatedProduct.Img = "/uploads/products/" + uniqueFileName;
                    }

                    _context.Entry(updatedProduct).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Product updated successfully" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return Json(new { success = false, message = "Product not found" });
                    }
                    throw;
                }
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            try
            {
                if (!string.IsNullOrEmpty(product.Img))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.Img.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error deleting product" });
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
