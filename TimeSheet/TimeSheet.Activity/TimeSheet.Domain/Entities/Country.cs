namespace TimeSheet.Domain.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Constructors

        public Country() { }
        public Country(string name)
        {
            Name = name;
        }
    }
}
