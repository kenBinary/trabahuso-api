using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.Models;

namespace trabahuso_api.DTOs.Job
{
    public record JobDto(
        string? JobDataId,
        string? JobTitle,
        string? Location,
        double? Salary,
        string? JobLevel
    );
}