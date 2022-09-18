using Pra.Books.Core.Entities;
using Pra.Books.Core.Interfaces;

namespace Pra.Books.Core.Services
{
    public class BookServiceMem : IBookService
    {
        private List<Author> authors;
        private List<Publisher> publishers;
        private List<Book> books;

        public IEnumerable<Author> Authors
        {
            get { return authors.AsReadOnly(); }
        }

        public IEnumerable<Publisher> Publishers
        {
            get { return publishers.AsReadOnly(); }
        }

        public BookServiceMem()
        {
            Seeding();
        }

        private void Seeding()
        {
            authors = new List<Author>
            {
                new Author ("Elsschot Willem"),
                new Author ("Boon Louis-Paul"),
                new Author ("Claus Hugo"),
                new Author ("Lanoye Tom"),
                new Author ("Zinzen Walter"),
                new Author ("Tuchman Barbara"),
                new Author ("Christie Agatha"),
                new Author ("Van Reybrouck David"),
                new Author ("Pauwels Jan"),
                new Author ("Konrad György"),
                new Author ("Breemeersch Koen"),
                new Author ("Jennings Roger"),
                new Author ("Meyer Deon"),
                new Author ("Jordan Camille"),
                new Author ("Swan Tom"),
                new Author ("Cook Robin"),
                new Author ("Brown Dan"),
                new Author ("Van Wittenberghe Annelies"),
                new Author ("De Vos Danny"),
                new Author ("Brusselmans Herman"),
                new Author ("Van Aar Hetty")
            };

            publishers = new List<Publisher>
            {
                new Publisher("Hadewijch"),
                new Publisher("Querido"),
                new Publisher("Arbeiderspers"),
                new Publisher("De Bezige Bij"),
                new Publisher("Prometheus"),
                new Publisher("QUE"),
                new Publisher("Academic Service"),
                new Publisher("Casterman"),
                new Publisher("AW Bruna"),
                new Publisher("Plantyn"),
                new Publisher("Luttingh"),
                new Publisher("Prometheus")
            };

            books = new List<Book>
            {
                new Book("Kaas"  ,                              authors[0],     publishers[1],  1953),
                new Book("Jan De Lichte",                       authors[1],     publishers[2],  1962),
                new Book("Geuzenboek",                          authors[1],     publishers[2],  1964),
                new Book("Het verdriet van België",             authors[2],     publishers[3],  1983),
                new Book("De geruchten",                        authors[3],     publishers[3],  1996),
                new Book("De koele minnaar",                    authors[2],     publishers[3],  1970),
                new Book("Kartonnen dozen",                     authors[3],     publishers[4],  1993),
                new Book("Mobotu",                              authors[4],     publishers[0],  1995),
                new Book("Programmeren met Turbo Pascal",       authors[14],    publishers[6],  1995),
                new Book("Programmeren met C++",                authors[14],    publishers[6],  1996),
                new Book("Programmeren met LISP",               authors[14],    publishers[6],  1995),
                new Book("Gestructureerde Analyse",             authors[13],    publishers[6],  1989),
                new Book("OO software ontwerp",                 authors[13],    publishers[6],  1992),
                new Book("Compleet handboek Access 97",         authors[11],    publishers[5],  1997),
                new Book("Compleet handboek Access 2000",       authors[11],    publishers[5],  1999),
                new Book("Compleet handboek Access 95",         authors[11],    publishers[5],  1995),
                new Book("Tuinieren voor beginners",            authors[10],    publishers[4],  1999),
                new Book("Tuinieren voor gevorderden",          authors[10],    publishers[3],  1999),
                new Book("Afrikaanse tuinen",                   authors[10],    publishers[3],  2000),
                new Book("Vreemd Lichaam",                      authors[15],    publishers[8],  2008),
                new Book("Het Juvenalis Dilemma",               authors[16],    publishers[10], 1998),
                new Book("Het verloren symbool",                authors[16],    publishers[10], 2009),
                new Book("Revolusi",                            authors[7],     publishers[3],  2020),
                new Book("Congo",                               authors[7],     publishers[3],  2010),
                new Book("Maanlicht van een andere planeet",    authors[19],    publishers[4],  2010)
            };
        }

        public bool AddAuthor(Author author)
        {
            authors.Add(author);
            return true;
        }

        public bool AddPublisher(Publisher publisher)
        {
            publishers.Add(publisher);
            return true;
        }

        public bool AddBook(Book book)
        {
            books.Add(book);
            return true;
        }

        public bool DeleteAuthor(Author author)
        {
            if(!authors.Contains(author) || IsAuthorInUse(author))
                return false;
            authors.Remove(author);
            return true;
        }

        public bool DeletePublisher(Publisher publisher)
        {
            if (!publishers.Contains(publisher) || IsPublisherInUse(publisher))
                return false;
            publishers.Remove(publisher);
            return true;
        }

        public bool DeleteBook(Book book)
        {
            if (!books.Contains(book))
                return false;
            books.Remove(book);
            return true;
        }

        public IEnumerable<Book> GetBooks(Author author = null, Publisher publisher = null)
        {
            IEnumerable<Book> filteredBooks = books.AsReadOnly();

            if(author != null)
                filteredBooks = FilterOnAuthor(filteredBooks, author);
            if(publisher != null)
                filteredBooks = FilterOnPublisher(filteredBooks, publisher);

            return filteredBooks;
        }

        private IEnumerable<Book> FilterOnAuthor(IEnumerable<Book> books, Author author)
        {
            List<Book> filtered = new List<Book>();
            foreach(Book book in books)
            {
                if(book.Author == author)
                {
                    filtered.Add(book);
                }
            }
            return filtered;
        }

        private IEnumerable<Book> FilterOnPublisher(IEnumerable<Book> books, Publisher publisher)
        {
            List<Book> filtered = new List<Book>();
            foreach (Book book in books)
            {
                if (book.Publisher == publisher)
                {
                    filtered.Add(book);
                }
            }
            return filtered;
        }

        public bool IsAuthorInUse(Author author)
        {
            foreach (Book book in books)
            {
                if (book.Author == author)
                    return true;
            }
            return false;
        }

        public bool IsPublisherInUse(Publisher publisher)
        {
            foreach (Book book in books)
            {
                if (book.Publisher == publisher)
                    return true;
            }
            return false;
        }

        public bool UpdateAuthor(Author author)
        {
            return true;
        }

        public bool UpdatePublisher(Publisher publisher)
        {
            return true;
        }

        public bool UpdateBook(Book book)
        {
            return true;
        }
    }
}
