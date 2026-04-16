using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // 1. Отримання всіх продуктів
    [HttpGet]
    public ActionResult<List<ProductDto>> GetAll()
    {
        var products = _productService.GetAllProducts();
        // Перетворюємо справжні Products на безпечні ProductDto
        var dtos = products.Select(p => new ProductDto(p.Id, p.Name)).ToList();
        return Ok(dtos);
    }

    // 2. Отримання продукту за ID
    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound(new { message = "Продукт не знайдено" });
        }
        return Ok(new ProductDto(product.Id, product.Name));
    }

    // 3. Створення нового продукту
    [HttpPost]
    public ActionResult<ProductDto> Create([FromBody] CreateProductDto newProductDto)
    {
        if (newProductDto == null || string.IsNullOrEmpty(newProductDto.Name))
        {
            return BadRequest(new { message = "Некоректні дані" });
        }

        // Створюємо сутність для БД на основі DTO
        var product = new Product { Name = newProductDto.Name };
        var createdProduct = _productService.AddProduct(product);

        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, new ProductDto(createdProduct.Id, createdProduct.Name));
    }

    // 4. НОВИЙ ЕНДПОІНТ: Видалення продукту
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var success = _productService.DeleteProduct(id);
        if (!success)
        {
            return NotFound(new { message = "Продукт не знайдено" });
        }
        return Ok(new { message = "Продукт успішно видалено" });
    }
}
