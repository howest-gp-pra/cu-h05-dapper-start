
namespace Pra.Books.Core.Entities
{
    public class Author
    {
        private string name;

        public Guid Id { get; internal set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Naam auteur kan niet leeg zijn");
                name = value;
            }
        }

        public Author(string name)
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
