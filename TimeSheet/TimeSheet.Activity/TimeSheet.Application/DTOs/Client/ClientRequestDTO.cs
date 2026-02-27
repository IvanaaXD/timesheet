using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Client
{
    public class ClientRequestDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }

        // Relationships

        public Guid CountryId { get; set; }

        // Constructors

        public ClientRequestDTO() { }
        public ClientRequestDTO(string name, string adress, string city, string zip, Guid countryId)
        {
            Name = name;
            Address = adress;
            City = city;
            Zip = zip;
            CountryId = countryId;
        }
    }
}
