using System.Collections.Generic;
using Dash.Infrastructure;

namespace Dash.Tests.TestDoubles
{
    public class StubFileSystem : IFileSystem
    {
        private readonly string _file;

        public StubFileSystem(string file)
        {
            _file = file;
        }

        public string ReadAllText(string path)
        {
            return _file;
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return _file.Split('\n');
        }
    }
}