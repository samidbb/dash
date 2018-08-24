using System.Collections.Generic;
using System.IO;

namespace Dash.Infrastructure
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        IEnumerable<string> ReadLines(string path);
        void WriteAllText(string path, string content);
        void WriteLines(string path, IEnumerable<string> content);
        void Delete(string path);
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

        public void WriteAllText(string path, string content)
        {
            path = Path.Combine(_rootPath, path);

            File.WriteAllText(path, content);
        }

        public void WriteLines(string path, IEnumerable<string> content)
        {
            path = Path.Combine(_rootPath, path);

            File.WriteAllLines(path, content);
        }

        public void Delete(string path)
        {
            path = Path.Combine(_rootPath, path);

            File.Delete(path);
        }
    }
}