using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class MarkdownTable : IEnumerable<string[]>
    {
        public MarkdownTable(string[] headers, string[][] lines)
        {
            Headers = headers;
            Lines = lines;
        }

        public string[] Headers { get; }
        public string[][] Lines { get; }

        public IEnumerator<string[]> GetEnumerator()
        {
            if (Headers == null)
            {
                yield break;
            }

            yield return Headers;

            if (Lines == null)
            {
                yield break;
            }
            
            foreach (var line in Lines)
            {
                yield return line;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}