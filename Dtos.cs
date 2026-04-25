public record ProductDto(int Id, string Name, decimal Price);
public record CreateProductDto(string Name, decimal Price, int BrandId);