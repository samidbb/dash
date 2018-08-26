using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;

namespace Dash.Tests.TestBuilders
{
    internal class FileVersionRepositoryBuilder
    {
        private FileSystem _fileSystem = FileSystem.CreateNull();

        public FileVersionRepositoryBuilder With(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        public FileVersionRepository Build()
        {
            return new FileVersionRepository(_fileSystem);
        }

        public static implicit operator FileVersionRepository(FileVersionRepositoryBuilder builder)
        {
            return builder.Build();
        }
    }
}