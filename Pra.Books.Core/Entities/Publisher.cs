
namespace Pra.Books.Core.Entities
{
    public class Publisher
    {
        private string name;

        public Guid Id { get; }

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Naam uitgeverij kan niet leeg zijn");
                name = value;
            }
        }

        public Publisher(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
