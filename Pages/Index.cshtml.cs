using Microsoft.AspNetCore.Mvc.RazorPages;
using C_.Application.DTOs;
using System.Collections.Generic;

namespace C_.Pages;

public class IndexModel : PageModel
{
    private readonly ProductService _service;
    public List<ProductDto> Products { get; set; } = new();

    public IndexModel(ProductService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        Products = _service.GetAllProducts();
    }
}
