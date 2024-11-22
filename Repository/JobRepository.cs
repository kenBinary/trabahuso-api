using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Compilers;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Interfaces;
using trabahuso_api.Mapper;
using trabahuso_api.Models;

namespace trabahuso_api.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly IConfiguration _config;
        private readonly TursoDatabaseSettings? _dbSettings;
        private readonly HttpClient _httpClient;
        private readonly ISqliteQueryCompiler _sqliteCompiler;
        public JobRepository(IConfiguration config, ISqliteQueryCompiler sqliteCompiler)
        {
            _sqliteCompiler = sqliteCompiler;
            _config = config;
            _dbSettings = _config.GetSection("TursoDatabase").Get<TursoDatabaseSettings>();
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_dbSettings?.Url ?? ""),
                DefaultRequestHeaders = {
                    {"Authorization", $"Bearer {_dbSettings?.Token}" ?? ""},
                }
            };
        }

        // TODO: Write better error handling
        public async Task<Job?> GetByIdAsync(string jobDataId)
        {

            var queryBuilder = new Query("job_data");
            queryBuilder.Where("job_data_id", jobDataId);

            var compiledQuery = _sqliteCompiler.Compile(queryBuilder);

            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "",
                new RequestData(
                    [
                        new Request(
                            "execute",
                            new Statement(
                                compiledQuery.RawSql,
                                compiledQuery.Bindings.ToSqlArguments()
                            )
                        )
                    ]
                )
            );


            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string stringJson = await response.Content.ReadAsStringAsync();
            TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

            if (responseObject == null)
            {
                Console.WriteLine("Failed to Deserialize");
                return null;
            }

            List<ResultRow>? row = responseObject.GetFirstResult()?.Response?.Result?.GetFirstRow();

            if (row == null)
            {
                return null;
            }

            return row.ToJob();
        }

        public async Task<List<Job>> GetAllAsync(JobFilters jobFilters)
        {
            List<Job> jobs = [];

            var queryBuilder = new Query("job_data");

            queryBuilder.When(
                !jobFilters.RetrieveAll,
                queryBuilder => queryBuilder.Offset(jobFilters.PageNumber)
            );

            queryBuilder.When(
                 !jobFilters.RetrieveAll,
                 queryBuilder => queryBuilder.Limit(jobFilters.PageSize)
             );

            queryBuilder.When(
               jobFilters.SortBy != null && !jobFilters.IsDescending,
               queryBuilder => queryBuilder.OrderBy(jobFilters.SortBy)
            );

            queryBuilder.When(
               jobFilters.SortBy != null && jobFilters.IsDescending,
               queryBuilder => queryBuilder.OrderByDesc(jobFilters.SortBy)
            );

            // TODO: implement filtering of fields
            // if (queryParams.Includes.Count() > 0)
            // {
            //     foreach (var field in queryParams.Includes)
            //     {
            //         queryBuilder.Select(field);
            //     }
            // }

            var compiledQuery = _sqliteCompiler.Compile(queryBuilder);

            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "",
                new RequestData(
                    [
                        new Request(
                            "execute",
                            new Statement(
                                compiledQuery.RawSql,
                                compiledQuery.Bindings.ToSqlArguments()
                            )
                        )
                    ]
                )
            );

            if (response == null)
            {
                return [];
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return [];
            }

            string stringJson = await response.Content.ReadAsStringAsync();
            TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

            if (responseObject == null)
            {
                Console.WriteLine("Failed to Deserialize");
                return [];
            }

            List<List<ResultRow>>? rows = responseObject.GetFirstResult()?.Response?.Result?.TableRows;

            if (rows == null)
            {
                return [];
            }

            jobs.AddRange(rows.ToJobs());

            return jobs;
        }


    }
}