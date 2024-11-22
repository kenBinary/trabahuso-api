using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public class TechSkill
    {
        public required string TechStackId { get; set; }
        public required string JobDataId { get; set; }
        public required string TechType { get; set; }
    }
}