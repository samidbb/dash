using System.Collections.Generic;
using System.IO;

namespace Dash.Infrastructure
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        IEnumerable<string> ReadLines(string path);
    }

    public class FileSystem : IFileSystem
    {
        private const string RootPath = @"/Users/twf/projects/dfds/ded-grafana-dashboards";
//        private const string RootPath = @"/projects/ded/ded-grafana-dashboards";

        public static IFileSystem Create()
        {
            return new FileSystem();
        }

        public string ReadAllText(string path)
        {
            path = Path.Combine(RootPath, path);

            return File.ReadAllText(path);
        }


        public IEnumerable<string> ReadLines(string path)
        {
            path = Path.Combine(RootPath, path);

            return File.ReadLines(path);
        }

    }
}