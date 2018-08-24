using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfigurationRepository
    {
        private static readonly string[] MandatoryHeader = {"Id", "Name", "Team"};

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

            foreach (var row in rows)
            {
                var items = row.Split('|');
                if (items.Length < MandatoryHeader.Length)
                {
                    throw new Exception("Too few columns");
                }

                if (isHeaderOk == 0)
                {
                    if (!MandatoryHeader.SequenceEqual(items.Take(3)))
                    {
                        throw new Exception("Headers do not match");
                    }

                    isHeaderOk = items.Length;
                    environments = items.Skip(MandatoryHeader.Length).ToArray();
                    continue;
                }

                yield return new DashboardConfigurationBuilder()
                        .WithId(items[0])
                        .WithName(items[1])
                        .WithTeam(items[2])
                        .WithEnvironments(BuildEnvironmentMap(items.Skip(MandatoryHeader.Length), environments).ToArray())
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
    }
}