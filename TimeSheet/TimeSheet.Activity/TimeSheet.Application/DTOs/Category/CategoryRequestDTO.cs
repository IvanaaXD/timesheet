using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Category
{
    public class CategoryRequestDTO
    {
        public string Name { get; set; }

        // Constructors

        public CategoryRequestDTO() { }
        public CategoryRequestDTO(string name)
        {
            Name = name;
        }
    }
}