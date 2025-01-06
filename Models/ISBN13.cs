namespace BibliotecaRafasixteen
{
    public struct ISBN13(string value) : IEquatable<ISBN13>
    {
        private string _value = value;

        public string Value
        {
            readonly get => _value;
            set => _value = value;
        }

        public static bool IsValid(string isbn)
        {
            if (isbn.Length != 13 || isbn.Any((c) => !char.IsDigit(c)))
                return false;

            int sum = 0;

            for (int i = 0; i < 12; i++)
            {
                int digit = int.Parse(isbn[i].ToString());

                if (i % 2 == 0)
                    sum += digit;
                else
                    sum += digit * 3;
            }

            int checksum = (10 - (sum % 10)) % 10;

            return checksum == int.Parse(isbn[12].ToString());
        }

        public static ISBN13 Random()
        {
            Random random = new();

            int prefix = 978;
            int group = random.Next(0, 10);
            int publisher = random.Next(0, 100);
            int title = random.Next(0, 1000000);

            string isbnWithoutCheck = $"{prefix}{group:D1}{publisher:D2}{title:D6}";

            int sum = 0;

            for (int i = 0; i < isbnWithoutCheck.Length; i++)
            {
                int digit = int.Parse(isbnWithoutCheck[i].ToString());

                if (i % 2 == 0)
                    sum += digit;
                else
                    sum += digit * 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            string isbnFull = $"{isbnWithoutCheck}{checkDigit}";
            return new ISBN13 { Value = isbnFull };
        }

        public readonly bool IsValid()
        {
            return IsValid(Value);
        }

        public readonly bool Equals(ISBN13 other)
        {
            return Value == other.Value;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is ISBN13 other && Equals(other);
        }

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override readonly string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(ISBN13 left, ISBN13 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ISBN13 left, ISBN13 right)
        {
            return !left.Equals(right);
        }

        public static implicit operator ISBN13(string value)
        {
            return new ISBN13 { Value = value };
        }

        public static implicit operator string(ISBN13 isbn)
        {
            return isbn.Value;
        }
    }
}