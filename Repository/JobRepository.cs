using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        private readonly HttpClient _httpClient;
        private readonly ISqliteQueryCompiler _sqliteCompiler;
        private readonly TursoDatabaseSettings? _dbSettings;
        private readonly ILogger<JobRepository> _logger;
        public JobRepository(
            ISqliteQueryCompiler sqliteCompiler, IOptions<TursoDatabaseSettings> options,
            ILogger<JobRepository> logger)
        {
            _logger = logger;
            _dbSettings = options.Value ?? new TursoDatabaseSettings();
            _sqliteCompiler = sqliteCompiler;
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
                _logger.LogError("failed to exectue HTTP request to turso");
                throw new Exception("failed to exectue HTTP request to turso");
            }

            string stringJson = await response.Content.ReadAsStringAsync();
            TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

            if (responseObject == null)
            {
                _logger.LogError("failed to deserialize");
                throw new Exception("failed to deserialize");
            }

            List<Row>? row = responseObject.GetFirstResult()?.Response?.Result?.GetFirstRow();

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

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("failed to exectue HTTP request to turso");
                throw new Exception("failed to exectue HTTP request to turso");
            }


            string stringJson = await response.Content.ReadAsStringAsync();
            TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

            if (responseObject == null)
            {
                _logger.LogError("failed to deserialize");
                throw new Exception("failed to deserialize");
            }

            List<List<Row>>? rows = responseObject.GetFirstResult()?.Response?.Result?.Rows;

            if (rows == null)
            {
                return [];
            }

            jobs.AddRange(rows.ToJobs());

            return jobs;
        }


    }
}