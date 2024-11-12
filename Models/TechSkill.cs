using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public class TechSkill
    {
        [JsonPropertyName("tech_stack_id")]
        public required string TechStackId { get; set; }
        [JsonPropertyName("job_data_id")]
        public required string JobDataId { get; set; }
        [JsonPropertyName("tech_type")]
        public required string TechType { get; set; }
    }
}