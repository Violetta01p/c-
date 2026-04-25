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

    [HttpGet]
    public ActionResult<List<ProductDto>> GetAll()
    {
        var products = _productService.GetAllProducts();
        
        var dtos = products.Select(p => new ProductDto(p.Id, p.Name ?? "", p.Price)).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null) return NotFound(new { message = "Продукт не знайдено" });
        
        return Ok(new ProductDto(product.Id, product.Name ?? "", product.Price));
    }

    [HttpPost]
    public ActionResult<ProductDto> Create([FromBody] CreateProductDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Name))
            return BadRequest(new { message = "Некоректні дані" });

        var product = new Product 
        { 
            Name = dto.Name, 
            Price = dto.Price,
            BrandId = dto.BrandId 
        };
        
        try {
            var created = _productService.AddProduct(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, 
                new ProductDto(created.Id, created.Name ?? "", created.Price));
        }
        catch (System.Exception) {
            return StatusCode(500, "Помилка бази даних. Перевірте, чи існує BrandId, який ви вказали!");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (_productService.DeleteProduct(id)) return Ok(new { message = "Видалено" });
        return NotFound();
    }
}