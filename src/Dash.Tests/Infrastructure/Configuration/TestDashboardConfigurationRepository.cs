using System;
using System.Linq;
using System.Text;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests.Infrastructure.Configuration
{
    public class TestDashboardConfigurationRepository
    {
        [Fact]
        public void Has_correct_dashboard_configuration_filename()
        {
            // TODO -- inject filename from settings
            Assert.Equal("DASHBOARDS.md", DashboardConfigurationRepository.DashboardConfigurationFileName);
        }
        
        [Theory]
        [InlineData("", 0)]
        [InlineData("|Id|Name|Team", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n", 0)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|Name1|Team1||||", 1)]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|Name1|Team1||||\n|Id2|Name2|Team2||||", 2)]
        public void Has_expected_number_of_rows(string input, int expectedRows)
        {
            var sut = A.DashboardConfigurationRepository.With(
                FileSystem.CreateNull(A.File.WithContent(input))
            ).Build();

            var dashboards = sut.GetAll();

            Assert.Equal(expectedRows, dashboards.Count());
        }

        [Fact]
        public void Can_parse_settings()
        {
            var sut = A.DashboardConfigurationRepository.With(
                FileSystem.CreateNull(A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|Id1|Name1|Team1|x|x||"))
            ).Build();

            var result = sut.GetAll().SingleOrDefault();

            DashboardConfigurationAssert.Equal(
                expected: A.DashboardConfiguration
                    .WithId("Id1")
                    .WithName("Name1")
                    .WithTeam("Team1")
                    .WithEnvironments(
                        A.Environment.WithName("Env1").WithState(EnvironmentState.Enabled),
                        A.Environment.WithName("Env2").WithState(EnvironmentState.Enabled),
                        A.Environment.WithName("Env3")
                    ),
                actual: result
            );
        }

        [Fact]
        public void Can_replace_existing_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|Id1|Name1|Team1|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("Id1")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .WithEnvironments(
                    A.Environment.WithName("Env1").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env2").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env3").WithState(EnvironmentState.Enabled))
                .Build();

            sut.Save(dashboardConfiguration);

//            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, file.Path);
            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|NewName|NewTeam|x|x|x|", file.Content);
        }

        [Fact]
        public void Can_append_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("AA")
                .WithName("B")
                .WithTeam("C")
                .WithEnvironments(
                    A.Environment.WithName("Env1"), 
                    A.Environment.WithName("Env2"), 
                    A.Environment.WithName("Env3").WithState(EnvironmentState.Enabled))
                .Build();

            sut.Save(dashboardConfiguration);

//            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, file.Path);
            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||\n|AA|B|C|||x|", file.Content);
        }

        [Fact]
        public void Can_remove_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            bool removed = sut.Remove("A");

            Assert.True(removed);
//            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, file.Path);
            Assert.Equal("|Id|Name|Team|\n|-|-|-|", file.Content);
        }

        [Fact]
        public void Will_not_save_when_trying_to_remove_a_non_existing_dashboard()
        {
            var sut = A.DashboardConfigurationRepository.With(
                FileSystem.CreateNull(
                    A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||")
                )
            ).Build();

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
                sb.AppendLine($"{nameof(DashboardConfiguration)} with");
                return sb;
            }

            var expectedText = CreateStringBuilder();
            var actualText = CreateStringBuilder();
            var notEqual = false;

            void AppendLine<T>(string format, Func<DashboardConfiguration, T> selector, Func<bool> equal = null, Func<T, string> print = null)
            {
                var expectedValue = selector(expected);
                var actualValue = selector(actual);

                equal = equal ?? (() => Equals(expectedValue, actualValue));
                print = print ?? (o => o.ToString());

                if (!equal())
                {
                    notEqual = true;
                    expectedText.AppendLine(string.Format(format, print(expectedValue)));
                    actualText.AppendLine(string.Format(format, print(actualValue)));
                }
            }

            AppendLine($"\t.{nameof(DashboardConfiguration.Id)}           = '{{0}}'", x => x.Id);
            AppendLine($"\t.{nameof(DashboardConfiguration.Name)}         = '{{0}}'", x => x.Name);
            AppendLine($"\t.{nameof(DashboardConfiguration.Team)}         = '{{0}}'", x => x.Team);
            AppendLine($"\t.{nameof(DashboardConfiguration.Environments)} = [{{0}}]", x => x.Environments, () => expected.Environments.SequenceEqual(actual.Environments), o => string.Join(",", o.Select(x => x.ToString())));

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}