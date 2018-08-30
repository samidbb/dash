using System.Collections.Generic;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfigurationRepository
    {
        public const string DashboardConfigurationFileName = "DASHBOARDS.md";

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
            var lines = LoadLinesFromFile();

            return DashboardConfigurationParser.GetStuffFromLines(lines);
        }

        private IEnumerable<string> LoadLinesFromFile()
        {
            return _fileSystem
                .GetFile(DashboardConfigurationFileName)
                .ReadLines();
        }

        public void Save(DashboardConfiguration dashboardConfiguration)
        {
            Stuff stuff = GetStuff();

            stuff.AddOrUpdate(dashboardConfiguration);

            SaveToFile(stuff);
        }

        private void SaveToFile(Stuff stuff)
        {
            var lines = DashboardConfigurationParser.GetLinesFromStuff(stuff);

            WriteLines(lines);
        }

        private void WriteLines(IEnumerable<string> lines)
        {
            _fileSystem
                .GetFile(DashboardConfigurationFileName)
                .WriteLines(lines);
        }

        public bool Remove(string id)
        {
            var stuff = GetStuff();

            if (stuff.Remove(id))
            {
                SaveToFile(stuff);
                return true;
            }

            return false;
        }
    }
}