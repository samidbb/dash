using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dash.Domain
{
    public class Dashboard
    {
        private readonly DashboardMeta _meta;

        public Dashboard(string id, string name, string team, DashboardMeta meta, Content content)
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
        public Content Content { get; }
        public DateTime LastModified => _meta.Author.Date;
    }

    public class Content
    {
        private readonly JObject _embeddedDocument;

        private static Content Create(string json)
        {
            var jo = JObject.Parse(json);

            jo.Remove("id");

            return new Content(jo);
        }

        private Content(JObject embeddedDocument)
        {
            _embeddedDocument = embeddedDocument;
        }

        public override string ToString()
        {
            return _embeddedDocument.ToString(Formatting.None);
        }

        public static implicit operator Content(string json)
        {
            return Create(json);
        }

        public static implicit operator string(Content doc)
        {
            return doc.ToString();
        }
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

        private Content _content;

        public DashboardBuilder WithContent(Content content)
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