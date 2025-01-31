using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.Job;
using trabahuso_api.Models;

namespace trabahuso_api.Mapper
{
    public static class JobMapper
    {
        public static List<Job> ToJobs(this List<List<Row>> resultRows)
        {
            List<Job> jobs = [];
            foreach (var resultRow in resultRows)
            {
                bool isInt = int.TryParse(resultRow[3].Value, out int Salary);

                jobs.Add(
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

            return jobs;
        }

        public static Job ToJob(this List<Row> resultRow)
        {

            bool isInt = int.TryParse(resultRow[3].Value, out int Salary);

            return new Job()
            {
                JobDataId = resultRow[0].Value,
                JobTitle = resultRow[1].Value,
                Location = resultRow[2].Value,
                Salary = isInt ? Salary : null,
                JobLevel = resultRow[4].Value,
                DateScraped = resultRow[5].Value
            };
        }

        public static JobDto ToJobDto(this Job job)
        {
            return new JobDto(
                job.JobDataId,
                job.JobTitle,
                job.Location,
                job.Salary,
                job.JobLevel,
                job.DateScraped
            );
        }

    }
}