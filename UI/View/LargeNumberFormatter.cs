namespace Utilities
{
    internal static class LargeNumberFormatter
    {
        public static string Convert(ulong value) =>
            value switch
            {
                >= 1_000_000_000 => (value / 1_000_000_000F).ToString("0.#") + "KKK",
                >= 1_000_000 => (value / 1_000_000F).ToString("0.#") + "KK",
                >= 1_000 => (value / 1_000F).ToString("0.#") + "K",
                _ => value.ToString()
            };
    }
}