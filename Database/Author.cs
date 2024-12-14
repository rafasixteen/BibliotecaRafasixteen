using SQLite;

namespace BibliotecaRafasixteen
{
    public class Author
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = new();
    }

    public class Book
    {
        [PrimaryKey]
        public string ISBN { get; set; }

        public string Title { get; set; }

        public int PublisherId { get; set; }

        [Ignore]
        public Publisher Publisher { get; set; }

        [Ignore]
        public List<Author> Authors { get; set; } = new();
    }

    public class Publisher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = new();
    }

    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
    }
}