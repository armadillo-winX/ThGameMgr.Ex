namespace ThGameMgr.Ex.Extensions
{
    internal static class StringExtensions
    {
        public static string TranslateUnixTime(this int unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime.ToString();
        }

        public static int TranslateHexadecimal(this string hexadecimal)
        {
            return Convert.ToInt32(hexadecimal, 16);
        }
    }
}
