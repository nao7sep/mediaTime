using System.Text.Json.Serialization;

namespace yyMediaTime
{
    public class MediaFileInfo
    {
        /// <summary>
        /// Assumes file.DateTime is not null.
        /// </summary>
        public static string GetNewFileName (MediaFileInfo file, int minutesToAdd) =>
            $"{file.DateTime!.Value.AddMinutes (minutesToAdd).ToString ("yyyyMMdd'-'HHmmss")} ({file.OriginalFileNameWithoutExtension}){file.Extension}";

        [JsonPropertyName ("file_path")]
        public string? FilePath { get; set; }

        [JsonPropertyName ("local_timestamp")]
        public DateTime? LocalTimestamp { get; set; }

        [JsonPropertyName ("original_file_name_without_extension")]
        public string? OriginalFileNameWithoutExtension { get; set; }

        [JsonPropertyName ("extension")]
        public string? Extension { get; set; }

        [JsonPropertyName ("metadata")]
        [JsonConverter (typeof (MetadataJsonConverter))]
        public IReadOnlyList <MetadataExtractor.Directory>? Metadata { get; set; }

        [JsonPropertyName ("type")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public MediaFileType? Type { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("date_time_source")]
        [JsonConverter (typeof (JsonStringEnumConverter))]
        public DateTimeSource? DateTimeSource { get; set; }

        /// <summary>
        /// Returns a local time if it's an image file.
        /// If it's a video file, it's assumed to be in UTC due to the specifications,
        /// but some cameras incorrectly set video files' date/time in local time.
        /// </summary>
        [JsonPropertyName ("date_time")]
        public DateTime? DateTime { get; set; }
    }
}
