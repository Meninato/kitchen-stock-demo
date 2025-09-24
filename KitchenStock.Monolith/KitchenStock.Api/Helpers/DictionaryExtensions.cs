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

    public static Dictionary<string, object> ToCamelCaseKeys(this Dictionary<string, object> dictionary)
    {
        return dictionary.ToDictionary(
            kvp => ToCamelCase(kvp.Key),
            kvp => kvp.Value
        );
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Handle snake_case to camelCase
        if (input.Contains('_'))
        {
            var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return input;

            // First part stays lowercase, capitalize first letter of subsequent parts
            var result = parts[0].ToLowerInvariant();
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    result += char.ToUpperInvariant(parts[i][0]) + parts[i][1..].ToLowerInvariant();
                }
            }
            return result;
        }

        // Handle PascalCase to camelCase
        if (input.Length > 0 && char.IsUpper(input[0]))
        {
            return char.ToLowerInvariant(input[0]) + input[1..];
        }

        return input;
    }
}
