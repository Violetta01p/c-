using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using C_.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
//using C_.Domain.Entities; // Додано, щоб клас Product не світився червоним

namespace C_.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // 1. Отримання всіх (Стандартний GET)
    [HttpGet]
    public ActionResult<List<ProductDto>> GetAll()
    {
        return Ok(_productService.GetAllProducts());
    }

    // 2. Отримання за ID з обмеженням маршруту (Route Constraint: :int)
    [HttpGet("{id:int}")] 
    public ActionResult<ProductDto> GetById(int id)
    {
        var dto = _productService.GetProductById(id);
        if (dto == null) return NotFound(new { message = "Продукт не знайдено" });
        return Ok(dto);
    }

    // 3. Пошук через Query String (api/products/search?name=phone&maxPrice=500)
    [HttpGet("search")]
    public ActionResult<List<ProductDto>> Search([FromQuery] string? name, [FromQuery] decimal? maxPrice)
    {
        return Ok(new { Message = $"Пошук: {name}, макс. ціна: {maxPrice}" });
    }

    // 4. Створення: Захищено ПОЛІТИКОЮ (тільки ті, хто має claim "permission=edit")
    [Authorize(Policy = "CanEdit")] 
    [HttpPost]
    public ActionResult<ProductDto> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product 
        {
            Name = dto.Name,
            Price = dto.Price,
            BrandId = dto.BrandId
        };
      
        var createdDto = _productService.AddProduct(product);
        return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
    }

    // 5. Видалення: Захищено РОЛЛЮ (тільки Admin або Manager)
    [Authorize(Roles = "Admin,Manager")] 
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        if (_productService.DeleteProduct(id)) return Ok(new { message = "Видалено" });
        return NotFound();
    }
}