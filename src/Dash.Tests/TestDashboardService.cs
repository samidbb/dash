using System;
using Dash.Domain;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;
using Dash.Tests.TestDoubles;
using Xunit;

namespace Dash.Tests
{
    public class TestDashboardService
    {
        [Fact]
        public void Test1()
        {
            var stubVersion = new StubFileSystem(
                $"{FileVersionRepository.Headers}\n" +
                "aws-account-billing.json;24083c9fae5cdcd5f707584ce126b98cd1472281;rifisdfds;40063756+rifisdfds@users.noreply.github.com;2018-08-08 10:10:36 +0100;GitHub;noreply@github.com;2018-08-08 10:10:36 +0100;Sorted graph by cost. Changed period to 90 days"
            );
            var stubSettings = new StubFileSystem("|Id|Name|Team|Env1|Env2|Env3|\n|:--|:-:|--:|\n|A|aws-account-billing.json|C|x|x||");
            var stubContent = new StubFileSystem("{}");
            var dashboardVersionRepository = new FileVersionRepository(stubVersion);
            var dashboardSettingsRepository = new DashboardConfigurationRepository(stubSettings);
            var sut = new DashboardService(dashboardVersionRepository, dashboardSettingsRepository, stubContent);

            var dashboards = sut.GetDashboards();

            var dashboard = Assert.Single(dashboards);

            Assert.Equal("A", dashboard.Id);
            Assert.Equal("aws-account-billing.json", dashboard.Name);
            Assert.Equal("C", dashboard.Team);
            Assert.Equal(new DateTime(2018, 8, 8, 9, 10, 36, DateTimeKind.Utc), dashboard.LastModified);
            Assert.Equal("{}", dashboard.Content);
        }
    }
}