namespace BibliotecaRafasixteen
{
    public class Book
    {
        private ISBN13 _isbn;

        public string ISBN
        {
            get => _isbn.ToString();
            set => _isbn = new ISBN13() { Value = value };
        }

        public required string Title { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; } = null!;

        public List<Author> Authors { get; set; } = new();

        public string AuthorsString
        {
            get => string.Join(", ", Authors.Select(author => author.Name));
            set => Authors = value.Split(',').Select(name => new Author { Name = name.Trim() }).ToList();
        }

        public static Book Random()
        {
            static List<Author> GetRandomAuthors()
            {
                List<Author> authors = new();
                int authorsCount = new Random().Next(1, 3);

                for (int i = 0; i < authorsCount; i++)
                    authors.Add(Author.Random());

                return authors;
            }

            string[] bookTitles = new string[]
            {
                "The Great Adventure", "Mystery of the Forest", "The Lost Kingdom", "Shadows in the Night", "Journey Beyond",
                "Whispers of the Wind", "The Forgotten Hero", "Secrets of the Ocean", "The Eternal Flame", "Chronicles of Time",
                "Beneath the Crimson Sky", "The Enchanted Garden", "Echoes of the Past", "The Last Frontier", "Winds of Change",
                "Tales of the Unknown", "The Hidden Treasure", "Legends of the Forgotten", "The Golden Compass", "The Midnight Star",
                "The Silent Watcher", "Through the Looking Glass", "The Celestial Key", "Dance of Shadows", "The Starlit Path",
                "The Iron Crown", "Valley of Dreams", "The Mystic Tides", "Riddles of the Ancients", "The Sapphire Stone",
                "Beyond the Horizon", "The Cursed Voyage", "The Winter Chronicles", "The Phantom's Secret", "The Emerald Throne",
                "The Whispering Woods", "The Serpent's Fang", "The Ivory Tower", "The Dark Abyss", "The Alchemist's Apprentice",

                "O Grande Aventureiro", "Mistério da Floresta", "O Reino Perdido", "Sombras na Noite", "Viagem Além",
                "Sussurros do Vento", "O Herói Esquecido", "Segredos do Oceano", "A Chama Eterna", "Crônicas do Tempo",
                "Sob o Céu Carmesim", "O Jardim Encantado", "Ecos do Passado", "A Última Fronteira", "Ventos da Mudança",
                "Contos do Desconhecido", "O Tesouro Oculto", "Lendas dos Esquecidos", "A Bússola Dourada", "A Estrela da Meia-Noite",
                "O Observador Silencioso", "Através do Espelho", "A Chave Celestial", "Dança das Sombras", "O Caminho Iluminado",
                "A Coroa de Ferro", "Vale dos Sonhos", "As Marés Místicas", "Enigmas dos Antigos", "A Pedra de Safira",
                "Além do Horizonte", "A Viagem Amaldiçoada", "As Crônicas de Inverno", "O Segredo do Fantasma", "O Trono Esmeralda",
                "As Florestas Sussurrantes", "A Presa da Serpente", "A Torre de Marfim", "O Abismo Negro", "O Aprendiz do Alquimista"
            };

            string title = bookTitles[new Random().Next(bookTitles.Length)];

            return new Book
            {
                ISBN = ISBN13.Random().ToString(),
                Title = title,
                Publisher = Publisher.Random(),
                Authors = GetRandomAuthors(),
            };
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
}