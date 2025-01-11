using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using trabahuso_api.DTOs.Location;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Interfaces;

namespace trabahuso_api.Controllers
{
    [ApiController]
    [Route("api/job-locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetJobByCount([FromQuery] LocationCountFilters locationCountFilters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationCount = await _locationRepository.GetLocationsCount(locationCountFilters);

            return Ok(locationCount);
        }

        [HttpGet("median")]
        public async Task<IActionResult> GetLocationMedianSalary([FromQuery] LocationMedianSalaryFilters locationMedianSalaryFilters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<LocationMedianSalaryDto> data = await _locationRepository.GetLocationMedianSalary(locationMedianSalaryFilters);

            return Ok(data);
        }

    }
}