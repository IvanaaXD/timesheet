using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TimeSheet.Application.Abstractions;
using TimeSheet.Application.DTOs.Category;
using TimeSheet.Application.Mappings;
using TimeSheet.Domain.Entities;
using TimeSheet.Domain.Interfaces;
using TimeSheet.Application.Common.Exceptions;
using FluentValidation;
using TimeSheet.Application.Common.Extensions;
using TimeSheet.Application.Validators;
using TimeSheetValidationException = TimeSheet.Application.Common.Exceptions.ValidationException;

namespace TimeSheet.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CategoryRequestDTO> _validator;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IValidator<CategoryRequestDTO> validator, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.FindCategoryByIdAsync(id);
            if (category == null) throw new NotFoundException($"Category with ID {id} not found.");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> GetCategoryByNameAsync(string name)
        {
            var category = await _categoryRepository.FindCategoryByNameAsync(name);
            if (category == null) throw new NotFoundException($"Category with name {name} not found.");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.FindAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);

        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryRequestDTO categoryRequestDTO)
        {
            await ValidateAndThrowAsync(_validator, categoryRequestDTO);

            var existing = await _categoryRepository.FindCategoryByNameAsync(categoryRequestDTO.Name);
            if (existing != null)
                throw new ConflictException($"Category with name '{categoryRequestDTO.Name}' already exists.");

            var category = _mapper.Map<Category>(categoryRequestDTO);

            var createdCatrgoty = await _categoryRepository.AddCategoryAsync(category);
            return _mapper.Map<CategoryDTO>(createdCatrgoty);
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(Guid id, CategoryRequestDTO categoryRequestDTO)
        {
            await ValidateAndThrowAsync(_validator, categoryRequestDTO);

            var existingCategory = await _categoryRepository.FindCategoryByIdAsync(id);
            if (existingCategory == null)
               throw new NotFoundException($"Category with ID {id} not found.");

            var categoryWithSameName = await _categoryRepository.FindCategoryByNameAsync(categoryRequestDTO.Name);

            if (categoryWithSameName != null && categoryWithSameName.Id != id)
            {
                throw new ConflictException($"Category with name '{categoryRequestDTO.Name}' already exists.");
            }

            _mapper.Map(categoryRequestDTO, existingCategory);

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return _mapper.Map<CategoryDTO>(existingCategory);
        }
    }
}
