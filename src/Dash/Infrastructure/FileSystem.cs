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
    }
}