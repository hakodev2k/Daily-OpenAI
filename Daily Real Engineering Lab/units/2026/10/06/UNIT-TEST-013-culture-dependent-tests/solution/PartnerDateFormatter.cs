using System.Globalization;

namespace CultureLab;

public static class PartnerDateFormatter
{
    public static string Format(DateOnly value)
    {
        return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
