using System.Text.Json.Serialization;

namespace yyMediaTime
{
    public class MediaFileModel
    {
        /// <summary>
        /// Assumes file.DateTime is not null.
        /// </summary>
        public static string GetNewFileName (MediaFileModel file, int minutesToAdd) =>
            $"{file.DateTime!.Value.AddMinutes (minutesToAdd).ToString ("yyyyMMdd'-'HHmmss")} ({file.OriginalFileNameWithoutExtension}){file.Extension}";

        [JsonPropertyName ("file_path")]
        public string? FilePath { get; set; }

        [JsonPropertyName ("local_timestamp")]
        public DateTime? LocalTimestamp { get; set; }

        [JsonPropertyName ("original_file_name_without_extension")]
        public string? OriginalFileNameWithoutExtension { get; set; }

        [JsonPropertyName ("extension")]
        public string? Extension { get; set; }

        [JsonIgnore]
        public IReadOnlyList <MetadataExtractor.Directory>? Metadata { get; set; }

        // Directory.Name, Directory.Tags and Tag.Name are not nullable.
        // Tag.Description is nullable.
        // Is Tag.HasName is false, there shouldnt be much reason to include the tag in the JSON.
        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Directory.cs
        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Tag.cs

        public record DirectoryRecord (string directory_name, IEnumerable <TagRecord> tags);

        public record TagRecord (string name, string? description);

        /// <summary>
        /// Dont use me.
        /// </summary>
        [JsonPropertyName ("metadata")]
        public IEnumerable <DirectoryRecord>? MetadataForSerialization =>
            Metadata?.Select (x => new DirectoryRecord (x.Name, x.Tags.Where (y => y.HasName).Select (y => new TagRecord (y.Name, y.Description))));

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
