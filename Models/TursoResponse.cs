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
        public List<TursoResult> Results { get; set; } = [];

        public TursoResult? GetFirstResult()
        {
            if (Results.Count > 0)
            {
                return Results[0];
            }

            return null;
        }
    }

    public class TursoResult
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("response")]
        public required TursoResultResponse Response { get; set; }
    }

    public class TursoResultResponse
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }
        [JsonPropertyName("result")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TursoResponseResult? Result { get; set; }
    }

    public class TursoResponseResult
    {
        [JsonPropertyName("cols")]
        public List<ResultColumns> TableColumns { get; set; } = [];
        [JsonPropertyName("rows")]
        public List<List<ResultRow>> TableRows { get; set; } = [];
        [JsonPropertyName("affected_row_count")]
        public int AffectedRows { get; set; }
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
    }

    public record ResultColumns(
        [property: JsonPropertyName("name")] string FieldName,
        [property: JsonPropertyName("decltype")] string DataType
    );

    public record ResultRow(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("value")]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? Value
    );
}