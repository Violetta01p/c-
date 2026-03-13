using System;
using System.Linq;

class Program
{
    static void Main()
    {
        using ShopContext db = new ShopContext();
        db.Database.EnsureCreated();

        Console.WriteLine("Базу створено! Починаємо роботу.\n");

        // 1. Створення 
        Brand myBrand = new Brand { Name = "NYX" };
        Product myProduct = new Product { Name = "Lipstick", Brand = myBrand };
        
        db.Brands.Add(myBrand); 
        db.Products.Add(myProduct); 
        db.SaveChanges(); 
        Console.WriteLine("Створено: додано бренд NYX та помаду.");

        // 2. Читання 
        Product savedProduct = db.Products.First(); 
        Console.WriteLine($"Прочитано з бази: {savedProduct.Name}");

        // 3. Оновлення 
        savedProduct.Name = "Red Lipstick"; 
        db.SaveChanges(); 
        Console.WriteLine($"Оновлено: назву змінено на {savedProduct.Name}");

        // 4. Видалення 
        db.Products.Remove(savedProduct); 
        db.SaveChanges(); 
        Console.WriteLine("Видалено: продукт видалено з бази.");
    }
}
