using System.Text.Json.Serialization;

namespace yyMediaTime
{
    public class ResultInfo
    {
        [JsonPropertyName ("file_path")]
        public string? FilePath { get; set; }

        [JsonPropertyName ("added_minutes")]
        public int? AddedMinutes { get; set; }

        [JsonPropertyName ("new_file_path")]
        public string? NewFilePath { get; set; }

        [JsonPropertyName ("type")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public ResultType? Type { get; set; }

        [JsonPropertyName ("exception_string")]
        public string? ExceptionString { get; set; }
    }
}
