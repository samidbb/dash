using System;
using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure
{
    public class Dashboard
    {
        private readonly DashboardMeta _meta;

        public Dashboard(string id, string name, string team, DashboardMeta meta, string content)
        {
            _meta = meta;
            Id = id;
            Team = team;
            Name = name;
            Content = content;
        }

        public string Id { get; }
        public string Team { get; }
        public string Name { get; }
        public string Content { get; }
        public DateTime LastModified => _meta.Author.Date;
    }

    public class DashboardBuilder
    {
        private string _id;

        public DashboardBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        private string _name;

        public DashboardBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        private string _team;

        public DashboardBuilder WithTeam(string team)
        {
            _team = team;
            return this;
        }

        private DashboardMeta _meta;

        public DashboardBuilder WithMeta(DashboardMeta meta)
        {
            _meta = meta;
            return this;
        }

        private string _content;

        public DashboardBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public Dashboard Build()
        {
            return new Dashboard(_id, _name, _team, _meta, _content);
        }

        public static implicit operator Dashboard(DashboardBuilder builder)
        {
            return builder.Build();
        }
    }

    public class DashboardMeta
    {
        public DashboardMeta(string commit, string commitMessage, Signature committer, Signature author)
        {
            Commit = commit;
            CommitMessage = commitMessage;
            Committer = committer;
            Author = author;
        }

        public string Commit { get; }
        public string CommitMessage { get; }
        public Signature Committer { get; }
        public Signature Author { get; }
    }

    public class DashboardMetaBuilder
    {
        private string _commit;

        public DashboardMetaBuilder WithCommit(string commit)
        {
            _commit = commit;
            return this;
        }

        private string _commitMessage;

        public DashboardMetaBuilder WithCommitMessage(string commitMessage)
        {
            _commitMessage = commitMessage;
            return this;
        }

        private Signature _committer;

        public DashboardMetaBuilder WithCommitter(Signature committer)
        {
            _committer = committer;
            return this;
        }

        private Signature _author;

        public DashboardMetaBuilder WithAuthor(Signature author)
        {
            _author = author;
            return this;
        }

        public DashboardMeta Build()
        {
            return new DashboardMeta(_commit, _commitMessage, _committer, _author);
        }

        public static implicit operator DashboardMeta(DashboardMetaBuilder builder)
        {
            return builder.Build();
        }
    }

    public class Signature
    {
        public Signature(string name, string email, DateTime date)
        {
            Name = name;
            Email = email;
            Date = date;
        }

        public string Name { get; }
        public string Email { get; }
        public DateTime Date { get; }
    }

    public class SignatureBuilder
    {
        private string _name;

        public SignatureBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        private string _email;

        public SignatureBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        private DateTime _date;

        public SignatureBuilder WithDate(DateTime date)
        {
            _date = date;
            return this;
        }

        public Signature Build()
        {
            return new Signature(_name, _email, _date);
        }

        public static implicit operator Signature(SignatureBuilder builder)
        {
            return builder.Build();
        }
    }

    public class DashboardService
    {
        private readonly DashboardVersionRepository _dashboardVersionRepository;
        private readonly DashboardSettingsRepository _dashboardSettingsRepository;
        private readonly IFileSystem _fileSystem;

        public DashboardService(DashboardVersionRepository dashboardVersionRepository, DashboardSettingsRepository dashboardSettingsRepository, IFileSystem fileSystem)
        {
            _dashboardVersionRepository = dashboardVersionRepository;
            _dashboardSettingsRepository = dashboardSettingsRepository;
            _fileSystem = fileSystem;
        }

        public IEnumerable<Dashboard> GetDashboards()
        {
            var versionList = _dashboardVersionRepository.GetDashboardVersionList().ToList();
            var settingsList = _dashboardSettingsRepository.GetAll().ToList();

            var map = settingsList.ToDictionary(x => x.Name);

            foreach (var version in versionList)
            {
                if (!map.TryGetValue(version.Entry, out var settings))
                {
                    continue;
                }

                Signature author = new SignatureBuilder().WithName(version.AuthorName).WithEmail(version.AuthorEmail).WithDate(version.AuthorDate);
                Signature committer = new SignatureBuilder().WithName(version.CommitterName).WithEmail(version.CommitterEmail).WithDate(version.CommitterDate);
                DashboardMeta meta = new DashboardMetaBuilder().WithCommit(version.Hash).WithCommitMessage(version.Message).WithCommitter(committer).WithAuthor(author);
                var content = _fileSystem.ReadAllText(version.Entry);

                yield return new DashboardBuilder()
                    .WithId(settings.Id)
                    .WithName(version.Entry)
                    .WithTeam(settings.Team)
                    .WithMeta(meta)
                    .WithContent(content)
                    .Build();
            }
        }
    }
}