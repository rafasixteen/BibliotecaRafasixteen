using SQLite;

namespace BibliotecaRafasixteen
{
    public class Author
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public List<Book> Books { get; set; } = new();

        public static Author Random()
        {
            string[] authorNames = new string[]
            {
                "Stephen King", "Agatha Christie", "J.K. Rowling", "George R.R. Martin", "J.R.R. Tolkien",
                "Dan Brown", "Paulo Coelho", "Haruki Murakami", "Margaret Atwood", "Neil Gaiman",
                "James Patterson", "Isaac Asimov", "Arthur C. Clarke", "Ernest Hemingway", "Jane Austen",
                "Charles Dickens", "Mark Twain", "Leo Tolstoy", "Fyodor Dostoevsky", "Emily Brontë",
                "Mary Shelley", "Victor Hugo", "Gabriel García Márquez", "John Steinbeck", "Virginia Woolf",
                "C.S. Lewis", "Kurt Vonnegut", "Ray Bradbury", "Toni Morrison", "Ursula K. Le Guin",
                "T.H. White", "Philip K. Dick", "H.P. Lovecraft", "Louisa May Alcott", "Edgar Allan Poe",
                "Herman Melville", "Chinua Achebe", "Alice Walker", "Kazuo Ishiguro", "Ian McEwan",
                "Salman Rushdie", "Colleen Hoover", "Suzanne Collins", "Veronica Roth", "Rick Riordan",

                "Machado de Assis", "José Saramago", "Clarice Lispector", "Fernando Pessoa", "Jorge Amado",
                "Eça de Queirós", "Luís de Camões", "Lygia Fagundes Telles", "Carlos Drummond de Andrade", "Graciliano Ramos",
                "Rubem Fonseca", "Manuel Bandeira", "Raquel de Queiroz", "João Guimarães Rosa", "Monteiro Lobato",
                "José de Alencar", "Joaquim Manuel de Macedo", "Aluísio Azevedo", "Cecília Meireles", "Érico Veríssimo",
                "Ariano Suassuna", "Mario de Andrade", "Adélia Prado", "Manoel de Barros", "Vinícius de Moraes",
                "Sophia de Mello Breyner", "António Lobo Antunes", "Ana Maria Machado", "Lídia Jorge", "Bernardo Guimarães",
                "Pepetela", "Gonçalves Dias", "Júlio Dinis", "Cristovão Tezza", "Hilda Hilst", "Marina Colasanti",
                "Valter Hugo Mãe", "Mia Couto", "Caio Fernando Abreu", "Raduan Nassar", "Chico Buarque"
            };

            string name = authorNames[new Random().Next(authorNames.Length)];
            return new Author { Name = name };
        }

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
}