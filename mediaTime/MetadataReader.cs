using System.Globalization;
using System.Text.RegularExpressions;
using yyLib;

namespace mediaTime
{
    public static partial class MetadataReader
    {
        [GeneratedRegex (@"^(?<Timestamp>20[0-9]{6}-[0-9]{6}) \((?<OriginalFileNameWithoutExtension>.+?)\)$", RegexOptions.CultureInvariant)]
        private static partial Regex TimestampAndOriginalFileNameWithoutExtensionGeneratedRegex ();

        private static MediaFileType? ReadType (IReadOnlyList <MetadataExtractor.Directory>? metadata)
        {
            // todo
            throw new yyNotImplementedException (yyMessage.Create ("This method is not implemented yet."));
        }

        private static string? ReadModel (IReadOnlyList <MetadataExtractor.Directory>? metadata)
        {
            // todo
            throw new yyNotImplementedException (yyMessage.Create ("This method is not implemented yet."));
        }

        private static bool ReadDateTime (IReadOnlyList <MetadataExtractor.Directory>? metadata, out DateTimeSource dateTimeSource, out DateTime dateTime)
        {
            // todo
            throw new yyNotImplementedException (yyMessage.Create ("This method is not implemented yet."));
        }

        public static MediaFileModel Read (string filePath)
        {
            MediaFileModel xMediaFile = new ();

            // Assuming the path is proper.
            // If not, an exception will be thrown somewhere and the upper method will catch it.
            xMediaFile.FilePath = filePath;

            string xFileNameWithoutExtension = Path.GetFileNameWithoutExtension (filePath);

            Match xMatch = TimestampAndOriginalFileNameWithoutExtensionGeneratedRegex ().Match (xFileNameWithoutExtension);

            if (xMatch.Success)
            {
                xMediaFile.LocalTimestamp = DateTime.ParseExact (xMatch.Groups ["Timestamp"].Value, "yyyyMMdd'-'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                xMediaFile.OriginalFileNameWithoutExtension = xMatch.Groups ["OriginalFileNameWithoutExtension"].Value;
            }

            else xMediaFile.OriginalFileNameWithoutExtension = xFileNameWithoutExtension;

            xMediaFile.Extension = Path.GetExtension (filePath);

            try
            {
                // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/ImageMetadataReader.cs
                // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Formats/Jpeg/JpegMetadataReader.cs
                // https://github.com/drewnoakes/metadata-extractor-dotnet/blob/master/MetadataExtractor/Formats/QuickTime/QuickTimeMetadataReader.cs

                xMediaFile.Metadata = MetadataExtractor.ImageMetadataReader.ReadMetadata (filePath);
            }

            catch (Exception xException) when (xException is MetadataExtractor.ImageProcessingException or IOException)
            {
                // xMediaFile.Metadata remains null.
            }

            xMediaFile.Type = ReadType (xMediaFile.Metadata);

            xMediaFile.Model = ReadModel (xMediaFile.Metadata);

            if (ReadDateTime (xMediaFile.Metadata, out DateTimeSource xDateTimeSource, out DateTime xDateTime))
            {
                xMediaFile.DateTimeSource = xDateTimeSource;
                xMediaFile.DateTime = xDateTime;
            }

            return xMediaFile;
        }
    }
}
