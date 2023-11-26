using System.Text.Json.Serialization;

namespace mediaTime
{
    public class ResultModel
    {
        [JsonPropertyName ("file_path")]
        public string? FilePath { get; set; }

        [JsonPropertyName ("new_file_path")]
        public string? NewFilePath { get; set; }

        [JsonPropertyName ("type")]
        public ResultType? Type { get; set; }
    }
}
