using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper; // Обов'язково для роботи IMapper
using C_.Application.DTOs; // Твій шлях до DTO

public class ProductService
{
    private readonly ShopContext _context;
    private readonly IMapper _mapper;

    // Конструктор тепер правильно приймає обидва сервіси
    public ProductService(ShopContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Отримуємо всі продукти та перетворюємо їх на DTO
    public List<ProductDto> GetAllProducts()
    {
        var products = _context.Products
            .Include(p => p.Brand) // Якщо у моделі Product є навігаційна властивість Brand
            .ToList();
            
        return _mapper.Map<List<ProductDto>>(products);
    }

    // Отримуємо один продукт за ID і мапимо в DTO
    public ProductDto? GetProductById(int id)
    {
        var product = _context.Products
            .Include(p => p.Brand)
            .FirstOrDefault(p => p.Id == id);
            
        return _mapper.Map<ProductDto>(product);
    }

    // Додавання продукту
    public ProductDto AddProduct(Product product)
    {
        try
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Помилка БД: {ex.Message}");
            throw;
        }
    }

    // Видалення продукту
    public bool DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }
}
      

