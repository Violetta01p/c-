using System.Collections.Generic;

public class Brand
{
    public int Id { get; set; }
    public string? Name { get; set; } 
    public int YearFounded { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}
