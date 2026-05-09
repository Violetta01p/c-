using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using C_.Application.DTOs;

public class ProductService
{
    private readonly ShopContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;
    private readonly IMemoryCache _cache;

    private const string CACHE_KEY = "products_list";

    public ProductService(
        ShopContext context,
        IMapper mapper,
        ILogger<ProductService> logger,
        IMemoryCache cache)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    // 🔹 GET ALL (з кешем)
    public List<ProductDto> GetAllProducts()
    {
        _logger.LogInformation("Запит на отримання всіх продуктів");

        // 🔥 cache hit
        if (_cache.TryGetValue(CACHE_KEY, out List<ProductDto> cachedProducts))
        {
            _logger.LogInformation("CACHE HIT: дані взяті з кешу. Кількість: {count}", cachedProducts.Count);
            return cachedProducts;
        }

        _logger.LogInformation("CACHE MISS: дані беруться з БД");

        var products = _context.Products
            .Include(p => p.Brand)
            .ToList();

        var result = _mapper.Map<List<ProductDto>>(products);

        // запис у кеш
        _cache.Set(CACHE_KEY, result, TimeSpan.FromSeconds(30));

        _logger.LogInformation("Дані збережені в кеш. Кількість: {count}", result.Count);

        return result;
    }

    // 🔹 GET BY ID
    public ProductDto? GetProductById(int id)
    {
        _logger.LogInformation("Отримання продукту за ID: {id}", id);

        var product = _context.Products
            .Include(p => p.Brand)
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Продукт з ID {id} не знайдено", id);
            return null;
        }

        return _mapper.Map<ProductDto>(product);
    }

    // 🔹 CREATE
    public ProductDto AddProduct(Product product)
    {
        try
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            _logger.LogInformation("Створено продукт ID: {id}, Name: {name}", product.Id, product.Name);

            // ❗ інвалідація кешу
            _cache.Remove(CACHE_KEY);
            _logger.LogInformation("Кеш очищено після створення продукту");

            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка при створенні продукту");
            throw;
        }
    }

    // 🔹 DELETE
    public bool DeleteProduct(int id)
    {
        _logger.LogInformation("Спроба видалення продукту ID: {id}", id);

        var product = _context.Products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            _logger.LogWarning("Продукт для видалення не знайдено ID: {id}", id);
            return false;
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        _logger.LogInformation("Продукт видалено ID: {id}", id);

        // ❗ інвалідація кешу
        _cache.Remove(CACHE_KEY);
        _logger.LogInformation("Кеш очищено після видалення");

        return true;
    }
}

