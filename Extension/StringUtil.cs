namespace Corbel.Extension
{
    internal static class StringExtension
    {
        internal static string FirstLetterToLower(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToLower(charArray[0]);

            return new string(charArray);
        }

        internal static string FirstLetterToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);

            return new string(charArray);
        }
    }

}