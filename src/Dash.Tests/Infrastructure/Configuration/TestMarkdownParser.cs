using System.Linq;
using Dash.Infrastructure.Configuration;
using Xunit;

namespace Dash.Tests.Infrastructure.Configuration
{
    public class TestMarkdownParser
    {
        [Theory]
        [InlineData("-", 1)]
        [InlineData(" - ", 1)]
        [InlineData(":-", 1)]
        [InlineData("-:", 1)]
        [InlineData(":-:", 1)]
        [InlineData("-|", 1)]
        [InlineData("|-", 1)]
        [InlineData("-|-", 2)]
        [InlineData("|-|-|", 2)]
        [InlineData("|---|---|", 2)]
        [InlineData("|:--|:-:|--:|", 3)]
        public void Test_ColumnDelimiter(string input, int expectedColumnCount)
        {
            Assert.Equal(expectedColumnCount, MarkdownParser.IsColumnDelimiter(input));
        }

        [Theory]
        [InlineData("a|b\n-|-\n1|2", "a|b\n1|2")]
        [InlineData("a|\n-\n", "a")]
        [InlineData("|a|b|c|\n|:--|:-:|--:|\n|1|2|3|", "a|b|c\n1|2|3")]
        [InlineData("|a|b|c|\n|:--|:-:|--:|\n||||", "a|b|c\n")]
        public void Can_parse_markdown_table_to_csv(string testInput, string expectedOutput)
        {
            string[] lines = testInput.Split('\n');

            var result = MarkdownParser.ParseMarkdownTable(lines);

            Assert.Equal(expectedOutput, string.Join('\n', result.Select(x => string.Join('|', x))));
        }

        // JOIN string WITH '|' JOIN lines WITH '\n'

        [Theory]
        [InlineData("a|b", "1|2", "|a|b|\n|-|-|\n|1|2|")]
        [InlineData("a", null, "|a|\n|-|")]
        [InlineData("a|b|c", "1|2|3", "|a|b|c|\n|-|-|-|\n|1|2|3|")]
        [InlineData("a|b|c", null, "|a|b|c|\n|-|-|-|")]
        public void Can_build_markdown_table_from_strings_of_strings(string testHeader, string testLines, string expectedOutput)
        {
            string[] headers = testHeader.Split('|');
            string[][] lines = testLines?.Split('\n').Select(x => x.Split('\n')).ToArray();
            var markdownTable = new MarkdownTable(headers, lines);

            var result = MarkdownParser.BuildMarkdownTable(markdownTable);

            Assert.Equal(expectedOutput, string.Join('\n', result));
        }
    }
}