using Dash.Infrastructure.Versioning;
using Dash.Tests.TestBuilders;

namespace Dash.Tests.Infrastructure.Versioning
{
    internal static class A
    {
        public static FileVersionBuilder FileVersion => new FileVersionBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
        public static FileVersionRepositoryBuilder FileVersionRepository => new FileVersionRepositoryBuilder();
    }
}