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
            return GetStuff().Configurations;
        }

        private Stuff GetStuff()
        {
            var lines = _fileSystem.GetFile(DashboardConfigurationFileName).ReadLines();

            var markdownTable = MarkdownParser.ParseMarkdownTable(lines);

            if (!MandatoryHeaders.SequenceEqual(markdownTable.Headers.Take(3)))
            {
                throw new Exception("Headers do not match");
            }

            var environments = markdownTable.Headers.Skip(MandatoryHeaders.Length).ToArray();

            var configurations = new List<DashboardConfiguration>();

            foreach (var items in markdownTable.Lines)
            {
                configurations.Add(new DashboardConfigurationBuilder()
                    .WithId(items[0])
                    .WithName(items[1])
                    .WithTeam(items[2])
                    .WithEnvironments(BuildEnvironmentMap(items.Skip(MandatoryHeaders.Length), environments).ToArray()));
            }


            return new Stuff(environments, configurations);
        }

        public class Stuff
        {
            public Stuff(string[] environments, List<DashboardConfiguration> configurations)
            {
                Environments = environments;
                Configurations = configurations;
            }

            public string[] Environments { get; }
            public List<DashboardConfiguration> Configurations { get; }
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
            var stuff = GetStuff();
            var dashboardConfigurations = stuff.Configurations.ToList();

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

            var markdownTable = CreateMarkdownTable(
                environments: MandatoryHeaders.Concat(stuff.Environments).ToArray(),
                dashboardConfigurations: dashboardConfigurations);

            _fileSystem.GetFile(DashboardConfigurationFileName).WriteLines(markdownTable);
        }

        public bool Remove(string id)
        {
            var stuff = GetStuff();
            var dashboardConfigurations = stuff.Configurations.ToList();

            var index = dashboardConfigurations.FindIndex(x => x.Id == id);

            if (index != -1)
            {
                dashboardConfigurations.RemoveAt(index);

                var markdownTable = CreateMarkdownTable(
                    environments: MandatoryHeaders.Concat(stuff.Environments).ToArray(),
                    dashboardConfigurations: dashboardConfigurations);
                _fileSystem.GetFile(DashboardConfigurationFileName).WriteLines(markdownTable);
                return true;
            }

            return false;
        }

        #region Create lines from DashboardConfiguration

        public static IEnumerable<string> CreateMarkdownTable(string[] environments, List<DashboardConfiguration> dashboardConfigurations)
        {
            var markdownTable = PrepareForMarkdown(environments, dashboardConfigurations);
            return MarkdownParser.BuildMarkdownTable(markdownTable);
        }

        private static MarkdownTable PrepareForMarkdown(string[] environments, List<DashboardConfiguration> dashboardConfigurations)
        {
            var headers = GetHeaders(environments, dashboardConfigurations);

            return new MarkdownTable(headers, GetLines(dashboardConfigurations, headers).ToArray());
        }

        private static string[] GetHeaders(string[] headerEnvironments, IEnumerable<DashboardConfiguration> dashboardConfigurations)
        {
            var actualEnvironments = new HashSet<string>(dashboardConfigurations.SelectMany(x => x.Environments).Select(x => x.Name));
            if (actualEnvironments.Count == 0)
            {
                return MandatoryHeaders;
            }

            var tempList = MandatoryHeaders.ToList();

            foreach (var environment in headerEnvironments)
            {
                if (actualEnvironments.Contains(environment))
                {
                    tempList.Add(environment);
                }
            }

            actualEnvironments.ExceptWith(headerEnvironments);

            tempList.AddRange(actualEnvironments.OrderBy(x => x));

            return tempList.ToArray();
        }

        private static IEnumerable<string[]> GetLines(List<DashboardConfiguration> dashboardConfigurations, string[] headers)
        {
            foreach (var dashboardConfiguration in dashboardConfigurations)
            {
                yield return GetLine(headers, dashboardConfiguration).ToArray();
            }
        }

        private static IEnumerable<string> GetLine(string[] headers, DashboardConfiguration dashboardConfiguration)
        {
            yield return dashboardConfiguration.Id;
            yield return dashboardConfiguration.Name;
            yield return dashboardConfiguration.Team;

            foreach (var environment in headers.Skip(MandatoryHeaders.Length))
            {
                var env = dashboardConfiguration.Environments.FirstOrDefault(x => x.Name == environment);
                yield return env?.Enabled == EnvironmentState.Enabled ? "x" : "";
            }
        }

        #endregion
    }
}