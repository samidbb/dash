using System;
using System.Linq;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Dash.Tests.TestHelpers;
using Xunit;

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
        [InlineData("")]
        [InlineData("|Id|Name")]
        [InlineData("|Id|Name|Team|")]
        [InlineData("|Id|Name|Team|Env1|Env2|Env3|")]
        [InlineData("|Id|Name|\n|-|-|\n|A|B|")]
        public void Throws_on_bad_formatted_header(string input)
        {
            
            var sut = A.DashboardConfigurationRepository.With(
                FileSystem.CreateNull(A.File.WithContent(input))
            ).Build();

            Assert.Throws<Exception>(() => sut.GetAll());
        }

        [Theory]
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
        public void Can_add_new_dashboard_with_environments()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("NewId")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .WithEnvironments(
                    A.Environment.WithName("Env1").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env2").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env3").WithState(EnvironmentState.Enabled))
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|NewId|NewName|NewTeam|x|x|x|", file.Content);
        }

        [Fact]
        public void Can_add_new_dashboard_without_environments()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("NewId")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal("|Id|Name|Team|\n|-|-|-|\n|NewId|NewName|NewTeam|", file.Content);
        }

        [Fact]
        public void Can_add_new_dashboard_with_new_environments()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|\n|Id1|Name1|Team1|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("NewId")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .WithEnvironments(
                    A.Environment.WithName("NewEnvironment1").WithState(EnvironmentState.Disabled),
                    A.Environment.WithName("NewEnvironment2").WithState(EnvironmentState.Enabled)
                )
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|NewEnvironment1|NewEnvironment2|\n|-|-|-|-|-|-|-|-|\n|Id1|Name1|Team1|x|x||||\n|NewId|NewName|NewTeam|||||x|", file.Content);
        }

        [Fact]
        public void Can_replace_existing_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|NewEnvironment|\n|-|-|-|\n|ExistingId|Name1|Team1|x|x|||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("ExistingId")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .WithEnvironments(
                    A.Environment.WithName("Env1").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env2").WithState(EnvironmentState.Enabled),
                    A.Environment.WithName("Env3").WithState(EnvironmentState.Enabled))
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|ExistingId|NewName|NewTeam|x|x|x|", file.Content);
        }

        [Fact]
        public void Can_append_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|Name1|Team1|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var dashboardConfiguration = A.DashboardConfiguration
                .WithId("NewId")
                .WithName("NewName")
                .WithTeam("NewTeam")
                .WithEnvironments(
                    A.Environment.WithName("Env1"),
                    A.Environment.WithName("Env2"),
                    A.Environment.WithName("Env3").WithState(EnvironmentState.Enabled))
                .Build();

            sut.Save(dashboardConfiguration);

            Assert.Equal("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|Name1|Team1|x|x||\n|NewId|NewName|NewTeam|||x|", file.Content);
        }

        [Fact]
        public void Can_remove_dashboard()
        {
            var file = A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|A|B|C|x|x||").Build();
            var sut = A.DashboardConfigurationRepository.With(FileSystem.CreateNull(file)).Build();

            var wasRemoved = sut.Remove("A");

            Assert.True(wasRemoved);
            Assert.Equal("|Id|Name|Team|\n|-|-|-|", file.Content);
        }

        [Fact]
        public void Will_not_save_when_trying_to_remove_a_non_existing_dashboard()
        {
            var sut = A.DashboardConfigurationRepository.With(
                FileSystem.CreateNull(
                    A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|-|-|-|-|-|-|\n|Id1|Name1|Team1|x|x||")
                )
            ).Build();

            var removed = sut.Remove("UnknownId");

            Assert.False(removed);
        }
    }
}