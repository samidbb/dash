using System;
using System.Collections.Generic;
using Dash.Infrastructure;

namespace Dash.Tests.TestDoubles
{
    public class StubFileSystem : IFileSystem
    {
        private readonly Func<string> _nextFile;

        public StubFileSystem(params string[] files)
        {
            var i = 0;

            _nextFile = () =>
            {
                if (i > files.Length - 1)
                {
                    return null;
                }

                return files[i++];
            };
        }

        public string ReadAllText(string path)
        {
            return _nextFile();
        }

        public IEnumerable<string> ReadLines(string path)
        {
            return _nextFile().Split('\n');
        }
    }
}