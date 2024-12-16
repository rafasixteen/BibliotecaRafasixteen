using SQLite;

namespace BibliotecaRafasixteen
{
    public class LibraryDatabase(string databasePath)
    {
        private readonly SQLiteConnection _connection = new(databasePath);

        public string DatabasePath => _connection.DatabasePath;

        #region CREATE

        public void AddBook(Book book)
        {
            try
            {
                BeginTransaction();

                foreach (Author author in book.Authors)
                {
                    Author existingAutor = EnsureAuthorExists(author.Name);
                    LinkBookToAuthor(book.ISBN, existingAutor.Id);
                }

                Publisher publisher = EnsurePublisherExists(book.Publisher.Name);
                InsertBook(book.ISBN, book.Title, publisher.Id);

                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        public Publisher InsertPublisher(string publisherName)
        {
            if (HasPublisher(publisherName))
                throw new InvalidOperationException($"Cannot insert publisher: A publisher with the name '{publisherName}' already exists in the database. Please choose a different name.");

            const string k_query = "insert into Publishers (Name) values (?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, publisherName);
            command.ExecuteNonQuery();

            return GetPublisherByName(publisherName);
        }

        public Author InsertAuthor(string authorName)
        {
            if (HasAuthor(authorName))
                throw new InvalidOperationException($"Cannot insert author: An author with the name '{authorName}' already exists in the database. Please choose a different name.");

            const string k_query = "insert into Authors (Name) values (?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, authorName);
            command.ExecuteNonQuery();

            return GetAuthorByName(authorName);
        }

        public Book InsertBook(string isbn, string tittle, int publisherId)
        {
            if (!Book.IsValidISBN13(isbn))
                throw new ArgumentException($"Cannot insert book: The ISBN '{isbn}' is not a valid ISBN-13. Please check the ISBN and try again.");

            if (HasBook(isbn))
                throw new InvalidOperationException($"Cannot insert book: A book with ISBN '{isbn}' already exists in the database. ISBNs must be unique. Please check the ISBN and try again.");

            const string k_query = "insert into Books(ISBN, Title, PublisherId) values (?, ?, ?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn, tittle, publisherId);
            command.ExecuteNonQuery();

            return GetBookByIsbn(tittle);
        }

        public void LinkBookToAuthor(string bookIsbn, int authorId)
        {
            const string k_query = "insert into BookAuthor (BookISBN, AuthorId) values (?, ?)";

            SQLiteCommand command = _connection.CreateCommand(k_query, bookIsbn, authorId);
            command.ExecuteNonQuery();
        }

        #endregion

        #region READ

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

        public bool HasPublisher(string publisherName)
        {
            const string k_query = "select count(*) from Publishers where Name = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, publisherName);
            int count = command.ExecuteScalar<int>();
            return count > 0;
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

        public Author GetAuthorById(int id)
        {
            const string k_query = "select * from Authors where Id = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, id);
            Author? result = command.ExecuteQuery<Author>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Author with Id '{id}' doesn't exist in the database.");

            return result;
        }

        public bool HasAuthor(string authorName)
        {
            const string k_query = "select count(*) from Authors where Name = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, authorName);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public Book GetBookByTitle(string title)
        {
            const string k_query = "select * from Books where Title = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, title);
            Book? result = command.ExecuteQuery<Book>().FirstOrDefault();

            if (result == null)
                throw new ArgumentException($"Book with title '{title}' doesn't exist in the database.");

            return result;
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

        public bool HasBook(string isbn)
        {
            const string k_query = "select count(*) from Books where ISBN = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, isbn);
            int count = command.ExecuteScalar<int>();
            return count > 0;
        }

        public List<Author> GetAuthorsOfBook(string bookIsbn)
        {
            const string k_query = "select * from Authors " +
                    "inner join BookAuthor on Authors.Id = BookAuthor.AuthorId " +
                    "where BookAuthor.BookISBN = ?";

            SQLiteCommand command = _connection.CreateCommand(k_query, bookIsbn);
            return command.ExecuteQuery<Author>();
        }

        public List<Book> GetAllBooks()
        {
            const string k_query = "select * from Books";
            SQLiteCommand command = _connection.CreateCommand(k_query);
            List<Book> books = command.ExecuteQuery<Book>();

            foreach (Book book in books)
            {
                book.Publisher = GetPublisherById(book.PublisherId);
                book.Authors = GetAuthorsOfBook(book.ISBN);
            }

            return books;
        }

        #endregion

        #region UPDATE

        #endregion

        #region DELETE

        public void DeleteBook(string isbn)
        {
            if (!HasBook(isbn))
                throw new InvalidOperationException($"Cannot delete book: A book with ISBN '{isbn}' does not exist in the database. Please check the ISBN and try again.");

            try
            {
                BeginTransaction();

                const string k_deleteBookAuthorLinkQuery = "delete from BookAuthor where BookISBN = ?";
                SQLiteCommand deleteLinkCommand = _connection.CreateCommand(k_deleteBookAuthorLinkQuery, isbn);
                deleteLinkCommand.ExecuteNonQuery();

                const string k_deleteBookQuery = "delete from Books where ISBN = ?";
                SQLiteCommand deleteBookCommand = _connection.CreateCommand(k_deleteBookQuery, isbn);
                deleteBookCommand.ExecuteNonQuery();

                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        #endregion

        public Publisher EnsurePublisherExists(string publisherName)
        {
            if (!HasPublisher(publisherName))
                return InsertPublisher(publisherName);

            return GetPublisherByName(publisherName);
        }

        public Author EnsureAuthorExists(string authorName)
        {
            if (!HasAuthor(authorName))
                return InsertAuthor(authorName);

            return GetAuthorByName(authorName);
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