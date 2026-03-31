using System;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Починаємо роботу з міграціями!\n");

        using (ShopContext db = new ShopContext())
        {
            Console.WriteLine("--- ДОДАВАННЯ ---");
            Brand myBrand = new Brand { Name = "L'Oreal", YearFounded = 1909 };
            Product myProduct = new Product { Name = "Mascara", Price = 450.50m, Brand = myBrand };
            
            Console.WriteLine($"Стан продукту ДО Add: {db.Entry(myProduct).State}"); 
            
            db.Brands.Add(myBrand);
            db.Products.Add(myProduct);
            
            Console.WriteLine($"Стан продукту ПІСЛЯ Add: {db.Entry(myProduct).State}"); 
            
            db.SaveChanges(); 
            
            Console.WriteLine($"Стан продукту ПІСЛЯ SaveChanges: {db.Entry(myProduct).State}\n"); 
        }

        using (ShopContext db = new ShopContext())
        {
            Console.WriteLine("--- ОНОВЛЕННЯ ---");
            Product savedProduct = db.Products.First();
            Console.WriteLine($"Знайдено: {savedProduct.Name}, Ціна: {savedProduct.Price} грн");
            
            savedProduct.Price = 500.00m; 
            
            Console.WriteLine($"Стан після зміни ціни: {db.Entry(savedProduct).State}");
            db.SaveChanges(); 
            Console.WriteLine("Ціну успішно оновлено у базі!\n");
        }

        using (ShopContext db = new ShopContext())
        {
            Console.WriteLine("--- ВИДАЛЕННЯ ---");
            Product productToDelete = db.Products.First();
            
            db.Products.Remove(productToDelete);
            Console.WriteLine($"Стан після Remove: {db.Entry(productToDelete).State}");
            
            db.SaveChanges(); 
            Console.WriteLine("Продукт видалено з бази.");
        }
    }
}

