using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ShopRepository
{
    private readonly ShopContext _context;

    public ShopRepository(ShopContext context)
    {
        _context = context;
    }

    // 1. CREATE (Додавання)
    public void AddBrand(Brand brand)
    {
        _context.Brands.Add(brand);
        _context.SaveChanges();
    }

    // 2. READ (Складний запит з Include - витягує одразу все)
    public List<Brand> GetAllBrandsWithDetails()
    {
        return _context.Brands
            .Include(b => b.Products)
                .ThenInclude(p => p.Reviews)
            .ToList();
    }

    // 3. READ (Фільтрація через LINQ)
    public List<Product> GetProductsCheaperThan(decimal maxPrice)
    {
        return _context.Products.Where(p => p.Price < maxPrice).ToList();
    }

    // 4. UPDATE (Оновлення)
    public void UpdateProductPrice(int productId, decimal newPrice)
    {
        var product = _context.Products.Find(productId);
        if (product != null)
        {
            product.Price = newPrice;
            _context.SaveChanges();
        }
    }

    // 5. DELETE (Видалення)
    public void DeleteReview(int reviewId)
    {
        var review = _context.Reviews.Find(reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }
    }
}
