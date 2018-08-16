using System.Collections.Generic;

namespace Dash.Features.Dashboards
{
    public class DashboardList
    {
        public List<DashboardListItem> Items { get; set; }
        public int? TotalCount { get; set; }
    }
}