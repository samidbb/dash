using System;
using System.Linq;
using System.Text;
using Dash.Infrastructure.Configuration;
using Dash.Tests.TestDoubles;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests.Infrastructure.Configuration
{
    public class TestDashboardConfigurationRepository
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
            var sut = new DashboardConfigurationRepository(stub);

            var dashboards = sut.GetAll().ToList();

            Assert.Equal(expectedRows, dashboards.Count);
        }

        [Fact]
        public void Can_parse_settings()
        {
            var stub = new StubFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            var result = sut.GetAll().FirstOrDefault();

            DashboardSettingsAssert.Equal(
                expected: A.DashboardConfiguration
                    .WithId("A")
                    .WithName("B")
                    .WithTeam("C")
                    .WithEnvironments("Env1", "Env2"),
                actual: result);
        }
    }

    internal static class DashboardSettingsAssert
    {
        public static void Equal(DashboardConfiguration expected, DashboardConfiguration actual)
        {
            if (Equals(expected, actual))
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);

            StringBuilder CreateStringBuilder()
            {
                var sb = new StringBuilder();
                sb.AppendLine("DashboardConfiguration with");
                return sb;
            }

            var expectedText = CreateStringBuilder();
            var actualText = CreateStringBuilder();
            var notEqual = false;

            void AppendLine(string format, object expectedValue, object actualValue, Func<bool> equal = null, Func<object, string> print = null)
            {
                equal = equal ?? (() => Equals(expectedValue, actualValue));
                print = print ?? (o => o.ToString());

                if (!equal())
                {
                    notEqual = true;
                    expectedText.AppendLine(string.Format(format, print(expectedValue)));
                    actualText.AppendLine(string.Format(format, print(actualValue)));
                }
            }

            AppendLine($"\t.{nameof(DashboardConfiguration.Id)}           = '{{0}}'", expected.Id, actual.Id);
            AppendLine($"\t.{nameof(DashboardConfiguration.Name)}         = '{{0}}'", expected.Name, actual.Name);
            AppendLine($"\t.{nameof(DashboardConfiguration.Team)}         = '{{0}}'", expected.Team, actual.Team);
            AppendLine($"\t.{nameof(DashboardConfiguration.Environments)} = [{{0}}]", expected.Environments, actual.Environments, () => expected.Environments.SequenceEqual(actual.Environments), o => string.Join(",", (string[]) o));

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}