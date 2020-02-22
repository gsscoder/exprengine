using System.Globalization;

static class StringExtensions
{
    public static string FormatInvariant(this string value, params object[] arguments)
    {
        return string.Format(CultureInfo.InvariantCulture, value, arguments);
    }

    public static string FormatLocal(this string value, params object[] arguments)
    {
        return string.Format(CultureInfo.CurrentCulture, value, arguments);
    }
}