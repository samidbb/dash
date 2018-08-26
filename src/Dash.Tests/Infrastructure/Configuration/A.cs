using Dash.Infrastructure.Configuration;
using Dash.Tests.TestBuilders;

namespace Dash.Tests.Infrastructure.Configuration
{
    internal static class A
    {
        public static DashboardConfigurationBuilder DashboardConfiguration => new DashboardConfigurationBuilder();
        public static EnvironmentBuilder Environment => new EnvironmentBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
        public static DashboardConfigurationRepositoryBuilder DashboardConfigurationRepository => new DashboardConfigurationRepositoryBuilder();
    }
}