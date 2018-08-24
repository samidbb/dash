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
        [Fact]
        public void Has_correct_dashboard_configuration_filename()
        {
            Assert.Equal("DASHBOARDS.md", DashboardConfigurationRepository.DashboardConfigurationFileName);
        }
        
        [Theory]
        [InlineData("", 0)]
        [InlineData("|Id|Name|Team", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||", 1)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||\n|A|B|C|x|x||", 2)]
        public void Has_expected_number_of_rows(string input, int expectedRows)
        {
            var stub = new FakeFileSystem(input);
            var sut = new DashboardConfigurationRepository(stub);

            var dashboards = sut.GetAll().ToList();

            Assert.Equal(expectedRows, dashboards.Count);
        }

        [Fact]
        public void Can_parse_settings()
        {
            var stub = new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            var result = sut.GetAll().FirstOrDefault();

            DashboardConfigurationAssert.Equal(
                expected: A.DashboardConfiguration
                    .WithId("A")
                    .WithName("B")
                    .WithTeam("C")
                    .WithEnvironments(("Env1", true), ("Env2", true), ("Env3", false)),
                actual: result
            );
        }

        [Fact]
        public void Can_overwrite_dashboard()
        {
            var stub = new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("A")
                .WithName("B")
                .WithTeam("C")
                .WithEnvironments(("Env1", true), ("Env2", true), ("Env3", false))
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, stub.WrittenPath);
            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||", stub.WrittenContent);
        }

        [Fact]
        public void Can_append_dashboard()
        {
            var stub = new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("AA")
                .WithName("B")
                .WithTeam("C")
                .WithEnvironments(("Env1", false), ("Env2", false), ("Env3", true))
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, stub.WrittenPath);
            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||\n|AA|B|C|||x|", stub.WrittenContent);
        }

        [Fact]
        public void Can_remove_dashboard()
        {
            var stub = new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            bool removed = sut.Remove("A");

            Assert.True(removed);
            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, stub.WrittenPath);
            Assert.Equal("|Id|Name|Team|\n|-|-|-|", stub.WrittenContent);
        }

        [Fact]
        public void Will_not_save_when_trying_to_remove_a_non_existing_dashboard()
        {
            var stub = new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||");
            var sut = new DashboardConfigurationRepository(stub);

            var removed = sut.Remove("AA");

            Assert.False(removed);
        }
    }

    internal static class DashboardConfigurationAssert
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
            AppendLine($"\t.{nameof(DashboardConfiguration.Environments)} = [{{0}}]", expected.Environments, actual.Environments, () => expected.Environments.SequenceEqual(actual.Environments), o => string.Join(",", ((string environment, bool enabled)[]) o));

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}