using System.Collections.Generic;
using System.Linq;

namespace Dash.Infrastructure.Configuration
{
    public class Stuff
    {
        private readonly string[] _environments;

        public Stuff(string[] environments, List<DashboardConfiguration> configurations)
        {
            _environments = environments;
            Configurations = configurations;
        }

        public List<DashboardConfiguration> Configurations { get; }

        public void AddOrUpdate(DashboardConfiguration dashboardConfiguration)
        {
            var i = Configurations.FindIndex(x => x.Id == dashboardConfiguration.Id);
            var newDashboardConfig = i == -1;

            if (!newDashboardConfig)
            {
                Configurations[i] = dashboardConfiguration;
            }
            else
            {
                Configurations.Add(dashboardConfiguration);
            }
        }

        public bool Remove(string id)
        {
            var index = Configurations.FindIndex(x => x.Id == id);
            if (index == -1)
            {
                return false;
            }

            Configurations.RemoveAt(index);
            return true;
        }

        public IEnumerable<string> GetEnvironments()
        {
            var actualEnvironments = new HashSet<string>(Configurations.SelectMany(x => x.Environments).Select(x => x.Name));
            if (actualEnvironments.Count == 0)
            {
                yield break;
            }

            foreach (var environment in _environments)
            {
                if (actualEnvironments.Contains(environment))
                {
                    yield return environment;
                }
            }

            actualEnvironments.ExceptWith(_environments);

            foreach (var environment in actualEnvironments.OrderBy(x => x))
            {
                yield return environment;
            }
        }
    }
}