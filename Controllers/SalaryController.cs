using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using trabahuso_api.Interfaces;
using trabahuso_api.util;

namespace trabahuso_api.Controllers
{
    [ApiController]
    [Route("api/salary")]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryRepository _salaryRepository;
        public SalaryController(ISalaryRepository salaryRepository)
        {
            _salaryRepository = salaryRepository;
        }

        [HttpGet("frequency-distribution")]
        public async Task<ActionResult<string>> GetFrequencyDistribution()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Distribution> salaryDistribution = await _salaryRepository.GetSalaryFrequencyDistributions();

            return Ok(salaryDistribution);
        }
    }
}