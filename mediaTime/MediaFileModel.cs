using System.Text.Json.Serialization;

namespace mediaTime
{
    public class MediaFileModel
    {
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

        [JsonPropertyName ("metadata")]
        public IEnumerable <(string directory_name, IEnumerable <(string tag_name, string? description)> tags)>? MetadataForSerialization =>
            Metadata?.Select (x => (x.Name, x.Tags.Select (y => (y.Name, y.Description))));

        [JsonPropertyName ("type")]
        public MediaFileType? Type { get; set; }

        [JsonPropertyName ("model")]
        public string? Model { get; set; }

        [JsonPropertyName ("date_time_source")]
        public DateTimeSource? DateTimeSource { get; set; }

        [JsonPropertyName ("date_time")]
        public DateTime? DateTime { get; set; }
    }
}
