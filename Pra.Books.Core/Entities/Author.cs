
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
                if (value.Length > 100)
                    value = value.Substring(0, 100);
                name = value;
            }
        }

        public Author(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        internal Author(Guid id, string name) : this(name)
        {
            Id = id;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
