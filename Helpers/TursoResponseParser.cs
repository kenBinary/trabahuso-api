using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Location;
using trabahuso_api.Models;

namespace trabahuso_api.Helpers
{
    public class TursoResponseParser
    {
        public List<Job>? ParseJobData(TursoResponse tursoResponse)
        {
            List<Job> data = [];

            Result? result = tursoResponse.GetFirstResult();

            if (result == null)
            {
                return null;
            }

            if (result.Response == null)
            {
                return null;
            }

            Response response = result.Response;

            ExecuteResult executeResult = response.Result;

            List<List<Row>> rows = executeResult.Rows;

            foreach (var resultRow in rows)
            {
                bool isInt = int.TryParse(resultRow[3].Value, out int Salary);

                data.Add(
                    new Job()
                    {
                        JobDataId = resultRow[0].Value,
                        JobTitle = resultRow[1].Value,
                        Location = resultRow[2].Value,
                        Salary = isInt ? Salary : null,
                        JobLevel = resultRow[4].Value,
                        DateScraped = resultRow[5].Value
                    }
                );
            }

            return data;
        }

        public List<LocationCountDto>? ParseLocationCount(TursoResponse tursoResponse)
        {
            List<LocationCountDto> data = [];

            Result? result = tursoResponse.GetFirstResult();

            if (result == null)
            {
                return null;
            }

            if (result.Response == null)
            {
                return null;
            }

            Response response = result.Response;

            ExecuteResult executeResult = response.Result;

            List<List<Row>> rows = executeResult.Rows;

            foreach (var resultRow in rows)
            {
                bool isInt = int.TryParse(resultRow[1].Value, out int count);
                string? province = resultRow[0].Value;

                if (isInt && province != null)
                {
                    data.Add(
                       new LocationCountDto(province, count)
                   );
                }
            }

            return data;
        }

        public List<LocationMedianSalaryDto>? ParseLocationMedianSalary(TursoResponse tursoResponse)
        {
            List<LocationMedianSalaryDto> data = [];

            Result? result = tursoResponse.GetFirstResult();

            if (result == null)
            {
                return null;
            }

            if (result.Response == null)
            {
                return null;
            }

            Response response = result.Response;

            ExecuteResult executeResult = response.Result;

            List<List<Row>> rows = executeResult.Rows;

            foreach (var row in rows)
            {
                bool isInt = int.TryParse(row[1].Value, out int salary);
                string? province = row[0].Value;

                if (isInt && province != null)
                {
                    data.Add(
                       new LocationMedianSalaryDto(province, salary)
                   );
                }
            }

            return data;
        }
        public List<List<Row>>? ParseResultRows(TursoResponse tursoResponse)
        {
            Result? result = tursoResponse.GetFirstResult();

            if (result == null)
            {
                return null;
            }

            if (result.Response == null)
            {
                return null;
            }

            Response response = result.Response;

            ExecuteResult executeResult = response.Result;

            List<List<Row>> rows = executeResult.Rows;

            return rows;
        }
    }
}