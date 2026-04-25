using AutoMapper;
using C_.Application.DTOs;

namespace C_.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}

