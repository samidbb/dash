using Dash.Infrastructure;
using Dash.Infrastructure.Versioning;

namespace Dash.Tests.TestBuilders
{
    internal class FileVersionParserBuilder
    {
        private FileSystem _fileSystem = FileSystem.CreateNull();

        public FileVersionParserBuilder With(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        public FileVersionParser Build()
        {
            return new FileVersionParser(_fileSystem);
        }

        public static implicit operator FileVersionParser(FileVersionParserBuilder builder)
        {
            return builder.Build();
        }
    }
}