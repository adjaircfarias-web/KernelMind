using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KernelMind.Infrastructure.Data.Converters;

public class DictionaryJsonConverter : ValueConverter<Dictionary<string, object>, string>
{
    public DictionaryJsonConverter() : base(
        v => Serialize(v),
        v => Deserialize(v))
    {
    }

    private static string Serialize(Dictionary<string, object> dict)
    {
        return JsonSerializer.Serialize(dict);
    }

    private static Dictionary<string, object> Deserialize(string json)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
    }
}

public class DictionaryComparer : ValueComparer<Dictionary<string, object>>
{
    public DictionaryComparer() : base(
        (d1, d2) => d1 != null && d2 != null && JsonSerializer.Serialize(d1) == JsonSerializer.Serialize(d2),
        d => d != null ? JsonSerializer.Serialize(d).GetHashCode() : 0)
    {
    }
}
