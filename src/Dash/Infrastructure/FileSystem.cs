using System.Collections.Generic;
using System.IO;

namespace Dash.Infrastructure
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        IEnumerable<string> ReadLines(string path);
        void WriteAllText(string name, string content);
        void WriteLines(string name, IEnumerable<string> content);
    }

    public class FileSystem : IFileSystem
    {
        private readonly string _rootPath;

        public FileSystem(Settings settings)
        {
            _rootPath = settings.Root;
        }

        public string ReadAllText(string path)
        {
            path = Path.Combine(_rootPath, path);

            return File.ReadAllText(path);
        }

        public IEnumerable<string> ReadLines(string path)
        {
            path = Path.Combine(_rootPath, path);

            return File.ReadLines(path);
        }

        public void WriteAllText(string name, string content)
        {
            var path = Path.Combine(_rootPath, name);

            File.WriteAllText(path, content);
        }

        public void WriteLines(string name, IEnumerable<string> content)
        {
            var path = Path.Combine(_rootPath, name);

            File.WriteAllLines(path, content);
        }
    }
}