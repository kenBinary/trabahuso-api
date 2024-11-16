using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public class Job
    {
        public string? JobDataId { get; set; }
        public string? JobTitle { get; set; }
        public string? Location { get; set; }
        public double? Salary { get; set; }
        public string? JobLevel { get; set; }
        public string? DateScraped { get; set; }
        public List<TechSkill> TechSkills { get; set; } = [];
    }
}