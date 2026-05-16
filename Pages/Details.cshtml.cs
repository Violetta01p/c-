using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using C_.Application.DTOs;

namespace C_.Pages;

public class DetailsModel : PageModel
{
    private readonly ProductService _service;

    public ProductDto Product { get; set; }

    public DetailsModel(ProductService service)
    {
        _service = service;
    }

    // Отримуємо ID з URL
    public IActionResult OnGet(int id)
    {
        Product = _service.GetProductById(id);
        
        if (Product == null)
        {
            return NotFound(); // Якщо товару немає, повертаємо 404
        }
        
        return Page();
    }
}