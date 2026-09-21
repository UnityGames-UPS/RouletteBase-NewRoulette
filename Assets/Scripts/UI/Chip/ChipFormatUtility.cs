internal static class ChipFormatUtility
{
    internal static string Format(double value)
    {
        if (value >= 1000)
            return (value / 1000.0).ToString("0.#") + "K";
        return value.ToString("0.##");
    }
}
