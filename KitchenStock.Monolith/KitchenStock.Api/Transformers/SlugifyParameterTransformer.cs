using System.Text.RegularExpressions;

namespace KitchenStock.Api.Transformers;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value is null) return null;

        var input = value.ToString()!;
        return Regex.Replace(input,
            "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
    }
}