using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trabahuso_api.DTOs.Location
{
    public record LocationMedianSalaryDto(
        string Location,
        double Salary
    );
}