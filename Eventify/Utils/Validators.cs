using System;
using System.Text.RegularExpressions;

namespace Eventify.Utils
{
    public static class Validators
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsStrongPassword(string password)
        {
            return password.Length >= 8 &&
                   Regex.IsMatch(password, "[A-Z]") &&
                   Regex.IsMatch(password, "[a-z]") &&
                   Regex.IsMatch(password, "[0-9]");
        }

        public static bool IsFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        public static bool IsValidEventDuration(DateTime start, DateTime end)
        {
            return start < end && (end - start).TotalHours <= 24;
        }
    }
}