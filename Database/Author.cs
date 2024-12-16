using SQLite;

namespace BibliotecaRafasixteen
{
    public class Author
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = new();

        public void CopyFrom(Author author)
        {
            Id = author.Id;
            Name = author.Name;
            Books = author.Books.Select(book => book.Clone()).ToList();
        }

        public Author Clone()
        {
            return new Author
            {
                Id = Id,
                Name = Name,
                Books = Books.Select(book => book.Clone()).ToList()
            };
        }
    }

    public class Book
    {
        [PrimaryKey]
        public required string ISBN { get; set; }

        public required string Title { get; set; }

        public int PublisherId { get; set; }

        [Ignore]
        public Publisher Publisher { get; set; } = null!;

        [Ignore]
        public List<Author> Authors { get; set; } = new();

        public string AuthorsString
        {
            get => string.Join(", ", Authors.Select(author => author.Name));
            set => Authors = value.Split(',').Select(name => new Author { Name = name.Trim() }).ToList();
        }

        public static bool IsValidISBN13(string isbn)
        {
            // Remove all hyphens.
            string cleanIsbn = isbn.Replace("-", "");

            if (cleanIsbn.Length != 13 || !cleanIsbn.All(char.IsDigit))
                return false;

            // ISBN-13 checksum validation.
            int sum = 0;

            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(cleanIsbn[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            int checksum = (10 - (sum % 10)) % 10;

            return checksum == int.Parse(cleanIsbn[12].ToString());
        }

        public void CopyFrom(Book book)
        {
            ISBN = book.ISBN;
            Title = book.Title;
            PublisherId = book.PublisherId;
            Publisher = book.Publisher.Clone();
            Authors = book.Authors.Select(author => author.Clone()).ToList();
        }

        public Book Clone()
        {
            return new Book
            {
                ISBN = ISBN,
                Title = Title,
                PublisherId = PublisherId,
                Publisher = Publisher.Clone(),
                Authors = Authors.Select(author => author.Clone()).ToList()
            };
        }
    }

    public class Publisher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = new();

        public void CopyFrom(Publisher publisher)
        {
            Id = publisher.Id;
            Name = publisher.Name;
            Books = publisher.Books.Select(book => book.Clone()).ToList();
        }

        public Publisher Clone()
        {
            return new Publisher
            {
                Id = Id,
                Name = Name,
                Books = Books.Select(book => book.Clone()).ToList()
            };
        }
    }

    public class BookAuthor
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }
    }
}