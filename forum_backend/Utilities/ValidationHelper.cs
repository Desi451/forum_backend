using System.Text.RegularExpressions;

namespace forum_backend.Utilities
{
    public static class ValidationHelper
    {
        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?!\.)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        public static bool ValidateLoginOrNickname(string login)
        {
            return Regex.IsMatch(login, "^[a-zA-Z0-9]{5,12}$");
        }

        public static bool ValidatePassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}
