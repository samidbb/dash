using System;

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
}