using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfigurationRepository
    {
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

            var isHeaderOk = false;
            var environments = new string[0];

            foreach (var row in rows)
            {
                var items = row.Split('|');
                if (items.Length < 4)
                {
                    throw new Exception("Too few columns");
                }

                if (!isHeaderOk)
                {
                    if (!"Id|Name|Team".Split('|').SequenceEqual(items.Take(3)))
                    {
                        throw new Exception("Headers do not match");
                    }

                    isHeaderOk = true;
                    environments = items.Skip(3).ToArray();
                    continue;
                }

                yield return new DashboardConfigurationBuilder()
                    .WithId(items[0])
                    .WithName(items[1])
                    .WithTeam(items[3])
                    .WithEnvironments(items.Skip(3).Select((x, i) => x == "x" ? environments[i] : null).Where(x => x != null).ToArray());
            }
        }
    }
}