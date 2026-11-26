using System;

namespace LibraryManagementSystem
{
    // SERVICES (BUSINESS) LAYER: every input rule lives here. It only answers true/false and never prints.
    public class InputValidator
    {
        public const int MaxCopies = 1000;   // "const" = a value that never changes

        // Text must not be empty and must not contain "|" (we use "|" to separate values in the data files).
        public bool IsValidText(string text)
        {
            return text.Trim() != "" && !text.Contains("|");
        }

        // A person's or author's name: letters, spaces, hyphens, apostrophes and full stops only (no digits).
        public bool IsValidName(string text)
        {
            if (text.Trim() == "")
            {
                return false;
            }
            foreach (char c in text)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-' && c != '\'' && c != '.')
                {
                    return false;   // found a character that is not allowed in a name
                }
            }
            return true;
        }

        // Can the text be read as a whole number greater than 0? The number goes into "number".
        public bool TryParsePositiveInt(string text, out int number)
        {
            return int.TryParse(text, out number) && number > 0;
        }

        // Number of copies must be between 1 and 1000.
        public bool IsCopiesInRange(int copies)
        {
            return copies >= 1 && copies <= MaxCopies;
        }

        // Phone number: digits only, 7 to 15 of them.
        public bool IsValidPhone(string phone)
        {
            if (phone.Length < 7 || phone.Length > 15)
            {
                return false;
            }
            foreach (char c in phone)      // look at each character
            {
                if (c < '0' || c > '9')    // a character that is not 0-9
                {
                    return false;
                }
            }
            return true;
        }
    }
}
