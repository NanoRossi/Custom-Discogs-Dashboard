using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonHelper
{
    public static T GetProperty<T>(this JsonNode? inputNode, string propertyName)
    {
        var containsProp = inputNode!.AsObject().TryGetPropertyValue(propertyName, out JsonNode? node);

        if (!containsProp)
        {
            return default(T)!;
        }

        return node!.GetValue<T>();
    }

    public static T? GetNestedValue<T>(this JsonNode? node, params string[] path)
    {
        if (node == null || path.Length == 0)
            return default;

        for (int i = 0; i < path.Length; i++)
        {
            if (node == null)
                return default;

            bool isLast = (i == path.Length - 1);
            string key = path[i];

            node = NavigateNode(node, key);

            if (isLast)
                return ConvertNode<T>(node);
        }

        return default;
    }

    private static JsonNode? NavigateNode(JsonNode node, string key)
    {
        // If key is numeric, treat as array index
        if (int.TryParse(key, out int index) && node is JsonArray array)
            return (index >= 0 && index < array.Count) ? array[index] : null;

        // Otherwise treat as object key
        if (node is JsonObject obj && obj.TryGetPropertyValue(key, out var child))
            return child;

        return null;
    }

    private static T? ConvertNode<T>(JsonNode? node)
    {
        if (node == null)
            return default;

        if (typeof(T) == typeof(JsonNode))
            return (T)(object)node;

        if (typeof(T) == typeof(JsonArray) && node is JsonArray arr)
            return (T)(object)arr;

        if (typeof(T) == typeof(Uri))
        {
            var str = node.GetValue<string?>();
            return Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var uri)
                ? (T)(object)uri
                : default;
        }

        if (node is JsonValue val)
        {
            try { return val.GetValue<T>(); }
            catch { return default; }
        }

        if (node is JsonArray listArray && IsListType(typeof(T), out var itemType))
            return DeserializeList<T>(listArray, itemType);

        try { return JsonSerializer.Deserialize<T>(node.ToJsonString()); }
        catch { return default; }
    }

    private static bool IsListType(Type type, out Type itemType)
    {
        itemType = typeof(object);
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            itemType = type.GetGenericArguments()[0];
            return true;
        }
        return false;
    }

    private static T? DeserializeList<T>(JsonArray array, Type itemType)
    {
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;

        foreach (var item in array)
        {
            try
            {
                var deserialized = JsonSerializer.Deserialize(item!.ToJsonString(), itemType);
                if (deserialized != null)
                    list.Add(deserialized);
            }
            catch { }
        }

        return (T)list;
    }
}