using System;
using System.Threading.Tasks;

namespace TimeSheet.Application.DTOs.Client
{
    public class ClientDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }

        // Relationships

        public string CountryName { get; set; }

        // Constructors

        public ClientDTO() { }
        public ClientDTO(string name, string adress, string city, string zip)
        {
            Name = name;
            Address = adress;
            City = city;
            Zip = zip;
        }
    }
}
