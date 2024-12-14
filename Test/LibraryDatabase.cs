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

        public Author GetAuthorByName(string name)
        {
            const string k_query = "select * from Authors where Name = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, name);
            Author? result = command.ExecuteQuery<Author>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Author with name '{name}' doesn't exist in the database.");

            return result;
        }

        public Book GetBookByTitle(string name)
        {
            const string k_query = "select * from Books where Title = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, name);
            Book? result = command.ExecuteQuery<Book>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Book with name '{name}' doesn't exist in the database.");

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
            const string k_query = "insert into Books(ISBN, Title, PublisherId) values (?, ?, ?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn, tittle, publisherId);
            command.ExecuteNonQuery();

            return GetBookByTitle(tittle);
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