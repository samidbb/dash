using System.Collections.Generic;
using Dash.Domain;
using Dash.Infrastructure;

namespace Dash.Tests.TestDoubles
{
    public class FakeFileSystem : IFileSystem
    {
        public string WrittenName { get; private set; }
        public string WrittenContent { get; private set; }

        private readonly string _content;

        public FakeFileSystem() : this(string.Empty)
        {
        }

        public FakeFileSystem(string content)
        {
            _content = content;
        }

        public string ReadAllText(string path)
        {
            return _content;
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return _content.Split('\n');
        }

        public void WriteAllText(string name, string content)
        {
            WrittenName = name;
            WrittenContent = content;
        }

        public void WriteLines(string name, IEnumerable<string> content)
        {
            WrittenName = name;
            WrittenContent = string.Join('\n', content);
        }
    }
}