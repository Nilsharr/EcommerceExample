namespace Ecommerce.Utils;

public static class EnumExtensions
{
    public static TEnum GetEnum<TEnum>(this string value, bool ignoreCase = true) where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(value, ignoreCase);
    }

    public static TEnum GetEnumOrDefault<TEnum>(this string value, bool ignoreCase = true) where TEnum : struct, Enum
    {
        return Enum.TryParse(value, ignoreCase, out TEnum result) ? result : default;
    }
}