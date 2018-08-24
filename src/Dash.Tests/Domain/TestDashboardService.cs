using System;
using Dash.Domain;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;
using Dash.Tests.TestDoubles;
using Xunit;

namespace Dash.Tests.Domain
{
    public class TestDashboardService
    {
        [Fact]
        public void Can_build_dashboard_from_sources()
        {
            var stubVersion = new FakeFileSystem(
                $"{FileVersionRepository.Headers}\n" +
                "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
            );
            var stubSettings =
                new FakeFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|aws-account-billing.json|C|x|x||");
            var stubContent = new FakeFileSystem("{\"id\": 1}");
            var dashboardVersionRepository = new FileVersionRepository(stubVersion);
            var dashboardConfigurationRepository = new DashboardConfigurationRepository(stubSettings);
            var sut = new DashboardService(dashboardVersionRepository, dashboardConfigurationRepository, stubContent);

            var dashboards = sut.GetAll();

            var dashboard = Assert.Single(dashboards);

            Assert.NotNull(dashboard);
            Assert.Equal("A", dashboard.Id);
            Assert.Equal("aws-account-billing.json", dashboard.Name);
            Assert.Equal("C", dashboard.Team);
            Assert.Equal(new DateTime(2018, 8, 8, 9, 10, 36, DateTimeKind.Utc), dashboard.LastModified);
            Assert.Equal("{}", dashboard.Content);
        }

        [Fact]
        public void Can_save_dashboard_content()
        {
            const string dashboardName = "new-dashboard.json";
            const string dashboardContent = "{}";

            var dummyFileSystem = new FakeFileSystem();
            var spyContent = new FakeFileSystem();
            var dashboardVersionRepository = new FileVersionRepository(dummyFileSystem);
            var dashboardConfigurationRepository = new DashboardConfigurationRepository(dummyFileSystem);
            var sut = new DashboardService(dashboardVersionRepository, dashboardConfigurationRepository, spyContent);

            var dashboard = A.Dashboard
                .WithName(dashboardName)
                .WithContent(dashboardContent)
                .Build();

            sut.Save(dashboard);

            Assert.Equal(dashboardName, spyContent.WrittenPath);
            Assert.Equal(dashboardContent, spyContent.WrittenContent);
        }

        [Fact]
        public void Can_save_dashboard_configuration()
        {
            var dummyFileSystem = new FakeFileSystem();
            var spyContent = new FakeFileSystem();
            var dashboardVersionRepository = new FileVersionRepository(dummyFileSystem);
            var dashboardConfigurationRepository = new DashboardConfigurationRepository(spyContent);
            var sut = new DashboardService(dashboardVersionRepository, dashboardConfigurationRepository, dummyFileSystem);

            var dashboard = A.Dashboard
                .WithId("id")
                .WithName("my-dashboard")
                .WithTeam("my-team")
                .Build();

            sut.Save(dashboard);

            Assert.Equal(DashboardConfigurationRepository.DashboardConfigurationFileName, spyContent.WrittenPath);
            Assert.Equal("|Id|Name|Team|\n|-|-|-|\n|id|my-dashboard|my-team|", spyContent.WrittenContent);
        }
    }
}