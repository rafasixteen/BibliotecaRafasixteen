namespace BibliotecaRafasixteen
{
    public class Notification(string message, Notification.EType type)
    {
        public string Message { get; set; } = message;

        public EType Type { get; set; } = type;

        public enum EType
        {
            Info,
            Warning,
            Error
        }

        public string TypeClass => Type.ToString().ToLower();

        public string IconClass => GetIconClass();

        private string GetIconClass()
        {
            return Type switch
            {
                EType.Info => "fa-solid fa-circle-check",
                EType.Warning => "fa-solid fa-circle-exclamation",
                EType.Error => "fa-solid fa-circle-xmark",
                _ => "",
            };
        }
    }
}