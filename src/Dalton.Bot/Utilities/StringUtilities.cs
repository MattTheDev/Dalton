using System.Globalization;

namespace Dalton.Bot.Utilities;

public static class StringUtilities
{
    public static string FirstLetterToUpper(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (str.Length > 1)
        {
            return char.ToUpper(str[0], CultureInfo.InvariantCulture) + str[1..];
        }

        return str.ToUpper(CultureInfo.InvariantCulture);
    }

    public static bool IsAlphaNumeric(this string str, char[] additionalAllowed)
    {
        return str.All(x =>
            char.IsWhiteSpace(x) ||
            char.IsLetterOrDigit(x) ||
            additionalAllowed.Any(y => x == y));
    }
}