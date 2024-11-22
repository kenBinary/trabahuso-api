using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using trabahuso_api.DTOs.Job;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Interfaces;
using trabahuso_api.Mapper;

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

        [HttpGet("{job-id}")]
        public async Task<ActionResult<string>> GetJob([FromRoute(Name = "job-id")] string jobId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                return NotFound("Job Not Found");
            }

            return Ok(job.ToJobDto());
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] JobFilters jobFilters)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobs = await _jobRepository.GetAllAsync(jobFilters);

            var jobsDto = jobs.Select(job => job.ToJobDto()).ToList();

            return Ok(jobsDto);
        }
    }
}