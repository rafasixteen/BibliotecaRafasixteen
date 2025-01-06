namespace BibliotecaRafasixteen
{
    public static class LibraryDatabaseServiceCollectionExtensions
    {
        public static void AddLibraryDatabase(this IServiceCollection services)
        {
            string projectPath = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
            string relativePath = Path.Combine(projectPath, "Database", "library.db");
            services.AddSingleton(new LibraryDatabase(relativePath));
        }
    }
}