using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public static class DashboardConfigurationParser
    {
        private static readonly string[] MandatoryHeaders = {"Id", "Name", "Team"};

        public static IEnumerable<string> GetLinesFromStuff(Stuff stuff)
        {
            var markdownTable = ToMarkdownTable(stuff);

            return MarkdownParser.BuildMarkdownTable(markdownTable);
        }

        #region GetLinesFromStuff

        private static MarkdownTable ToMarkdownTable(Stuff stuff)
        {
            var headers = GetHeaders(stuff);

            var lines = GetLines(stuff).ToArray();

            return new MarkdownTable(headers, lines);
        }

        private static string[] GetHeaders(Stuff stuff)
        {
            return MandatoryHeaders.Concat(stuff.GetEnvironments()).ToArray();
        }

        private static IEnumerable<string[]> GetLines(Stuff stuff)
        {
            var environments = stuff.GetEnvironments().ToArray();

            foreach (var dashboardConfiguration in stuff.Configurations)
            {
                yield return GetLine(environments, dashboardConfiguration).ToArray();
            }
        }

        private static IEnumerable<string> GetLine(string[] environments, DashboardConfiguration dashboardConfiguration)
        {
            yield return dashboardConfiguration.Id;
            yield return dashboardConfiguration.Name;
            yield return dashboardConfiguration.Team;

            foreach (var environment in environments)
            {
                var env = dashboardConfiguration.Environments.FirstOrDefault(x => x.Name == environment);
                yield return env?.Enabled == EnvironmentState.Enabled ? "x" : "";
            }
        }

        #endregion

        public static Stuff GetStuffFromLines(IEnumerable<string> lines)
        {
            var markdownTable = ParseMarkdownTable(lines);

            VerifyMarkdownTable(markdownTable);

            return CreateStuff(markdownTable);
        }

        #region GetStuffFromLines

        private static MarkdownTable ParseMarkdownTable(IEnumerable<string> lines)
        {
            return MarkdownParser.ParseMarkdownTable(lines);
        }

        private static void VerifyMarkdownTable(MarkdownTable markdownTable)
        {
            if (!MandatoryHeaders.SequenceEqual(markdownTable.Headers.Take(MandatoryHeaders.Length)))
            {
                throw new Exception("Headers do not match");
            }
        }

        private static Stuff CreateStuff(MarkdownTable markdownTable)
        {
            var environments = GetEnvironmentsFromHeaders(markdownTable.Headers);

            var configurations = BuildDashboardConfigurationsFromLines(environments, markdownTable.Lines);

            return new Stuff(environments, configurations.ToList());
        }

        private static string[] GetEnvironmentsFromHeaders(IEnumerable<string> headers)
        {
            return headers.Skip(MandatoryHeaders.Length).ToArray();
        }

        private static IEnumerable<DashboardConfiguration> BuildDashboardConfigurationsFromLines(string[] environments, IEnumerable<string[]> markdownTableLines)
        {
            foreach (var items in markdownTableLines)
            {
                yield return new DashboardConfigurationBuilder()
                        .WithId(items[0])
                        .WithName(items[1])
                        .WithTeam(items[2])
                        .WithEnvironments(GetEnvironmentList(environments, items.Skip(MandatoryHeaders.Length)).ToArray())
                    ;
            }
        }

        private static IEnumerable<Environment> GetEnvironmentList(string[] environments, IEnumerable<string> items)
        {
            var i = 0;

            foreach (var item in items)
            {
                yield return new EnvironmentBuilder()
                    .WithName(environments[i])
                    .WithState(item == "x" ? EnvironmentState.Enabled : EnvironmentState.Disabled);

                i++;
            }

            while (i < environments.Length)
            {
                yield return new EnvironmentBuilder()
                    .WithName(environments[i]);

                i++;
            }
        }

        #endregion
    }
}