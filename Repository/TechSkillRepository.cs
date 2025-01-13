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
        private static class NormalizationData
        {
            public static readonly string[] ProgrammingLanguages = ["c", "c#", "c++", "clojure", "cobol",
            "css", "css3", "dart", "elixir", "erlang", "f#", "fortran", "gdscript", "go", "groovy", "haskell",
            "html", "html5", "java", "javascript", "js", "kotlin", "lua", "matlab", "objective-c", "perl", "php",
            "php 7", "python", "python 3", "ruby", "rust", "scala", "solidity", "sql", "swift", "typescript", "zig"];
            public static readonly string[] CloudPlatforms = ["alibaba cloud", "amazon web services", "aws", "azure",
            "cisco cloud", "digitalocean", "firebase", "gcp", "google cloud", "google cloud platform", "heroku",
            "ibm cloud", "linode", "microsoft azure", "oracle cloud", "oracle cloud infrastructure", "sap cloud platform"];
            public static readonly string[] Databases = ["cassandra", "dynamodb", "eloquent orm", "mariadb",
            "microsoft sql server", "mongodb", "mongoose", "ms sql server", "mysql", "nosql", "peewee", "postgresql",
            "redis", "sqlite"];
            public static readonly string[] FrameworksAndLibraries = [".net", ".net core", "angular", "asp.net",
            "asp.net core", "backbone", "bootstrap", "bulma", "cakephp", "django", "django orm", "ember", "ember.js",
            "emberjs", "express", "express.js", "expressjs", "fastapi", "fastify", "flask", "flutter", "gatsby",
            "gorm", "graphql", "hibernate", "java ee", "java se", "java server faces", "javafx", "jquery",
            "knex.js", "laravel", "laravel eloquent", "lodash", "material-ui", "materialize", "matplotlib",
            "nestjs", "next.js", "nextjs", "node.js", "nodejs", "numpy", "nuxt", "nuxt.js", "pandas", "phoenix",
            "prisma", "pygame", "pytorch", "rails active record", "react", "react native", "react.js", "reactjs",
            "redux", "ruby on rails", "rxjs", "semantic ui", "sequelize", "socket.io", "spring", "spring boot",
            "spring data", "spring data jpa", "sqlalchemy", "svelte", "symfony", "tailwind", "tailwind css",
            "tailwindcss", "tensorflow", "typeorm", "unity", "vue", "vue.js", "vuejs", "webpack", "xamarin"];
            public static readonly string[] Tools = ["android studio", "assembly", "bash", "docker", "git",
            "github", "gitlab", "gradle", "grunt", "gulp", "jenkins", "jira", "kubernetes", "linux", "postman",
            "powershell", "preact"];
        }
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

            queryBuilder.When(
                techSkillFilters.Category != null,
                queryBuilder =>
                {
                    switch (techSkillFilters.Category)
                    {
                        case "programming_languages":
                            return queryBuilder.WhereIn("tech_type", NormalizationData.ProgrammingLanguages);
                        case "databases":
                            return queryBuilder.WhereIn("tech_type", NormalizationData.Databases);
                        case "frameworks_and_libraries":
                            return queryBuilder.WhereIn("tech_type", NormalizationData.FrameworksAndLibraries);
                        case "cloud_platforms":
                            return queryBuilder.WhereIn("tech_type", NormalizationData.CloudPlatforms);
                        case "tools":
                            return queryBuilder.WhereIn("tech_type", NormalizationData.Tools);
                        default:
                            return queryBuilder;
                    }
                }
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