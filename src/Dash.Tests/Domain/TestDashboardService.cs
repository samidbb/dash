using System;
using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;
using Xunit;

namespace Dash.Tests.Domain
{
    public class TestDashboardService
    {
        [Fact]
        public void Can_build_dashboard_from_sources()
        {
            var sut = A.DashboardService
                .With(A.FileVersionRepository.With(
                    A.FileVersionParser.With(FileSystem.CreateNull(A.File.WithContent(
                        $"{FileVersionParser.Headers}\n" +
                        "e;h;an;ae;2018-08-08 10:10:36 +0100;cn;ce;2018-08-08 10:10:36 +0100;m"
                    ))))
                )
                .With(A.DashboardConfigurationRepository
                    .With(FileSystem.CreateNull(
                        A.File.WithContent("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|Id1|e|Team1|x|x||")))
                )
                .With(FileSystem.CreateNull(A.File.WithContent("{\"id\": 1}")))
                .Build();

            var dashboards = sut.GetAll();

            var dashboard = Assert.Single(dashboards);

            Assert.NotNull(dashboard);
            Assert.Equal("Id1", dashboard.Id);
            Assert.Equal("e", dashboard.Name);
            Assert.Equal("Team1", dashboard.Team);
            Assert.Equal(new DateTime(2018, 8, 8, 9, 10, 36, DateTimeKind.Utc), dashboard.LastModified);
            Assert.Equal("{}", dashboard.Content);
        }

        [Fact]
        public void Can_save_dashboard_content()
        {
            const string dashboardName = "new-dashboard.json";
            const string dashboardContent = "{}";

            var spy = A.File.Build();

            var sut = A.DashboardService
                .With(A.FileVersionRepository)
                .With(A.DashboardConfigurationRepository)
                .With(FileSystem.CreateNull(spy))
                .Build();

            var dashboard = A.Dashboard
                .WithName(dashboardName)
                .WithContent(dashboardContent)
                .Build();

            sut.Save(dashboard);

            Assert.Equal(dashboardContent, spy.Content);
        }

        [Fact]
        public void Can_save_dashboard_configuration()
        {
            var spy = A.File.Build();
            var sut = A.DashboardService
                .With(A.FileVersionRepository)
                .With(A.DashboardConfigurationRepository
                    .With(FileSystem.CreateNull(spy))
                )
                .With(FileSystem.CreateNull())
                .Build();

            var dashboard = A.Dashboard
                .WithId("id")
                .WithName("my-dashboard")
                .WithTeam("my-team")
                .Build();

            sut.Save(dashboard);

            Assert.Equal("|Id|Name|Team|\n|-|-|-|\n|id|my-dashboard|my-team|", spy.Content);
        }
    }
}