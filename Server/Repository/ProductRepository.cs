using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _context;

        public ProductRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Get all products (Synchronous)
    

        // Get all products 
        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

     

        // Get product by ID 
        public async Task<Product> GetById(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        // Add product
        public async Task Add(Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Invalid product data.");
            }

            var existingProduct = await _context.Products
                .AnyAsync(p => p.Name == product.Name);

            if (existingProduct)
            {
                throw new Exception("A product with the same name already exists.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        

        // Update product
        public async Task Update(Product product)
        {
            var existingProduct = await _context.Products.SingleOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
             _context.Products.Update(existingProduct);
           _context.SaveChanges();
        }

        public async Task<List<Product>> Search(string searchTerm)
        {
            var searchTermLower = searchTerm.ToLower();
            return await _context.Products
                .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{searchTermLower}%") || EF.Functions.Like(p.Description.ToLower(), $"%{searchTermLower}%"))
                .ToListAsync();
        }
    }
}
