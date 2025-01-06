using SQLite;

namespace BibliotecaRafasixteen
{
    public class Publisher
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public List<Book> Books { get; set; } = new();

        public static Publisher Random()
        {
            string[] publisherNames = new string[]
            {
                "HarperCollins", "Penguin Random House", "Macmillan", "Simon & Schuster", "Hachette Livre",
                "Oxford University Press", "Pearson", "Cambridge University Press", "Scholastic", "Wiley",
                "Springer", "Bloomsbury", "Random House", "Cengage", "McGraw-Hill Education",
                "Pan Macmillan", "DK Publishing", "Usborne", "Faber and Faber", "Quirk Books",
                "Chronicle Books", "Andrews McMeel Publishing", "Workman Publishing", "Sourcebooks",
                "Abrams Books", "Little, Brown and Company", "Tor Books", "Orbit", "Crown Publishing Group",
                "Del Rey", "Bantam", "Ballantine Books", "Vintage", "Knopf", "Picador", "Anchor Books",
                "New Directions", "Algonquin Books", "Graywolf Press", "Counterpoint Press", "Coffee House Press",

                "Editora Globo", "Companhia das Letras", "Saraiva", "Editora Record", "Grupo Editorial Novo Século",
                "Editora Intrínseca", "Editora Rocco", "Editora Objetiva", "Grupo Editorial Autêntica", "Editora Moderna",
                "Editora Ática", "Editora FTD", "Editora Positivo", "Leya", "Editora Planeta", "Bertrand Brasil",
                "Martins Fontes", "Livros do Brasil", "Editora 34", "Cosac Naify", "Porto Editora", "Grupo Editorial Pensamento",
                "Editora Sextante", "Panda Books", "Melhoramentos", "Publifolha", "Oficina de Textos", "Girassol Brasil",
                "Estrela Cultural", "Nemo", "Zahar", "Editora Pallas", "Pólen Livros", "L&PM Editores", "Quatro Cantos"
            };

            string name = publisherNames[new Random().Next(publisherNames.Length)];
            return new Publisher { Name = name };
        }

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
}