using AutoMapper;
using Bookstore.Models.Models;
using Bookstore.Models.Models.ViewModels;

namespace Bookstore.Views.Utilities
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EditCreateVM, Category>().ReverseMap();
            CreateMap<ProductVM, Product>().ReverseMap();
            CreateMap<Product, HomeVM>().ReverseMap();
            CreateMap<CompanyVM, Company>().ReverseMap();
           
        }
    }
}