using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Interfaces;

namespace trabahuso_api.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] QueryObject queryParams)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobs = await _jobRepository.GetAllAsync(queryParams);

            return Ok(jobs);
        }
    }
}