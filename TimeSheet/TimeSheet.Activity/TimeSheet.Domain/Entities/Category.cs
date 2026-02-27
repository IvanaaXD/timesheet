namespace TimeSheet.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Constructors

        public Category() {}
        public Category(string name)
        {
            Name = name;    
        }
    }
}
