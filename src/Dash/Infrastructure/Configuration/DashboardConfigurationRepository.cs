using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfigurationRepository
    {
        private static readonly string[] MandatoryHeaders = {"Id", "Name", "Team"};

        private readonly IFileSystem _fileSystem;

        public DashboardConfigurationRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<DashboardConfiguration> GetAll()
        {
            var lines = _fileSystem.ReadLines("DASHBOARDS.md");
            return ParseDashboardSettings(lines);
        }

        private static IEnumerable<DashboardConfiguration> ParseDashboardSettings(IEnumerable<string> lines)
        {
            var rows = MarkdownParser.ParseFirstMarkdownTableAsCsvLines(lines);

            var isHeaderOk = 0;
            var environments = new string[0];

            foreach (var items in rows)
            {
                if (items.Length < MandatoryHeaders.Length)
                {
                    throw new Exception("Too few columns");
                }

                if (isHeaderOk == 0)
                {
                    if (!MandatoryHeaders.SequenceEqual(items.Take(3)))
                    {
                        throw new Exception("Headers do not match");
                    }

                    isHeaderOk = items.Length;
                    environments = items.Skip(MandatoryHeaders.Length).ToArray();
                    continue;
                }

                yield return new DashboardConfigurationBuilder()
                        .WithId(items[0])
                        .WithName(items[1])
                        .WithTeam(items[2])
                        .WithEnvironments(BuildEnvironmentMap(items.Skip(MandatoryHeaders.Length), environments).ToArray())
                    ;
            }
        }

        private static IEnumerable<(string environment, bool enabled)> BuildEnvironmentMap(IEnumerable<string> items, string[] environments)
        {
            var i = 0;

            foreach (var item in items)
            {
                yield return (environment: environments[i], enabled: item == "x");
                i++;
            }

            while (i < environments.Length)
            {
                yield return (environment: environments[i], enabled: false);
                i++;
            }
        }

        public void Save(DashboardConfiguration dashboardConfiguration)
        {
            var dashboardConfigurations = GetAll().ToList();

            var i = dashboardConfigurations.FindIndex(x => x.Id == dashboardConfiguration.Id);
            var newDashboardConfig = i == -1;

            if (!newDashboardConfig)
            {
                dashboardConfigurations[i] = dashboardConfiguration;
            }
            else
            {
                dashboardConfigurations.Add(dashboardConfiguration);
            }

            _fileSystem.WriteLines("DASHBOARDS.md", CreateMarkdownTable(dashboardConfigurations));
        }

        private static IEnumerable<string> CreateMarkdownTable(List<DashboardConfiguration> dashboardConfigurations)
        {
            return MarkdownParser.BuildMarkdownTable(PrepareForMarkdown(dashboardConfigurations));
        }

        private static IEnumerable<string[]> PrepareForMarkdown(List<DashboardConfiguration> dashboardConfigurations)
        {
            yield return GetHeaders(dashboardConfigurations);

            foreach (var dashboardConfiguration in dashboardConfigurations)
            {
                yield return GetLine(dashboardConfiguration).ToArray();
            }
        }

        private static string[] GetHeaders(IEnumerable<DashboardConfiguration> dashboardConfigurations)
        {
            var configuration = dashboardConfigurations.FirstOrDefault();
            if (configuration == null)
            {
                return MandatoryHeaders;
            }

            var tempList = MandatoryHeaders.ToList();
            tempList.AddRange(configuration.Environments.Select(x => x.environment));

            return tempList.ToArray();
        }

        private static IEnumerable<string> GetLine(DashboardConfiguration dashboardConfiguration)
        {
            yield return dashboardConfiguration.Id;
            yield return dashboardConfiguration.Name;
            yield return dashboardConfiguration.Team;

            foreach (var environment in dashboardConfiguration.Environments)
            {
                yield return environment.enabled ? "x" : "";
            }
        }
    }
}