using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public class Job
    {

        [JsonPropertyName("job_data_id")]
        public required string JobDataId { get; set; }
        [JsonPropertyName("job_title")]
        public required string JobTitle { get; set; }
        [JsonPropertyName("location")]
        public required string Location { get; set; }
        [JsonPropertyName("salary")]
        public int? Salary { get; set; }
        [JsonPropertyName("job_level")]
        public string? JobLevel { get; set; }
        [JsonPropertyName("date_scraped")]
        public required string DateScraped { get; set; }
        public List<TechSkill> TechSkills { get; set; } = [];

    }
}