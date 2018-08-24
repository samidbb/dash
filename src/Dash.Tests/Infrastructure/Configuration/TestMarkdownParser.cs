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
        public void Can_parse_markdown_table_to_csv(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, string.Join('\n', MarkdownParser.ParseFirstMarkdownTableAsCsvLines(input.Split('\n'))));
        }

        [Theory]
        [InlineData("a|b\n1|2", "|a|b|\n|-|-|\n|1|2|")]
        [InlineData("a", "|a|\n|-|")]
        [InlineData("a|b|c\n1|2|3", "|a|b|c|\n|-|-|-|\n|1|2|3|")]
        [InlineData("a|b|c", "|a|b|c|\n|-|-|-|")]
        public void Can_build_markdown_table_from_strings_of_strings(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, string.Join('\n', MarkdownParser.BuildMarkdownTable(input.Split('\n').Select(x => x.Split('|')))));
        }
    }
}