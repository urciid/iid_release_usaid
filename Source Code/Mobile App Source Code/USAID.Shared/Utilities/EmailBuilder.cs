using System;
using System.Text;

namespace USAID.Utilities
{
    public static class EmailBuilder
    {
        private const string newline = "<br>";

        public static void AddBoldedHeader(StringBuilder sb, string header)
        {
            sb.Append("<b>" + header + "</b>" + newline);
        }

        public static void AddRegularText(StringBuilder sb, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                sb.Append(text + newline);
        }

        public static void AddItalicizedText(StringBuilder sb, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                sb.Append("<i>" + text + "</i>" + newline);
        }

        public static void AddIndentedText(StringBuilder sb, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                sb.Append("&nbsp; &nbsp; &nbsp; &nbsp;" + text + newline);
        }

        public static void AddNewline(StringBuilder sb)
        {
            sb.Append(newline);
        }
    }
}

