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
            if (Equals(expected, actual))
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);

            StringBuilder CreateStringBuilder()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{nameof(DashboardConfiguration)} with");
                return sb;
            }

            var expectedText = CreateStringBuilder();
            var actualText = CreateStringBuilder();
            var notEqual = false;

            void AppendLine<T>(string format, Func<DashboardConfiguration, T> selector, Func<bool> equal = null, Func<T, string> print = null)
            {
                var expectedValue = selector(expected);
                var actualValue = selector(actual);

                equal = equal ?? (() => Equals(expectedValue, actualValue));
                print = print ?? (o => o.ToString());

                if (!equal())
                {
                    notEqual = true;
                    expectedText.AppendLine(string.Format(format, print(expectedValue)));
                    actualText.AppendLine(string.Format(format, print(actualValue)));
                }
            }

            AppendLine($"\t.{nameof(DashboardConfiguration.Id)}           = '{{0}}'", x => x.Id);
            AppendLine($"\t.{nameof(DashboardConfiguration.Name)}         = '{{0}}'", x => x.Name);
            AppendLine($"\t.{nameof(DashboardConfiguration.Team)}         = '{{0}}'", x => x.Team);
            AppendLine($"\t.{nameof(DashboardConfiguration.Environments)} = [{{0}}]", x => x.Environments, () => expected.Environments.SequenceEqual(actual.Environments), o => string.Join(",", o.Select(x => x.ToString())));

            if (notEqual)
            {
                throw new EqualException(expectedText, actualText);
            }
        }
    }
}