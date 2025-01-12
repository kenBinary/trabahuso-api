using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Salary;
using trabahuso_api.util;

namespace trabahuso_api.Interfaces
{
    public interface ISalaryRepository
    {
        Task<List<Distribution>> GetSalaryFrequencyDistributions();
    }
}