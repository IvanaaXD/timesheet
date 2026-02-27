using TimeSheet.Domain.Interfaces;

namespace TimeSheet.Domain.Entities
{
    public class Client : ISoftDelete
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public Boolean IsDeleted { get; set; } = false;

        // Relationships

        public Guid CountryId { get; set; }
        public virtual Country Country { get; set; }

        // Constructors

        public Client() { }
        public Client(string name, string adress, string city, string zip, Boolean isDeleted)
        {
            Name = name;
            Address = adress;
            City = city;
            Zip = zip;
            IsDeleted = isDeleted;
        }
    }
}
