using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Dash.Features.Dashboards
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
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
            return new DashboardList
            {
                Items = new List<DashboardListItem>
                {
                    new DashboardListItem
                    {
                        Id = "DC668DB0-EDBA-4716-AB78-49F972B39261",
                        Name = "dashboard.json",
                        Team = "DED",
                        LastModified = DateTime.Now
                    }
                },
                TotalCount = 1
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
            return Ok(new DashboardListItem
            {
                Id = "DC668DB0-EDBA-4716-AB78-49F972B39261",
                Name = "dashboard.json",
                Team = "DED",
                LastModified = DateTime.Now
            });
        }
    }
}