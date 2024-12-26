using System.Text.Json;
using System.Text.Json.Serialization;
using yyLib;

namespace mediaTime
{
    public class MetadataJsonConverter: JsonConverter <IReadOnlyList <MetadataExtractor.Directory>>
    {
        // Directory.Name, Directory.Tags and Tag.Name are not nullable.
        // Tag.Description is nullable.
        // If Tag.HasName is false, there shouldnt be much reason to include the tag in the JSON.
        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Directory.cs
        // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Tag.cs

        public override void Write (
            Utf8JsonWriter writer, IReadOnlyList <MetadataExtractor.Directory> metadata, JsonSerializerOptions options) // "options" is not used.
        {
            foreach (var directory in metadata)
            {
                writer.WriteStartObject ();
                writer.WriteString ("directory_name", directory.Name);
                writer.WriteStartArray ("tags");

                foreach (var tag in directory.Tags.Where (x => x.HasName))
                {
                    writer.WriteStartObject ();
                    writer.WriteString ("name", tag.Name);
                    writer.WriteString ("description", tag.Description);
                    writer.WriteEndObject ();
                }

                writer.WriteEndArray ();
                writer.WriteEndObject ();
            }
        }

        public override IReadOnlyList <MetadataExtractor.Directory> Read (
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new yyNotSupportedException ("Deserialization not supported.");
    }
}
