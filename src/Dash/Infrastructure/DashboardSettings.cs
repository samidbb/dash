using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure
{
    public class DashboardSettings
    {
        public static readonly DashboardSettings Empty = new DashboardSettingsBuilder().Build();

        public DashboardSettings(string id, string name, string team, string[] environments)
        {
            Id = id;
            Name = name;
            Team = team;
            Environments = environments;
        }

        public string Id { get; }
        public string Name { get; }
        public string Team { get; }
        public string[] Environments { get; }
    }

    public class DashboardSettingsBuilder
    {
        private string _id;
        private string _name;
        private string _team;
        private string[] _environments;

        public DashboardSettingsBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public DashboardSettingsBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DashboardSettingsBuilder WithTeam(string team)
        {
            _team = team;
            return this;
        }

        public DashboardSettingsBuilder WithEnvironments(params string[] environments)
        {
            _environments = environments;
            return this;
        }

        public DashboardSettings Build()
        {
            return new DashboardSettings(_id ?? "", _name ?? "", _team ?? "", _environments ?? new string[0]);
        }

        public static implicit operator DashboardSettings(DashboardSettingsBuilder builder)
        {
            return builder.Build();
        }
    }

    public class DashboardSettingsRepository
    {
        private readonly IFileSystem _fileSystem;

        public DashboardSettingsRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<DashboardSettings> GetAll()
        {
            var lines = _fileSystem.ReadLines("DASHBOARDS.md");
            return ParseDashboardSettings(lines);
        }

        private static IEnumerable<DashboardSettings> ParseDashboardSettings(IEnumerable<string> lines)
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

                yield return new DashboardSettings(items[0], items[1], items[2], items.Skip(3).Select((x, i) => x == "x" ? environments[i] : null).Where(x => x != null).ToArray());
            }
        }
    }
}