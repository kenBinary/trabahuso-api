using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.DTOs.TechSkill;
using trabahuso_api.Interfaces;
using trabahuso_api.Mapper;
using trabahuso_api.Models;

namespace trabahuso_api.Controllers
{
    [ApiController]
    [Route("api/TechSkill")]
    public class TechStackController : ControllerBase
    {

        private readonly ITechSkillRepository _techSkillRepository;

        public TechStackController(ITechSkillRepository techSkillRepository)
        {
            _techSkillRepository = techSkillRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTechSkills([FromQuery] TechSkillFilters techSkillFilters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<TechSkill> techSkills = await _techSkillRepository.GetAllAsync(techSkillFilters);

            List<TechSkillDto> techSkillDtos = techSkills.Select(TechSkill => TechSkill.ToTechSkillDto()).ToList();

            return Ok(techSkillDtos);
        }

        [HttpGet("{tech-skill-id}")]
        public async Task<IActionResult> GetTechSkill([FromRoute(Name = "tech-skill-id")] string techSkillId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TechSkill? techSkill = await _techSkillRepository.GetByIdAsync(techSkillId);

            if (techSkill == null)
            {
                return NotFound("Tech Skill with specified ID not found");
            }

            return Ok(techSkill.ToTechSkillDto());
        }

    }
}