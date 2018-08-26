using System;
using System.Linq;
using System.Text;
using Dash.Infrastructure.Configuration;
using Xunit;
using Xunit.Sdk;

namespace Dash.Tests.TestHelpers
{
    internal static class DashboardConfigurationAssert
    {
        public static void Equal(DashboardConfiguration expected, DashboardConfiguration actual)
        {
            new DashboardConfigurationAssertHelper().Equal(expected, actual);
        }

        private class DashboardConfigurationAssertHelper : AssertHelper<DashboardConfiguration>
        {
            protected override void AssertObject(DashboardConfiguration expected, DashboardConfiguration actual)
            {
                AssertProperty($"\t.{nameof(DashboardConfiguration.Id)}           = '{{0}}'", x => x.Id);
                AssertProperty($"\t.{nameof(DashboardConfiguration.Name)}         = '{{0}}'", x => x.Name);
                AssertProperty($"\t.{nameof(DashboardConfiguration.Team)}         = '{{0}}'", x => x.Team);
                AssertProperty($"\t.{nameof(DashboardConfiguration.Environments)} = [{{0}}]", x => x.Environments, () => expected.Environments.SequenceEqual(actual.Environments), o => string.Join(",", o.Select(x => x.ToString())));
            }
        }
    }
}