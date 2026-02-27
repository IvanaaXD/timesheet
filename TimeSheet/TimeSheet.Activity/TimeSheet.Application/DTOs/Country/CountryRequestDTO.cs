using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Country
{
    public class CountryRequestDTO
    {
        public string Name { get; set; }

        // Constructors

        public CountryRequestDTO() { }
        public CountryRequestDTO(string name)
        {
            Name = name;
        }
    }
}