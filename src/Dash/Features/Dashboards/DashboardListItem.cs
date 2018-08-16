using System;

namespace Dash.Features.Dashboards
{
    public class DashboardListItem
    {
        public string Id { get; set; }
        public string Team { get; set; }
        public string Name { get; set; }
        public DateTime? LastModified { get; set; }
    }
}