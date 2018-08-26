using Dash.Domain;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;
using Dash.Tests.Infrastructure.Configuration;

namespace Dash.Tests.Domain
{
    internal static class A
    {
        public static DashboardBuilder Dashboard => new DashboardBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
        public static DashboardServiceBuilder DashboardService => new DashboardServiceBuilder();
        public static DashboardConfigurationRepositoryBuilder DashboardConfigurationRepository => new DashboardConfigurationRepositoryBuilder();
    }
    
    public class DashboardServiceBuilder
    {
        private FileVersionRepository _fileVersionRepository;
        private DashboardConfigurationRepository _dashboardConfigurationRepository;
        private FileSystem _fileSystem;

        public DashboardServiceBuilder With(FileVersionRepository fileVersionRepository)
        {
            _fileVersionRepository = fileVersionRepository;
            return this;
        }

        public DashboardServiceBuilder With(DashboardConfigurationRepository dashboardConfigurationRepository)
        {
            _dashboardConfigurationRepository = dashboardConfigurationRepository;
            return this;
        }

        public DashboardServiceBuilder With(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        public DashboardService Build()
        {
            return new DashboardService(_fileVersionRepository, _dashboardConfigurationRepository, _fileSystem);
        }

        public static implicit operator DashboardService(DashboardServiceBuilder builder)
        {
            return builder.Build();
        }
    }

}
