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
        // we can only send a POST request
        // in request we only need
        // -DB url
        // -auth token
        // returns a JSON object

        Job GetJob(string jobDataId);
        Task<List<Job>> GetAllAsync(QueryObject queryParams);
    }
}