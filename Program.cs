using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        using (var context = new ShopContext())
        {
            // Використовуємо наш новий репозиторій!
            var repo = new ShopRepository(context);

            Console.WriteLine("--- 1. СТВОРЕННЯ ДАНИХ ---");
            var myBrand = new Brand 
            { 
                Name = "Dior",
                Products = new List<Product>
                {
                    new Product 
                    { 
                        Name = "Perfume", Price = 3500m,
                        Reviews = new List<Review> { new Review { Text = "Amazing!", Rating = 5 } }
                    },
                    new Product { Name = "Lipstick", Price = 1200m }
                }
            };
            repo.AddBrand(myBrand);
            Console.WriteLine("Бренд, продукти та відгуки успішно додані!\n");

            Console.WriteLine("--- 2. СКЛАДНИЙ ЗАПИТ (INCLUDE) ---");
            var brands = repo.GetAllBrandsWithDetails();
            foreach (var b in brands)
            {
                Console.WriteLine($"Бренд: {b.Name}");
                foreach (var p in b.Products)
                {
                    Console.WriteLine($"  - {p.Name} ({p.Price} грн), Відгуків: {p.Reviews.Count}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("--- 3. ФІЛЬТРАЦІЯ (LINQ) ---");
            var cheapProducts = repo.GetProductsCheaperThan(2000m);
            Console.WriteLine("Продукти дешевші за 2000 грн:");
            foreach (var p in cheapProducts)
            {
                Console.WriteLine($"  * {p.Name} - {p.Price} грн");
            }
            Console.WriteLine();

            Console.WriteLine("--- 4. ОНОВЛЕННЯ ---");
            if (cheapProducts.Count > 0)
            {
                repo.UpdateProductPrice(cheapProducts[0].Id, 1500m);
                Console.WriteLine($"Ціну на {cheapProducts[0].Name} успішно оновлено!\n");
            }

            Console.WriteLine("--- 5. ВИДАЛЕННЯ ---");
            repo.DeleteReview(1);
            Console.WriteLine("Відгук видалено з бази.");
        }
    }
}

