using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dash.Infrastructure
{
    /// <remarks>
    /// https://github.com/lunet-io/markdig/blob/master/src/Markdig.Tests/Specs/PipeTableSpecs.md
    /// </remarks>>
    public static class MarkdownParser
    {
        private const char ColumnSeparator = '|';
        private const char ColumnAlignment = ':';

        private static readonly Regex ColumnPattern = new Regex("^-+$", RegexOptions.Singleline);

        /*
         * Rule 1:
         * a) Each line of a paragraph block have to contain at least a column delimiter | that is not embedded by either a code inline (backstick `) or a HTML inline.
         * b) The second row must separate the first header row from sub-sequent rows by containing a header column separator for each column separated by a column delimiter. 
         * 
         * A header column separator is:
         * - starting by optional spaces
         * - followed by an optional : to specify left align
         * - followed by a sequence of at least one - character
         * - followed by an optional : to specify right align (or center align if left align is also defined)
         * - ending by optional spaces
         *
         * Rule 2:
         * A pipe table ends after a blank line or the end of the file.
         * 
         * Rule 3:
         * A cell content is trimmed (start and end) from white-spaces.
         *
         * Rule 4:
         * Column delimiters | at the very beginning of a line or just before a line ending with only spaces and/or terminated by a newline can be omitted
         *
         * Rule 5:
         * The first row is considered as a header row if it is separated from the regular rows by a row containing a header column separator for each column.
         *
         * Rule 6:
         * A column delimiter has a higher priority than emphasis delimiter
         *
         * Rule 7:
         * A backstick/code delimiter has a higher precedence than a column delimiter |
         *
         * Rule 8:
         * A HTML inline has a higher precedence than a column delimiter |
         *
         * Rule 9:
         * Links have a higher precedence than the column delimiter character |
         *
         * Rule 10:
         * It is possible to have a single row header only
         */
        public static IEnumerable<string> ParseFirstMarkdownTableAsCsvLines(IEnumerable<string> lines)
        {
            using (var it = lines.GetEnumerator())
            {
                var header = FindHeader(it);
                if (header == null)
                {
                    yield break;
                }

                yield return Trim(header);

                while (it.MoveNext())
                {
                    var line = it.Current;
                    if (line.IndexOf(ColumnSeparator) == -1)
                    {
                        break;
                    }

                    yield return Trim(line);
                }
            }
        }

        private static string FindHeader(IEnumerator<string> it)
        {
            while (true)
            {
                var header = FindPotentialHeader(it);
                if (header == null)
                {
                    return null;
                }

                if (!it.MoveNext())
                {
                    return null;
                }

                var current = it.Current;
                var isColumnDelimiter = IsColumnDelimiter(current);
                if (isColumnDelimiter > 0)
                {
                    return header;
                }
            }
        }

        private static string FindPotentialHeader(IEnumerator<string> it)
        {
            while (it.MoveNext())
            {
                var header = it.Current;
                if (header.IndexOf(ColumnSeparator) != -1)
                {
                    return header;
                }
            }

            return null;
        }

        private static string Trim(string input)
        {
            return input.Trim(' ', '\t', ColumnSeparator);
        }

        /// <remarks>
        /// A header column separator is:
        /// - starting by optional spaces
        /// - followed by an optional : to specify left align
        /// - followed by a sequence of at least one - character
        /// - followed by an optional : to specify right align (or center align if left align is also defined)
        /// - ending by optional spaces
        /// </remarks>
        public static int IsColumnDelimiter(string input)
        {
            input = input.Trim();

            var columns = input.Split(ColumnSeparator, StringSplitOptions.RemoveEmptyEntries);
            var isColumnDelimiter = columns.Select(column => column.Trim(ColumnAlignment)).All(column => ColumnPattern.IsMatch(column));

            return isColumnDelimiter ? columns.Length : 0;
        }
    }
}