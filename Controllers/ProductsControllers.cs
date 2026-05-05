using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System; // Обов'язково для роботи Exception
using C_.Application.DTOs; // Щоб бачив ProductDto

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
        // Тепер сервіс сам повертає List<ProductDto>, ручне маплення не потрібне!
        var dtos = _productService.GetAllProducts();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        // Сервіс сам повертає ProductDto
        var dto = _productService.GetProductById(id);
        if (dto == null) return NotFound(new { message = "Продукт не знайдено" });
      
        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<ProductDto> Create([FromBody] CreateProductDto dto)
    {
        // Тепер ми кидаємо ArgumentException, а наш ApiExceptionFilter його автоматично спіймає і поверне 400 BadRequest!
        if (dto == null || string.IsNullOrEmpty(dto.Name))
            throw new ArgumentException("Ім'я продукту не може бути порожнім.");

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            BrandId = dto.BrandId
        };
      
        // Ми прибрали try-catch! Якщо буде помилка БД, її перехопить наш ExceptionHandlingMiddleware.
        var createdDto = _productService.AddProduct(product);
        return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (_productService.DeleteProduct(id)) return Ok(new { message = "Видалено" });
        return NotFound();
    }

    // ========================================================
    // МЕТОД ДЛЯ ДЕМОНСТРАЦІЇ ПРАКТИЧНОЇ №13 
    // ========================================================
    [HttpGet("test-errors/{type}")]
    public IActionResult TestErrors(int type)
    {
        // Імітація 1: Некоректний параметр (спіймає ApiExceptionFilter -> поверне 400)
        if (type == 1) 
            throw new ArgumentException("Ціна товару не може бути від'ємною!");

        // Імітація 2: Порушення бізнес-правила (спіймає ApiExceptionFilter -> поверне 400)
        if (type == 2) 
            throw new InvalidOperationException("Неможливо видалити товар, бо він вже у кошику клієнта!");

        // Імітація 3: Системна помилка БД (спіймає ExceptionHandlingMiddleware -> поверне 500)
        if (type == 3) 
            throw new Exception("Відмова доступу до бази даних SQLite.");

        return Ok("Все працює без помилок. Виберіть тип помилки від 1 до 3.");
    }
}
