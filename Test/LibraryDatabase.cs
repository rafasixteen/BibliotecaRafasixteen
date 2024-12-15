using SQLite;

namespace BibliotecaRafasixteen
{
    public class LibraryDatabase(string databasePath)
    {
        private readonly SQLiteConnection _connection = new(databasePath);

        public string DatabasePath => _connection.DatabasePath;

        public Publisher GetPublisherByName(string name)
        {
            const string k_query = "select * from Publishers where Name = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, name);
            Publisher? result = command.ExecuteQuery<Publisher>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Publisher with name '{name}' doesn't exist in the database.");

            return result;
        }

        public Publisher GetPublisherById(int id)
        {
            const string k_query = "select * from Publishers where Id = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, id);
            Publisher? result = command.ExecuteQuery<Publisher>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Publisher with Id '{id}' doesn't exist in the database.");

            return result;
        }

        public Author GetAuthorByName(string name)
        {
            const string k_query = "select * from Authors where Name = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, name);
            Author? result = command.ExecuteQuery<Author>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Author with name '{name}' doesn't exist in the database.");

            return result;
        }

        public List<Author> GetAuthorsOf(string bookIsbn)
        {
            const string k_query = "select * from Authors " +
                    "inner join BookAuthor on Authors.Id = BookAuthor.AuthorId " +
                    "where BookAuthor.BookISBN = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, bookIsbn);
            return command.ExecuteQuery<Author>();
        }

        public Book GetBookByIsbn(string isbn)
        {
            const string k_query = "select * from Books where ISBN = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn);
            Book? result = command.ExecuteQuery<Book>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Book with ISBN '{isbn}' doesn't exist in the database.");

            return result;
        }

        public Publisher InsertPublisher(string publisherName)
        {
            if (PublisherExists(publisherName))
                throw new ArgumentException($"Publisher with name '{publisherName}' already exists in the database.");

            const string k_query = "insert into Publishers (Name) values (?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, publisherName);
            command.ExecuteNonQuery();

            return GetPublisherByName(publisherName);
        }

        public Author InsertAuthor(string authorName)
        {
            if (AuthorExists(authorName))
                throw new ArgumentException($"Author with name '{authorName}' already exists in the database.");

            const string k_query = "insert into Authors (Name) values (?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, authorName);
            command.ExecuteNonQuery();

            return GetAuthorByName(authorName);
        }

        public Book InsertBook(string isbn, string tittle, int publisherId)
        {
            if (BookExists(isbn))
                throw new ArgumentException($"Book with ISBN '{isbn}' already exists in the database.");

            const string k_query = "insert into Books(ISBN, Title, PublisherId) values (?, ?, ?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn, tittle, publisherId);
            command.ExecuteNonQuery();

            return GetBookByIsbn(tittle);
        }

        public void DeleteBookByISBN(string isbk)
        {
            const string k_query = "delete from Books where ISBN = ?";
            SQLiteCommand command = _connection.CreateCommand(k_query, isbk);
            command.ExecuteNonQuery();
        }

        public bool PublisherExists(string publisherName)
        {
            const string k_query = "select count(*) from Publishers where Name = ? limit 1";

            SQLiteCommand command = _connection.CreateCommand(k_query, publisherName);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public bool AuthorExists(string authorName)
        {
            const string k_query = "select count(*) from Authors where Name = ? limit 1";

            SQLiteCommand command = _connection.CreateCommand(k_query, authorName);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public bool BookExists(string isbn)
        {
            const string k_query = "select count(*) from Books where ISBN = ? limit 1";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public Publisher EnsurePublisherExists(string publisherName)
        {
            if (!PublisherExists(publisherName))
                return InsertPublisher(publisherName);

            return GetPublisherByName(publisherName);
        }

        public Author EnsureAuthorExists(string authorName)
        {
            if (!AuthorExists(authorName))
                return InsertAuthor(authorName);

            return GetAuthorByName(authorName);
        }

        public void LinkBookToAuthor(string isbn, int authorId)
        {
            const string k_query = "insert into BookAuthor (BookISBN, AuthorId) values (?, ?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn, authorId);
            command.ExecuteNonQuery();
        }

        public List<Author> GetAuthorsOf(Book book)
        {
            const string k_query = "select * from Authors " +
                "inner join BookAuthor on Authors.Id = BookAuthor.AuthorId " +
                "where BookAuthor.BookISBN = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, book.ISBN);
            return command.ExecuteQuery<Author>();
        }

        public List<Book> GetAllBooks()
        {
            const string k_query = "select * from Books";
            SQLiteCommand command = _connection.CreateCommand(k_query);
            List<Book> books= command.ExecuteQuery<Book>();

            foreach (Book book in books)
            {
                book.Publisher = GetPublisherById(book.PublisherId);
                book.Authors = GetAuthorsOf(book.ISBN);
            }

            return books;
        }

        public List<Book> SearchBooksByTitle(string title)
        {
            const string k_query = "select * from Books where Title like ?";
            SQLiteCommand command = _connection.CreateCommand(k_query, $"%{title}%");
            List<Book> books = command.ExecuteQuery<Book>();

            foreach (Book book in books)
            {
                book.Publisher = GetPublisherById(book.PublisherId);
                book.Authors = GetAuthorsOf(book.ISBN);
            }

            return books;
        }

        public void UpdateBook(Book book)
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction()
        {
            _connection.BeginTransaction();
        }

        public void Commit()
        {
            _connection.Commit();
        }

        public void Rollback()
        {
            _connection.Rollback();
        }
    }
}