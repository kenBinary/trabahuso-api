using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using trabahuso_api.DTOs.Location;
using trabahuso_api.Helpers;


namespace trabahuso_api.Repository
{

    public class LocationRepository : ILocationRepository
    {
        private readonly IConfiguration _config;
        private readonly TursoDatabaseSettings? _dbSettings;
        private readonly HttpClient _httpClient;
        private readonly ISqliteQueryCompiler _sqliteCompiler;
        public LocationRepository(IConfiguration config, ISqliteQueryCompiler sqliteCompiler)
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

        public async Task<List<LocationCountDto>> GetLocationsCount(LocationCountFilters locationCountFilters)
        {
            var queryBuilder = new Query("job_data")
            .SelectRaw("CASE WHEN instr(location, ',') > 0 THEN substr(location, 1, instr(location, ',') - 1) ELSE location END AS province, COUNT(*)  as count")
            .Where("location", "!=", "unspecified")
            .GroupBy("province");


            queryBuilder.When(
                !locationCountFilters.RetrieveAll,
                queryBuilder => queryBuilder.Offset(locationCountFilters.PageNumber)
            );

            queryBuilder.When(
                 !locationCountFilters.RetrieveAll,
                 queryBuilder => queryBuilder.Limit(locationCountFilters.PageSize)
             );

            queryBuilder.When(
                 locationCountFilters.IsDescending,
                 queryBuilder => queryBuilder.OrderByDesc("count")
             );

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

            List<LocationCountDto>? data = tursoParser.ParseLocationCount(responseObject);


            if (data == null)
            {
                return [];
            }

            return data;
        }
    }
}