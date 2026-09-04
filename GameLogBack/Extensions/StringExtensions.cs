using System.Text;

namespace GameLogBack.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string str)
    {
        var kebabCaseString = new StringBuilder()
            .Append(str)
            .Replace(" ", "-")
            .ToString()
            .ToLower();
        return kebabCaseString;
    }
}