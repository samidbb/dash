using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dash.Infrastructure.Versioning
{
    public class FileVersion
    {
        public string Entry { get; set; }
        public string Hash { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime AuthorDate { get; set; }
        public string CommitterName { get; set; }
        public string CommitterEmail { get; set; }
        public DateTime CommitterDate { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return Entry;
        }
    }

    public class FileVersionBuilder
    {
        private string _entry = "";
        private string _hash = "";
        private string _authorName = "";
        private string _authorEmail = "";
        private DateTime _authorDate = DateTime.MinValue;
        private string _committerName = "";
        private string _committerEmail = "";
        private DateTime _committerDate = DateTime.MinValue;
        private string _message = "";

        public FileVersionBuilder WithEntry(string entry)
        {
            _entry = entry;
            return this;
        }

        public FileVersionBuilder WithHash(string hash)
        {
            _hash = hash;
            return this;
        }

        public FileVersionBuilder WithAuthorName(string authorName)
        {
            _authorName = authorName;
            return this;
        }

        public FileVersionBuilder WithAuthorEmail(string authorEmail)
        {
            _authorEmail = authorEmail;
            return this;
        }

        public FileVersionBuilder WithAuthorDate(DateTime authorDate)
        {
            _authorDate = authorDate;
            return this;
        }

        public FileVersionBuilder WithCommitterName(string committerName)
        {
            _committerName = committerName;
            return this;
        }

        public FileVersionBuilder WithCommitterEmail(string committerEmail)
        {
            _committerEmail = committerEmail;
            return this;
        }

        public FileVersionBuilder WithCommitterDate(DateTime committerDate)
        {
            _committerDate = committerDate;
            return this;
        }

        public FileVersionBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public FileVersion Build()
        {
            return new FileVersion
            {
                Entry = _entry,
                Hash = _hash,
                AuthorName = _authorName,
                AuthorEmail = _authorEmail,
                AuthorDate = _authorDate,
                CommitterName = _committerName,
                CommitterEmail = _committerEmail,
                CommitterDate = _committerDate,
                Message = _message
            };
        }

        public static implicit operator FileVersion(FileVersionBuilder builder)
        {
            return builder.Build();
        }
    }

    public class FileVersionRepository
    {
        public const string FileVersionCsvFileName = "dashboard_version.csv";
        public const string Headers = "entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message";

        private readonly IFileSystem _fileSystem;

        private readonly Lazy<List<FileVersion>> _fileVersionList;
        
        public FileVersionRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _fileVersionList = new Lazy<List<FileVersion>>(() => LoadAllFileVersions().ToList());
        }

        public IEnumerable<FileVersion> GetFileVersionList()
        {
            return _fileVersionList.Value;
        }

        private IEnumerable<FileVersion> LoadAllFileVersions()
        {
            var lines = _fileSystem.ReadLines(FileVersionCsvFileName).ToList();
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

        public void Save(FileVersion fileVersion)
        {
            var index = _fileVersionList.Value.FindIndex(x => x.Entry==fileVersion.Entry);
            if (index == -1)
            {
                _fileVersionList.Value.Add(fileVersion);
            }
            else
            {
                _fileVersionList.Value[index] = fileVersion;
            }
        }
    }
}