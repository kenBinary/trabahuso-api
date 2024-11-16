using System;
using System.Collections.Generic;
using System.Linq;
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
        public JobRepository(IConfiguration config)
        {
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

        public Job GetJob(string jobDataId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Job>> GetAllAsync(QueryObject queryParams)
        {
            List<Job> jobs = [];

            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "",
                new RequestData(
                    [
                        new Request(
                            "execute",
                            new Statement(
                                "select * from job_data where salary > 10 limit 10"
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
            }

            string stringJson = await response.Content.ReadAsStringAsync();
            // Console.WriteLine(stringJson);
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