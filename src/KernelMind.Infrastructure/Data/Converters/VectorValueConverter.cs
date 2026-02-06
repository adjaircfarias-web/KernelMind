using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KernelMind.Infrastructure.Data.Converters;

public class VectorValueConverter : ValueConverter<float[], string>
{
    public VectorValueConverter() : base(
        v => SerializeVector(v),
        v => DeserializeVector(v))
    {
    }

    private static string SerializeVector(float[] vector)
    {
        return string.Join(",", vector);
    }

    private static float[] DeserializeVector(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Array.Empty<float>();
        }
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(float.Parse)
            .ToArray();
    }
}
