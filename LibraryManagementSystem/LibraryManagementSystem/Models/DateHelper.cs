using System;
using System.Globalization;

namespace LibraryManagementSystem
{
    // HELPER: one place that decides how dates look in files and on screen: year-month-day, e.g. 2026-11-25.
    // "static" means we call it directly (DateHelper.Format(...)) without creating an object, like Console.WriteLine.
    public static class DateHelper
    {
        // Date -> text, e.g. 2026-11-25
        public static string Format(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        // Text -> date, without crashing. Returns true if the text was a real date in the form yyyy-MM-dd.
        public static bool TryParse(string text, out DateTime date)
        {
            return DateTime.TryParseExact(text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
