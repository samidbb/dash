using System.Linq;
using System.Text;
using Dash.Infrastructure;
using Dash.Tests.TestDoubles;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests
{
    public class TestDashboardSettingsRepository
    {
        [Theory]
        [InlineData("", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||", 1)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||\n|A|B|C|x|x||", 2)]
        public void Has_expected_number_of_rows(string input, int expectedRows)
        {
            var stub = new StubFileSystem(input);
            var sut = new DashboardSettingsRepository(stub);

            var dashboards = sut.GetAll().ToList();

            Assert.Equal(expectedRows, dashboards.Count);
        }

        [Fact]
        public void Can_parse_settings()
        {
            var stub = new StubFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||");
            var sut = new DashboardSettingsRepository(stub);

            var result = sut.GetAll().FirstOrDefault();

            DashboardSettingsAssert.Equal(
                expected: A.DashboardSettings
                    .WithId("A")
                    .WithName("B")
                    .WithTeam("C")
                    .WithEnvironments("Env1", "Env2"),
                actual: result);
        }
    }

    public static class A
    {
        public static DashboardSettingsBuilder DashboardSettings => new DashboardSettingsBuilder();
    }

    internal static class DashboardSettingsAssert
    {
        public static void Equal(DashboardSettings expected, DashboardSettings actual)
        {
            if (Equals(expected, actual))
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);

            var expectedText = PrintDashboardSettings(expected);
            var actualText = PrintDashboardSettings(actual);

            if (expectedText != actualText)
            {
                throw new EqualException(expectedText, actualText);
            }
        }

        private static string PrintDashboardSettings(DashboardSettings expected)
        {
            var expectedText = new StringBuilder();
            expectedText.AppendLine("DashboardSettings with");
            expectedText.AppendLine($"\t.Id   = '{expected.Id}'");
            expectedText.AppendLine($"\t.Name = '{expected.Name}'");
            expectedText.AppendLine($"\t.Team = '{expected.Team}'");
            expectedText.AppendLine($"\t.Env  = [{string.Join(",", expected.Environments)}]");
            return expectedText.ToString();
        }
    }
}