using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Country
{
    public class CountryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Constructors

        public CountryDTO() { }
        public CountryDTO(string name)
        {
            Name = name;
        }
    }
}
