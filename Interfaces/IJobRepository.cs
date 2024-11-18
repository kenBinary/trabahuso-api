using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Models;

namespace trabahuso_api.Interfaces
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(string jobDataId);
        Task<List<Job>> GetAllAsync(QueryObject queryParams);
    }
}