namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfiguration
    {
        public DashboardConfiguration(string id, string name, string team, string[] environments)
        {
            Id = id;
            Name = name;
            Team = team;
            Environments = environments;
        }

        public string Id { get; }
        public string Name { get; }
        public string Team { get; }
        public string[] Environments { get; }
    }

    public class DashboardConfigurationBuilder
    {
        private string _id = "";
        private string _name = "";
        private string _team = "";
        private string[] _environments = new string[0];

        public DashboardConfigurationBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public DashboardConfigurationBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DashboardConfigurationBuilder WithTeam(string team)
        {
            _team = team;
            return this;
        }

        public DashboardConfigurationBuilder WithEnvironments(params string[] environments)
        {
            _environments = environments;
            return this;
        }

        public DashboardConfiguration Build()
        {
            return new DashboardConfiguration(_id, _name, _team, _environments);
        }

        public static implicit operator DashboardConfiguration(DashboardConfigurationBuilder builder)
        {
            return builder.Build();
        }
    }
}