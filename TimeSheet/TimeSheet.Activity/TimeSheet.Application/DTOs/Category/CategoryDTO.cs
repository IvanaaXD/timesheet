using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Category
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Constructors

        public CategoryDTO() { }
        public CategoryDTO(string name)
        {
            Name = name;
        }
    }
}
