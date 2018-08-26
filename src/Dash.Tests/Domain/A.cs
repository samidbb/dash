using Dash.Domain;
using Dash.Tests.TestBuilders;

namespace Dash.Tests.Domain
{
    internal static class A
    {
        public static DashboardBuilder Dashboard => new DashboardBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
        public static DashboardServiceBuilder DashboardService => new DashboardServiceBuilder();
        public static DashboardConfigurationRepositoryBuilder DashboardConfigurationRepository => new DashboardConfigurationRepositoryBuilder();
        public static FileVersionRepositoryBuilder FileVersionRepository => new FileVersionRepositoryBuilder();
    }
}