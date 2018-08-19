using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Dash.Domain;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Dash.Features.Dashboards
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
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
        public virtual ActionResult<DashboardList> GetDashboards()
        {
            var dashboards = _dashboardService.GetDashboards().ToList();

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
        [Route("/api/dashboards/{id}")]
        [SwaggerOperation("GetDashboardById")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DashboardDetails))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void))]
        public virtual IActionResult DashboardsIdGet([FromRoute] [Required] string id)
        {
            var dashboard = _dashboardService.GetDashboards().SingleOrDefault(x => x.Id == id);
            if (dashboard == null)
            {
                return NotFound();
            }

            return Ok(dashboard);
        }
    }
}