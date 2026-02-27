using AutoMapper;
using TimeSheet.Application.DTOs.Category;
using TimeSheet.Domain.Entities;

namespace TimeSheet.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryRequestDTO, Category>();
        }
    }
}