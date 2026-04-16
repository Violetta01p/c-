using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
    public ActionResult<List<Product>> GetAll()
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _productService.GetProductById(id);
        if (product == null)
        {
            return NotFound(new { message = "Продукт не знайдено" });
        }
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create([FromBody] Product newProduct)
    {
        if (newProduct == null || string.IsNullOrEmpty(newProduct.Name))
        {
            return BadRequest(new { message = "Некоректні дані" });
        }

        var createdProduct = _productService.AddProduct(newProduct);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }
}
