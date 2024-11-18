using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace trabahuso_api.Models
{
    public record RequestData(
         [property: JsonPropertyName("requests")] List<Request> Requests
     );

    public record Request(
        [property: JsonPropertyName("type")] string Type,
        [property:JsonPropertyName("stmt")]
            [property:JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            Statement? Statement = null
    )
    ;
    public record Statement(
        [property: JsonPropertyName("sql")] string SqlQuery,
        [property: JsonPropertyName("args")]
            [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            List<SqlArgument>? Arguments = null
    );

    public record SqlArgument(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("value")] string Value
    );
}