using System;

namespace Dash.Domain
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
}