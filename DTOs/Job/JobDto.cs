using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.Models;

namespace trabahuso_api.DTOs.Job
{
    public record JobDto(
        string? JobTitle,
        string? Location,
        int? Salary,
        string? JobLevel
    );
}