using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Location;
using trabahuso_api.DTOs.Querries;

namespace trabahuso_api.Interfaces
{
    public interface ILocationRepository
    {
        Task<List<LocationCountDto>> GetLocationsCount(LocationCountFilters locationCountFilters);
        Task<List<LocationMedianSalaryDto>> GetLocationMedianSalary(LocationMedianSalaryFilters locationMedianSalaryFilters);
    }
}