using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;

namespace Dash.Tests.TestBuilders
{
    internal class FileVersionRepositoryBuilder
    {
        private FileVersionParser _fileVersionParser = FileVersionParser.CreateNull();

        public FileVersionRepositoryBuilder With(FileVersionParser fileVersionParser)
        {
            _fileVersionParser = fileVersionParser;
            return this;
        }

        public FileVersionRepository Build()
        {
            return new FileVersionRepository(_fileVersionParser);
        }

        public static implicit operator FileVersionRepository(FileVersionRepositoryBuilder builder)
        {
            return builder.Build();
        }
    }
}