using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Dash.Domain;
using Dash.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Dash.Features.Dashboards
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private const string GetByIdRouteName = "GetDashboardById";

        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns all available dashboards
        /// </summary>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/api/dashboards")]
        [SwaggerOperation("GetDashboards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DashboardList))]
        public ActionResult<DashboardList> GetDashboards()
        {
            var dashboards = _dashboardService.GetAll().ToList();

            return new DashboardList
            {
                Items = dashboards.Select(x => new DashboardListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Team = x.Team,
                    LastModified = x.LastModified
                }).ToList(),
                TotalCount = dashboards.Count
            };
        }

        /// <summary>
        /// Returns a single dashboard
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">OK</response>
        [HttpGet]
        [Route("/api/dashboards/{id}", Name = GetByIdRouteName)]
        [SwaggerOperation(GetByIdRouteName)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DashboardDetails))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
        public ActionResult<DashboardDetails> DashboardsIdGet([FromRoute] [Required] string id)
        {
            var dashboard = _dashboardService.GetById(id);
            if (dashboard == null)
            {
                return NotFound();
            }

            var details = new DashboardDetails
            {
                Id = dashboard.Id,
                Name = dashboard.Name,
                Team = dashboard.Team,
                LastModified = dashboard.LastModified,
                Content = dashboard.Content.ToString()
            };

            return details;
        }

        [HttpPost]
        [Route("/api/dashboards")]
        [SwaggerOperation("CreateDashboard")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(DashboardListItem))]
        public IActionResult Post([FromBody] DashboardInput input)
        {
            var id = IdGenerator.Generate(input.Name + input.Team);

            var dashboard = new DashboardBuilder()
                .WithId(id)
                .WithName(input.Name)
                .WithTeam(input.Team)
                .WithContent(input.Content)
                .Build();

            _dashboardService.Save(dashboard);

            var details = new DashboardDetails
            {
                Id = dashboard.Id,
                Name = dashboard.Name,
                Team = dashboard.Team,
                LastModified = dashboard.LastModified,
                Content = dashboard.Content.ToString()
            };

            return CreatedAtRoute(GetByIdRouteName, new {id = dashboard.Id}, details);
        }

        [HttpPut]
        [Route("/api/dashboards/{id}")]
        [SwaggerOperation("CreateOrUpdateDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DashboardListItem))]
        [SwaggerResponse(HttpStatusCode.Created, typeof(DashboardListItem))]
        public IActionResult Put([FromRoute] [Required] string id, [FromBody] DashboardInput input)
        {
            var dashboard = new DashboardBuilder()
                .WithId(id)
                .WithName(input.Name)
                .WithTeam(input.Team)
                .WithContent(input.Content)
                .Build();

            _dashboardService.Save(dashboard);

            var details = new DashboardDetails
            {
                Id = dashboard.Id,
                Name = dashboard.Name,
                Team = dashboard.Team,
                LastModified = dashboard.LastModified,
                Content = dashboard.Content.ToString()
            };

            return CreatedAtRoute(GetByIdRouteName, new {id = dashboard.Id}, details);
        }

        [HttpDelete]
        [Route("/api/dashboards/{id}")]
        [SwaggerOperation("CreateOrUpdateDashboard")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
        public IActionResult Delete([FromRoute] [Required] string id)
        {
            if (_dashboardService.DeleteById(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }

    public class DashboardInput
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public string Content { get; set; }
    }
}