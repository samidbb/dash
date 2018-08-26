using Dash.Infrastructure.Versioning;
using Dash.Tests.Infrastructure.Configuration;

namespace Dash.Tests.Infrastructure.Versioning
{
    internal static class A
    {
        public static FileVersionBuilder FileVersion => new FileVersionBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
    }
}