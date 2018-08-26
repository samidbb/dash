using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;

namespace Dash.Tests.TestBuilders
{
    internal class DashboardConfigurationRepositoryBuilder
    {
        private FileSystem _fileSystem = FileSystem.CreateNull();

        public DashboardConfigurationRepositoryBuilder With(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        public DashboardConfigurationRepository Build()
        {
            return new DashboardConfigurationRepository(_fileSystem);
        }

        public static implicit operator DashboardConfigurationRepository(DashboardConfigurationRepositoryBuilder builder)
        {
            return builder.Build();
        }
    }
}