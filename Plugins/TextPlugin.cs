using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace DataAICapstone.Plugins
{
    /// <summary>
    /// Text processing plugin for Semantic Kernel agents
    /// </summary>
    public class TextPlugin
    {
        [KernelFunction]
        [Description("Convert text to uppercase")]
        public string ToUpperCase([Description("The text to convert")] string text)
        {
            return text.ToUpperInvariant();
        }

        [KernelFunction]
        [Description("Convert text to lowercase")]
        public string ToLowerCase([Description("The text to convert")] string text)
        {
            return text.ToLowerInvariant();
        }

        [KernelFunction]
        [Description("Convert text to title case")]
        public string ToTitleCase([Description("The text to convert")] string text)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        [KernelFunction]
        [Description("Count the number of characters in text")]
        public int CountCharacters([Description("The text to count")] string text)
        {
            return text.Length;
        }

        [KernelFunction]
        [Description("Count the number of words in text")]
        public int CountWords([Description("The text to count")] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        [KernelFunction]
        [Description("Reverse the text")]
        public string ReverseText([Description("The text to reverse")] string text)
        {
            return new string(text.Reverse().ToArray());
        }

        [KernelFunction]
        [Description("Replace text within a string")]
        public string ReplaceText(
            [Description("The original text")] string text,
            [Description("The text to find")] string find,
            [Description("The text to replace with")] string replace)
        {
            return text.Replace(find, replace);
        }

        [KernelFunction]
        [Description("Extract a substring from text")]
        public string ExtractSubstring(
            [Description("The original text")] string text,
            [Description("The starting position (0-based)")] int startIndex,
            [Description("The length of the substring")] int length)
        {
            if (startIndex < 0 || startIndex >= text.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + length > text.Length)
                length = text.Length - startIndex;

            return text.Substring(startIndex, length);
        }

        [KernelFunction]
        [Description("Split text into an array using a delimiter")]
        public string[] SplitText(
            [Description("The text to split")] string text,
            [Description("The delimiter to split on")] string delimiter)
        {
            return text.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
        }

        [KernelFunction]
        [Description("Join an array of text with a delimiter")]
        public string JoinText(
            [Description("The text array as comma-separated values")] string textArray,
            [Description("The delimiter to join with")] string delimiter)
        {
            var parts = textArray.Split(',').Select(s => s.Trim()).ToArray();
            return string.Join(delimiter, parts);
        }

        [KernelFunction]
        [Description("Trim whitespace from the beginning and end of text")]
        public string TrimText([Description("The text to trim")] string text)
        {
            return text.Trim();
        }

        [KernelFunction]
        [Description("Check if text contains a specific substring")]
        public bool ContainsText(
            [Description("The text to search in")] string text,
            [Description("The text to search for")] string searchText)
        {
            return text.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }
    }
}
