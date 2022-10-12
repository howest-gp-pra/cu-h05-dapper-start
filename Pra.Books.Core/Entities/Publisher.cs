
namespace Pra.Books.Core.Entities
{
    public class Publisher
    {
        private string name;

        public Guid Id { get; internal set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Naam uitgeverij kan niet leeg zijn");
                if (value.Length > 100)
                    value = value.Substring(0, 100);
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
