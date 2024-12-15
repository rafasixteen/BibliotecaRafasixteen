using SQLite;

namespace BibliotecaRafasixteen
{
    public class Author
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = null!;
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
        public List<Author> Authors { get; set; } = null!;
    }

    public class Publisher
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public required string Name { get; set; }

        [Ignore]
        public List<Book> Books { get; set; } = null!;
    }

    public class BookAuthor
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }
    }
}