public class Product
{
    public int Id { get; set; } 
    public string Name { get; set; } 
    public int BrandId { get; set; }
    public Brand Brand { get; set; } 
}
