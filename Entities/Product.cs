using System.Collections.Generic;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    
    public int BrandId { get; set; }
    public Brand? Brand { get; set; }
    public List<Review> Reviews { get; set; } = new();
}

