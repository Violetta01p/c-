using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using C_.Application.DTOs;

namespace C_.Pages;

public class CreateModel : PageModel
{
    private readonly ProductService _service;

    [BindProperty]
    public CreateProductDto NewProduct { get; set; } = default!;

    public CreateModel(ProductService service)
    {
        _service = service;
    }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();

        var product = new Product
        {
            Name = NewProduct.Name,
            Price = NewProduct.Price,
            BrandId = NewProduct.BrandId
        };

        _service.AddProduct(product);
        return RedirectToPage("Index");
    }
}
