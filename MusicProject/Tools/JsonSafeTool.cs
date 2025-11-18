using System.Text.Json;

namespace MusicProject.Tools
{
    public static class JsonSafeTool
    {
        public static JsonElement? SafeProperty(this JsonElement element, string name) {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out JsonElement value))
                return value;
            return null;
        }
        public static string? SafeString(this JsonElement? element) {
            if (element == null) return null;
            if (element.Value.ValueKind == JsonValueKind.String)
                return element.Value.GetString();
            return null;
        }
    }
}
