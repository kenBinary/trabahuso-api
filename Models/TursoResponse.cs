using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public class TursoResponse
    {
        [JsonPropertyName("baton")]
        public required string Baton { get; set; }
        [JsonPropertyName("base_url")]
        public string? BaseUrl { get; set; }
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; } = [];

        public Result? GetFirstResult()
        {
            if (Results.Count > 0)
            {
                return Results[0];
            }

            return null;
        }
    }

    public class Result
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("response")]
        public Response? Response { get; set; }

        [JsonPropertyName("error")]
        public Error? Error { get; set; }
    }

    public class Error
    {
        [JsonPropertyName("message")]
        public required string Message { get; set; }
        [JsonPropertyName("code")]
        public required string Code { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required ExecuteResult Result { get; set; }
    }

    public class ExecuteResult
    {
        [JsonPropertyName("cols")]
        public List<Column> Cols { get; set; } = [];
        [JsonPropertyName("rows")]
        public List<List<Row>> Rows { get; set; } = [];
        [JsonPropertyName("affected_row_count")]
        public int AffectedRowCount { get; set; }
        [JsonPropertyName("last_insert_rowid")]
        public string? LastInsertRowId { get; set; }
        [JsonPropertyName("replication_index")]
        public required string ReplicationIndex { get; set; }
        [JsonPropertyName("rows_read")]
        public int RowsRead { get; set; }
        [JsonPropertyName("rows_written")]
        public int RowsWritten { get; set; }
        [JsonPropertyName("query_duration_ms")]
        public double QueryDurationMs { get; set; }

        public List<Row>? GetFirstRow()
        {
            if (Rows.Count > 0)
            {
                return Rows[0];
            }

            return null;
        }
    }

    public record Column(
        [property: JsonPropertyName("name")] string FieldName,
        [property: JsonPropertyName("decltype")] string DataType
    );

    public record Row(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("value")]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? Value
    );
}