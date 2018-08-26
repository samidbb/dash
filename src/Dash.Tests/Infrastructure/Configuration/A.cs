using System.Collections.Generic;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;

namespace Dash.Tests.Infrastructure.Configuration
{
    internal static class A
    {
        public static DashboardConfigurationBuilder DashboardConfiguration => new DashboardConfigurationBuilder();
        public static EnvironmentBuilder Environment => new EnvironmentBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
        public static DashboardConfigurationRepositoryBuilder DashboardConfigurationRepository => new DashboardConfigurationRepositoryBuilder();
    }

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

    internal class FakeFileBuilder
    {
        private string _content = string.Empty;

        public FakeFileBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public FakeFile Build()
        {
            return new FakeFile(string.Empty, _content);
        }

        public static implicit operator FakeFile(FakeFileBuilder builder)
        {
            return builder.Build();
        }
    }

    internal class FakeFile : File
    {
        public FakeFile(string path, string content) : base(path)
        {
            Content = content;
        }

        public string Content { get; private set; }

        public override string ReadAllText()
        {
            return Content;
        }

        public override IEnumerable<string> ReadLines()
        {
            return Content.Split('\n');
        }

        public override void WriteAllText(string content)
        {
            Content = content;
        }

        public override void WriteLines(IEnumerable<string> content)
        {
            Content = string.Join('\n', content);
        }
    }
}