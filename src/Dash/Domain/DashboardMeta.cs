namespace Dash.Domain
{
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
}