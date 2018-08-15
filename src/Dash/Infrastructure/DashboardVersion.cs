using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dash.Infrastructure
{
    public class DashboardVersion
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

    public class DashboardVersionBuilder
    {
        private string _entry;

        public DashboardVersionBuilder WithEntry(string entry)
        {
            _entry = entry;
            return this;
        }

        private string _hash;

        public DashboardVersionBuilder WithHash(string hash)
        {
            _hash = hash;
            return this;
        }

        private string _authorName;

        public DashboardVersionBuilder WithAuthorName(string authorName)
        {
            _authorName = authorName;
            return this;
        }

        private string _authorEmail;

        public DashboardVersionBuilder WithAuthorEmail(string authorEmail)
        {
            _authorEmail = authorEmail;
            return this;
        }

        private DateTime _authorDate;

        public DashboardVersionBuilder WithAuthorDate(DateTime authorDate)
        {
            _authorDate = authorDate;
            return this;
        }

        private string _committerName;

        public DashboardVersionBuilder WithCommitterName(string committerName)
        {
            _committerName = committerName;
            return this;
        }

        private string _committerEmail;

        public DashboardVersionBuilder WithCommitterEmail(string committerEmail)
        {
            _committerEmail = committerEmail;
            return this;
        }

        private DateTime _committerDate;

        public DashboardVersionBuilder WithCommitterDate(DateTime committerDate)
        {
            _committerDate = committerDate;
            return this;
        }

        private string _message;

        public DashboardVersionBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public DashboardVersion Build()
        {
            return new DashboardVersion
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

        public static implicit operator DashboardVersion(DashboardVersionBuilder builder)
        {
            return builder.Build();
        }
    }

    public class DashboardVersionRepository
    {
        public const string DashboardVersionCsvFileName = "dashboard_version.csv";
        public const string Headers = "entry;hash;author_name;author_email;author_date;committer_name;committer_email;committer_date;message";

        private readonly IFileSystem _fileSystem;

        public DashboardVersionRepository(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IEnumerable<DashboardVersion> GetDashboardVersionList()
        {
            var lines = _fileSystem.ReadLines(DashboardVersionCsvFileName).ToList();
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

                yield return new DashboardVersionBuilder()
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
    }
}