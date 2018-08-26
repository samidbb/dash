using Dash.Domain;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;

namespace Dash.Tests.TestBuilders
{
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