using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dash.Infrastructure.Versioning
{
    public class FileVersionParser
    {
        public const string FileVersionCsvFileName = "dashboard_version.csv";
        public const string Headers = "entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message";

        public static FileVersionParser CreateNull()
        {
            return new NullFileVersionParser(Enumerable.Empty<FileVersion>());
        }

        public static FileVersionParser CreateNull(params FileVersion[] fileVersions)
        {
            return new NullFileVersionParser(fileVersions);
        }

        private readonly FileSystem _fileSystem;

        public FileVersionParser(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public virtual IEnumerable<FileVersion> ParseFileVersions()
        {
            var lines = _fileSystem.GetFile(FileVersionCsvFileName).ReadLines().ToList();
            if (lines.Count < 2)
            {
                yield break;
            }

            if (!Headers.Equals(lines[0]))
            {
                yield break;
            }

            foreach (var line in lines.Skip(1))
            {
                var tokens = line.Split(';');
                if (tokens.Length != 9)
                {
                    continue;
                }

                yield return new FileVersionBuilder()
                    .WithEntry(tokens[0])
                    .WithHash(tokens[1])
                    .WithAuthorName(tokens[2])
                    .WithAuthorEmail(tokens[3])
                    .WithAuthorDate(DateTime.Parse(tokens[4], CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal))
                    .WithCommitterName(tokens[5])
                    .WithCommitterEmail(tokens[6])
                    .WithCommitterDate(DateTime.Parse(tokens[7], CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal))
                    .WithMessage(tokens[8])
                    .Build();
            }
        }
        
        private class NullFileVersionParser : FileVersionParser
        {
            private readonly IEnumerable<FileVersion> _fileVersions;

            public NullFileVersionParser(IEnumerable<FileVersion> fileVersions) : base(FileSystem.CreateNull())
            {
                _fileVersions = fileVersions;
            }

            public override IEnumerable<FileVersion> ParseFileVersions()
            {
                return _fileVersions;
            }
        }
    }
}