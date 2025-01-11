using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SqlKata;
using trabahuso_api.DTOs.Querries;
using trabahuso_api.Interfaces;
using trabahuso_api.Mapper;
using trabahuso_api.Models;

namespace trabahuso_api.Repository
{
    public class TechSkillRepository : ITechSkillRepository
    {
        private readonly IConfiguration _config;
        private readonly TursoDatabaseSettings? _dbSettings;
        private readonly HttpClient _httpClient;
        private readonly ISqliteQueryCompiler _sqliteCompiler;

        public TechSkillRepository(IConfiguration config, ISqliteQueryCompiler sqliteQueryCompiler)
        {
            _sqliteCompiler = sqliteQueryCompiler;
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

        // TODO: refactor tech_skill table to include categories
        // TODO: write better error handling
        public async Task<List<TechSkill>> GetAllAsync(TechSkillFilters techSkillFilters)
        {
            var queryBuilder = new Query("tech_skill");


            queryBuilder.When(
                !techSkillFilters.RetrieveAll,
                queryBuilder => queryBuilder.Offset(techSkillFilters.PageNumber)
            );
            queryBuilder.When(
                !techSkillFilters.RetrieveAll,
                queryBuilder => queryBuilder.Limit(techSkillFilters.PageSize)
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

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.RequestMessage);
                Console.WriteLine("response unsuccessful");
                return [];
            }

            string stringJson = await response.Content.ReadAsStringAsync();

            try
            {
                TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

                if (responseObject == null)
                {
                    return [];
                }

                List<List<Row>>? rows = responseObject.GetFirstResult()?.Response?.Result?.Rows;

                if (rows == null)
                {
                    return [];
                }

                return rows.ToTechSkills();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
                return [];
            }

        }

        public async Task<TechSkill?> GetByIdAsync(string techSkillId)
        {
            var queryBuilder = new Query("tech_skill").Where("tech_stack_id", techSkillId);

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
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.RequestMessage);
                Console.WriteLine("response unsuccessful");
                return null;
            }

            string stringJson = await response.Content.ReadAsStringAsync();

            try
            {
                TursoResponse? responseObject = JsonSerializer.Deserialize<TursoResponse>(stringJson);

                if (responseObject == null)
                {
                    return null;
                }

                List<Row>? row = responseObject.GetFirstResult()?.Response?.Result?.GetFirstRow();

                if (row == null)
                {
                    return null;
                }

                return row.ToTechSkill();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error.Message);
                return null;
            }


        }
    }
}