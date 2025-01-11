using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using trabahuso_api.CustomAttributes;

namespace trabahuso_api.DTOs.Querries
{
    public record QueryFilters
    {
        public int PageNumber { get; set; } = 0;
        [Range(0, 25, ErrorMessage = "Page size exceeded, it must be between 0 and 25")]
        public int PageSize { get; set; } = 10;
        public bool RetrieveAll { get; set; } = false;

        // TODO: implement filtering of fields
        // public string[] Includes { get; set; } = [];
    }

    public record TechSkillFilters : QueryFilters
    {

    }

    public record JobFilters : QueryFilters
    {
        [AllowedValuesCustom(["job_data_id", "job_title", "location", "salary", "job_level", "date_scraped"])]
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }

}