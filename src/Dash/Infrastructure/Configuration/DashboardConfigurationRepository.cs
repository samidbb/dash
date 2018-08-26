using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfigurationRepository
    {
        public const string DashboardConfigurationFileName = "DASHBOARDS.md";

        private static readonly string[] MandatoryHeaders = {"Id", "Name", "Team"};

        private readonly FileSystem _fileSystem;

        public DashboardConfigurationRepository(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<DashboardConfiguration> GetAll()
        {
            var lines = _fileSystem.GetFile(DashboardConfigurationFileName).ReadLines();
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

        private static IEnumerable<Environment> BuildEnvironmentMap(IEnumerable<string> items, string[] environments)
        {
            var i = 0;

            foreach (var item in items)
            {
                yield return new EnvironmentBuilder().WithName(environments[i]).WithState(item == "x" ? EnvironmentState.Enabled : EnvironmentState.Disabled);
                i++;
            }

            while (i < environments.Length)
            {
                yield return new EnvironmentBuilder().WithName(environments[i]);
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

            _fileSystem.GetFile(DashboardConfigurationFileName).WriteLines(CreateMarkdownTable(dashboardConfigurations));
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
            tempList.AddRange(configuration.Environments.Select(x => x.Name));

            return tempList.ToArray();
        }

        private static IEnumerable<string> GetLine(DashboardConfiguration dashboardConfiguration)
        {
            yield return dashboardConfiguration.Id;
            yield return dashboardConfiguration.Name;
            yield return dashboardConfiguration.Team;

            foreach (var environment in dashboardConfiguration.Environments)
            {
                yield return environment.Enabled==EnvironmentState.Enabled ? "x" : "";
            }
        }

        public bool Remove(string id)
        {
            var dashboardConfigurations = GetAll().ToList();

            var index = dashboardConfigurations.FindIndex(x => x.Id == id);

            if (index != -1)
            {
                dashboardConfigurations.RemoveAt(index);
                _fileSystem.GetFile(DashboardConfigurationFileName).WriteLines(CreateMarkdownTable(dashboardConfigurations));
                return true;
            }

            return false;
        }
    }
}