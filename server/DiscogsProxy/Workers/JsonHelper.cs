using System.Text.Json.Nodes;

namespace DiscogsProxy.Workers;

// TODO: Comments and tests!

public static class JsonHelper
{
    // For simple types (string, int, Uri, etc.)
    public static T? GetPropertyValue<T>(this JsonNode? input, params string[] path)
    {
        if (input == null || path.Length == 0)
        {
            return default;
        }

        foreach (var key in path)
        {
            if (input == null)
            {
                return default;
            }

            input = NavigateNode(input, key);
        }

        return HandleNodeSimple<T>(input);
    }

    public static T? GetPropertyValue<T, TElem>(this JsonNode? input, params string[] path) where T : ICollection<TElem>, new()
    {
        if (input == null || path.Length == 0)
            return default;

        foreach (var key in path)
        {
            if (input == null)
                return default;

            input = NavigateNode(input, key);
        }

        return HandleNodeCollection<T, TElem>(input);
    }

    private static T? HandleNodeSimple<T>(JsonNode? input)
    {
        if (input == null)
        {
            return default;
        }

        var tType = typeof(T);

        if (tType == typeof(JsonNode) || (tType == typeof(JsonArray) && input is JsonArray))
        {
            return (T)(object)input;
        }

        if (tType == typeof(Uri))
        {
            var str = input.GetValue<string?>();
            return Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var uri)
                ? (T)(object)uri
                : default;
        }

        if (input is JsonValue jsonValue)
        {
            return jsonValue.GetValue<T>();
        }

        return default;
    }

    private static T? HandleNodeCollection<T, TElem>(JsonNode? input) where T : ICollection<TElem>, new()
    {
        if (input == null)
        {
            return default;
        }

        if (typeof(T) == typeof(JsonNode) || (typeof(T) == typeof(JsonArray) && input is JsonArray))
        {
            return (T)(object)input;
        }

        if (input is JsonArray jsonArray)
        {
            var combined = new T();

            foreach (var item in jsonArray)
            {
                combined.Add(item!.GetValue<TElem>());
            }

            return combined;
        }

        return default;
    }

    private static JsonNode? NavigateNode(JsonNode node, string key)
    {
        if (int.TryParse(key, out int index) && node is JsonArray array)
        {
            return index >= 0 && index < array.Count ? array[index] : null;
        }

        if (node is JsonObject obj && obj.TryGetPropertyValue(key, out var child))
        {
            return child;
        }

        return null;
    }
}