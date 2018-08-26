using Dash.Domain;
using Dash.Tests.Infrastructure.Configuration;

namespace Dash.Tests.Domain
{
    internal static class A
    {
        public static DashboardBuilder Dashboard => new DashboardBuilder();
        public static FakeFileBuilder File => new FakeFileBuilder();
    }
}