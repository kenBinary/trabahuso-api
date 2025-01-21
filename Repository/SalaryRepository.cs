using System.Text.Json;
using Microsoft.Extensions.Options;
using SqlKata;
using trabahuso_api.Helpers;
using trabahuso_api.Interfaces;
using trabahuso_api.Mapper;
using trabahuso_api.Models;
using trabahuso_api.util;

namespace trabahuso_api.Repository
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly TursoDatabaseSettings? _dbSettings;
        private readonly HttpClient _httpClient;
        private readonly ISqliteQueryCompiler _sqliteCompiler;
        private readonly ILogger<SalaryRepository> _logger;
        public SalaryRepository(
            ISqliteQueryCompiler sqliteCompiler, IOptions<TursoDatabaseSettings> options,
            ILogger<SalaryRepository> logger)
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
        public async Task<List<Distribution>> GetSalaryFrequencyDistributions()
        {
            var queryBuilder = new Query("job_data")
                        .WhereNotNull("salary")
                        .OrderByDesc("salary");

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

            TursoResponseParser tursoParser = new TursoResponseParser();

            List<Job>? data = tursoParser.ParseJobData(responseObject);

            if (data == null)
            {
                return [];
            }

            List<double> salaries = [];
            for (int i = 0; i < data.Count; i++)
            {
                double? salary = data[i].Salary;

                if (salary != null)
                {
                    salaries.Add((double)salary);
                }
            }

            FrequencyDistribution frequencyDistribution = new FrequencyDistribution();
            List<Distribution> distribution = frequencyDistribution.GetFrequencyDistribution(salaries);

            //TODO: optimize this lol 
            foreach (var range in distribution)
            {
                double lowerRange = double.Parse(range.Range.Split("-")[0]);
                double upperRange = double.Parse(range.Range.Split("-")[1]);

                foreach (var salary in salaries)
                {
                    if (salary >= lowerRange && salary <= upperRange)
                    {
                        range.Count += 1;
                    }
                }
            }

            return distribution;
        }
    }
}