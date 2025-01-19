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
        public SalaryRepository(ISqliteQueryCompiler sqliteCompiler, IOptions<TursoDatabaseSettings> options)
        {
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