
namespace Pra.Books.Core.Entities
{
    public class Book
    {
        private string title;
        private Author author;
        private Publisher publisher;

        public Guid Id { get; internal set; }

        public string Title
        {
            get { return title; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Titel boek kan niet leeg zijn");
                title = value;
            }
        }

        public int Year { get; set; }

        public Author Author
        { 
            get { return author; }
            set
            {
                if(value == null)
                {
                    throw new Exception("Auteur is verplicht op te geven");
                }
                author = value;
            }
        }

        public Publisher Publisher 
        { 
            get { return publisher; }
            set
            {
                if(value == null)
                {
                    throw new Exception("Uitgeverij is verplicht op te geven");
                }
                publisher = value;
            }
        }

        public Book(string title, Author author, Publisher publisher, int year)
        {
            Id = Guid.NewGuid();
            Title = title;
            Author = author;
            Publisher = publisher;
            Year = year;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
