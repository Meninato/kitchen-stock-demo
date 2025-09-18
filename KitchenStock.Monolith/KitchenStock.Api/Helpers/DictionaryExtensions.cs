using System.Text.RegularExpressions;

namespace KitchenStock.Api.Helpers;

public static class DictionaryExtensions
{
    public static Dictionary<string, object> ToSnakeCaseKeys(this Dictionary<string, object> dictionary)
    {
        return dictionary.ToDictionary(
            kvp => ToSnakeCase(kvp.Key),
            kvp => kvp.Value
        );
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, "([a-z])([A-Z])", "$1_$2")
                   .ToLowerInvariant();
    }
}
