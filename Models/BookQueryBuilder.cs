using SQLite;

namespace BibliotecaRafasixteen
{
    public class BookQueryBuilder(LibraryDatabase database)
    {
        private readonly LibraryDatabase _database = database;

        private readonly Dictionary<EBookColumn, string> _filters = new();

        private EBookColumn? _sortColumn = null;
        private ESortOrder? _sortOrder = null;

        private int _skip = 0;
        private int _take = 0;

        private bool ShouldFilter => _filters.Count > 0;

        private bool ShouldSort => _sortColumn != null && _sortOrder != null;

        private bool ShouldPaginate => _skip > 0 || _take > 0;

        public BookQueryBuilder All()
        {
            _sortColumn = null;
            _sortOrder = null;
            return this;
        }

        public BookQueryBuilder SortBy(EBookColumn column, ESortOrder order)
        {
            _sortColumn = column;
            _sortOrder = order;
            return this;
        }

        public BookQueryBuilder FilterBy(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return this;

            searchQuery = searchQuery.ToLower();

            // Example search query: "title: The Great Adventure; authors: J.K. Rowling; publisher: Bloomsbury".
            // The search query can contain multiple filter column-value pairs separated by ";".
            // Each filter column-value pair is separated by ":".
            // The filter column is the name of the column to filter by (e.g., title, authors, publisher, isbn).
            // The filter value is the value to search for in the specified column.
            // This is going to look for books with the title "The Great Adventure" by J.K. Rowling published by Bloomsbury.

            Dictionary<string, EBookColumn> possibleFilters = new()
            {
                { "title", EBookColumn.Title },
                { "authors", EBookColumn.Authors },
                { "publisher", EBookColumn.Publisher },
                { "isbn", EBookColumn.ISBN }
            };

            try
            {
                string[] pairs = searchQuery.Split(';');

                foreach (string pair in pairs)
                {
                    // Skip empty pairs (e.g., after a trailing semicolon).
                    if (string.IsNullOrWhiteSpace(pair))
                        continue;

                    string[] columnValue = pair.Split(':');

                    // If it's an incomplete column-value pair (e.g., user is typing), skip it.
                    if (columnValue.Length != 2)
                    {
                        // If the query has no valid column-value pair, treat the whole query as a title search.
                        if (pairs.Length == 1)
                        {
                            _filters.Clear();
                            _filters.Add(EBookColumn.Title, searchQuery);
                            return this;
                        }

                        continue;
                    }

                    string filterColumn = columnValue[0].Trim();
                    string filterValue = columnValue[1].TrimStart().TrimEnd();

                    EBookColumn column = ResolveFilterColumn(filterColumn, possibleFilters);
                    _filters.Add(column, filterValue);
                }
            }
            catch
            {
                _filters.Clear();
                _filters.Add(EBookColumn.Title, searchQuery);

                throw;
            }

            return this;
        }

        public BookQueryBuilder Skip(int value)
        {
            _skip = value;
            return this;
        }

        public BookQueryBuilder Take(int value)
        {
            _take = value;
            return this;
        }

        public int Count()
        {
            string query = BuildCountQuery();
            SQLiteCommand command = _database.Connection.CreateCommand(query);
            return command.ExecuteScalar<int>();
        }

        public List<Book> Execute()
        {
            string query = BuildQuery();

            SQLiteCommand command = _database.Connection.CreateCommand(query);

            List<Book> books = command.ExecuteQuery<Book>();

            foreach (Book book in books)
                _database.PopulateBook(book);

            return books;
        }

        private string BuildQuery()
        {
            if (!ShouldSort && !ShouldFilter)
                return @$"select * from Books {BuildPaginationClause()}";

            return @$"
                select * from Books
                join Publishers on Books.PublisherId = Publishers.Id
                join BookAuthor on Books.ISBN = BookAuthor.BookISBN
                join Authors on BookAuthor.AuthorId = Authors.Id
                {BuildFilterClause()}
                group by Books.ISBN
                {BuildSortClause()}
                {BuildPaginationClause()}";
        }

        private string BuildCountQuery()
        {
            return @$"
                select count(distinct Books.ISBN)
                from Books
                join Publishers on Books.PublisherId = Publishers.Id
                join BookAuthor on Books.ISBN = BookAuthor.BookISBN
                join Authors on BookAuthor.AuthorId = Authors.Id
                {BuildFilterClause()}";
        }

        private string BuildFilterClause()
        {
            if (!ShouldFilter)
                return string.Empty;

            List<string> conditions = new();

            foreach ((EBookColumn column, string value) in _filters)
            {
                string columnName = ResolveColumnName(column);
                conditions.Add($"{columnName} like '%{value}%'");
            }

            return "where " + string.Join(" and ", conditions);
        }

        private string BuildSortClause()
        {
            if (!ShouldSort)
                return string.Empty;

            return $"order by {ResolveColumnName(_sortColumn!.Value)} {ResolveSortOrderName(_sortOrder!.Value)}";
        }

        private string BuildPaginationClause()
        {
            if (!ShouldPaginate)
                return string.Empty;

            return $"limit {_take} offset {_skip}";
        }

        private static string ResolveColumnName(EBookColumn column)
        {
            return column switch
            {
                EBookColumn.Title => "Books.Title",
                EBookColumn.Publisher => "Publishers.Name",
                EBookColumn.ISBN => "Books.ISBN",
                EBookColumn.Authors => "Authors.Name",
                _ => throw new ArgumentException($"Invalid column: {column}")
            };
        }

        private static string ResolveSortOrderName(ESortOrder order)
        {
            return order switch
            {
                ESortOrder.Ascending => "asc",
                ESortOrder.Descending => "desc",
                _ => throw new ArgumentException($"Invalid sort order: {order}")
            };
        }

        private static EBookColumn ResolveFilterColumn(string filterColumn, Dictionary<string, EBookColumn> possibleFilters)
        {
            List<KeyValuePair<string, EBookColumn>> matchedColumns = new();

            foreach (KeyValuePair<string, EBookColumn> kv in possibleFilters)
            {
                string columnName = kv.Key;

                if (columnName.StartsWith(filterColumn, StringComparison.OrdinalIgnoreCase))
                    matchedColumns.Add(kv);
            }

            if (matchedColumns.Count == 0)
                throw new ArgumentException($"No match found for filter column: '{filterColumn}'");

            if (matchedColumns.Count > 1)
                throw new ArgumentException($"Ambiguous filter column: '{filterColumn}'. Multiple matches found.");

            return matchedColumns[0].Value;
        }
    }
}