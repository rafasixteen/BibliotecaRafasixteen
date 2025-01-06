namespace BibliotecaRafasixteen
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

#if WINDOWS
        protected override Window CreateWindow(IActivationState? activationState)
        {
            const float k_aspectRatio = 16f / 9f;
            const int k_minWidth = 1600;
            const float k_minHeight = k_minWidth / k_aspectRatio;

            TitleBar titleBar = new()
            {
                BackgroundColor = Color.FromArgb("#ff2194f3"),
                Title = "Biblioteca Rafasixteen",
                ForegroundColor = Colors.Transparent,
            };

            Window window = new(new MainPage())
            {
                MinimumWidth = k_minWidth,
                MinimumHeight = k_minHeight,
                TitleBar = titleBar,
            };

            return window;
        }
#endif
    }
}
