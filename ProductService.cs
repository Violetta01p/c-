using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class ProductService
{
    private readonly ShopContext _context;

    public ProductService(ShopContext context)
    {
        _context = context;
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products.Include(p => p.Brand).ToList();
    }

    public Product? GetProductById(int id)
    {
        return _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == id);
    }

    public Product AddProduct(Product product)
    {
        try{

        _context.Products.Add(product);
        _context.SaveChanges(); 
        return product;}
        catch ( Exception ex){
            Console.WriteLine($"Помилка БД: {ex.Message}");
            throw;
        }
    }

    public bool DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }
}
