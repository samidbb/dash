using System.Collections.Generic;
using Dash.Infrastructure;

namespace Dash.Tests.TestDoubles
{
    internal class FakeFile : File
    {
        public FakeFile(string path, string content) : base(path)
        {
            Content = content;
        }

        public string Content { get; private set; }

        public override string ReadAllText()
        {
            return Content;
        }

        public override IEnumerable<string> ReadLines()
        {
            return Content.Split('\n');
        }

        public override void WriteAllText(string content)
        {
            Content = content;
        }

        public override void WriteLines(IEnumerable<string> content)
        {
            Content = string.Join('\n', content);
        }
    }
}