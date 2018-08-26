using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class DashboardConfiguration
    {
        public DashboardConfiguration(string id, string name, string team, Environment[] environments)
        {
            Id = id;
            Name = name;
            Team = team;
            Environments = environments;
        }

        public string Id { get; }
        public string Name { get; }
        public string Team { get; }
        public Environment[] Environments { get; }
    }

    public class DashboardConfigurationBuilder
    {
        private string _id = "";
        private string _name = "";
        private string _team = "";
        private Environment[] _environments = new Environment[0];

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

        public DashboardConfigurationBuilder WithEnvironments(params Environment[] environments)
        {
            _environments = environments;
            return this;
        }

        public DashboardConfigurationBuilder WithEnvironments(params EnvironmentBuilder[] builders)
        {
            _environments = builders.Select(x => x.Build()).ToArray();
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

    public enum EnvironmentState
    {
        Enabled,
        Disabled
    }

    public sealed class Environment
    {
        public Environment(string name, EnvironmentState enabled)
        {
            Name = name;
            Enabled = enabled;
        }

        public string Name { get; }
        public EnvironmentState Enabled { get; }

        private bool Equals(Environment other)
        {
            return string.Equals(Name, other.Name) && Enabled == other.Enabled;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Environment other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Enabled.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{{{Name} is {Enabled:G}}}";
        }
    }

    public class EnvironmentBuilder
    {
        private string _name = string.Empty;
        private EnvironmentState _enabled = EnvironmentState.Disabled;

        public EnvironmentBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public EnvironmentBuilder WithState(EnvironmentState enabled)
        {
            _enabled = enabled;
            return this;
        }

        public Environment Build()
        {
            return new Environment(_name, _enabled);
        }

        public static implicit operator Environment(EnvironmentBuilder builder)
        {
            return builder.Build();
        }
    }
}