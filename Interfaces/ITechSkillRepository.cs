using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Models;

namespace trabahuso_api.Interfaces
{
    public interface ITechSkillRepository
    {
        // we can only send a POST request
        // in request we only need
        // -DB url
        // -auth token
        // returns a JSON object

        Task<TechSkill?> GetByIdAsync(string techSkillId);
        Task<List<TechSkill>> GetAllAsync(TechSkillFilters techSkillFilters);
    }
}