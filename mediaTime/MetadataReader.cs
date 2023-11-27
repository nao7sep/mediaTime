using System.Globalization;
using System.Text.RegularExpressions;
using yyLib;

namespace mediaTime
{
    public static partial class MetadataReader
    {
        [GeneratedRegex (@"^(?<Timestamp>20[0-9]{6}-[0-9]{6}) \((?<OriginalFileNameWithoutExtension>.+?)\)$", RegexOptions.CultureInvariant)]
        private static partial Regex TimestampAndOriginalFileNameWithoutExtensionGeneratedRegex ();

        private static MediaFileType ReadType (IReadOnlyList <MetadataExtractor.Directory>? metadata)
        {
            if (metadata == null)
                return MediaFileType.Unsupported;

            // Confirmed all JPEG files (that I currently had) contained this directory.

            if (metadata.Any (x => x.Name.Equals ("JPEG", StringComparison.OrdinalIgnoreCase)))
                return MediaFileType.Image;

            // Tested with a number of video files that I had in my old & new hard disks.

            // 1. ".avi" files contained the "AVI" directory and nothing related to QuickTime.
            // 2. ".mts" and ".m2ts" files were unreadable.
            // 3. Some old QuickTime files contained local times in "Created" and "Modified".

            // Currently, no other issues have been found.

            if (metadata.Any (x => x.Name.Equals ("QuickTime File Type", StringComparison.OrdinalIgnoreCase)))
                return MediaFileType.Video;

            return MediaFileType.Unsupported;
        }

        // Result is trimmed.
        // One of my current cameras returns "RICOH GR III       ".

        // "Canon EOS 70D" seems to set "Model" in video files.
        // https://exiftool.org/TagNames/QuickTime.html

        private static string? ReadModel (IReadOnlyList <MetadataExtractor.Directory>? metadata) =>
            (metadata?.FirstOrDefault (x => x.Name.Equals ("Exif IFD0", StringComparison.OrdinalIgnoreCase))?.Tags.
                FirstOrDefault (x => x.Name.Equals ("Model", StringComparison.OrdinalIgnoreCase))?.Description ??
            metadata?.FirstOrDefault (x => x.Name.Equals ("QuickTime Metadata Header", StringComparison.OrdinalIgnoreCase))?.Tags.
                FirstOrDefault (x => x.Name.Equals ("Model", StringComparison.OrdinalIgnoreCase))?.Description)?.Trim ();

        private static bool TryReadDateTime (string filePath, IReadOnlyList <MetadataExtractor.Directory>? metadata, out DateTimeSource dateTimeSource, out DateTime dateTime)
        {
            if (metadata != null)
            {
                static bool TryReadAndParseDateTime (IReadOnlyList <MetadataExtractor.Directory> metadata,
                    DateTimeSource expectedSource, string directoryName, string tagName, string dateTimeFormat, DateTimeStyles styles,
                    out DateTimeSource dateTimeSource, out DateTime dateTime)
                {
                    string? xDateTimeString = metadata.FirstOrDefault (x => x.Name.Equals (directoryName, StringComparison.OrdinalIgnoreCase))?.Tags.
                        FirstOrDefault (x => x.Name.Equals (tagName, StringComparison.OrdinalIgnoreCase))?.Description?.Trim (); // Trimming just to make sure.

                    if (xDateTimeString != null && DateTime.TryParseExact (xDateTimeString, dateTimeFormat, CultureInfo.InvariantCulture, styles, out dateTime))
                    {
                        dateTimeSource = expectedSource;
                        return true;
                    }

                    // These values will be ignored.
                    dateTimeSource = default;
                    dateTime = default;
                    return false;
                }

                if (TryReadAndParseDateTime (metadata, DateTimeSource.Image_Exif_DateTimeOriginal,
                        // Parses something like "2023:11:19 08:24:18".
                        // Assuming it's in local time considering the following page and files I have.
                        // https://photo.stackexchange.com/questions/96711/why-dont-exif-tags-contain-time-zone-information
                        "Exif SubIFD", "Date/Time Original", "yyyy':'MM':'dd HH':'mm':'ss", DateTimeStyles.AssumeLocal, out dateTimeSource, out dateTime) ||
                    TryReadAndParseDateTime (metadata, DateTimeSource.Image_Exif_DateTimeDigitized,
                        "Exif SubIFD", "Date/Time Digitized", "yyyy':'MM':'dd HH':'mm':'ss", DateTimeStyles.AssumeLocal, out dateTimeSource, out dateTime) ||
                    TryReadAndParseDateTime (metadata, DateTimeSource.Image_Exif_DateTime,
                        "Exif IFD0", "Date/Time", "yyyy':'MM':'dd HH':'mm':'ss", DateTimeStyles.AssumeLocal, out dateTimeSource, out dateTime) ||
                    TryReadAndParseDateTime (metadata, DateTimeSource.Video_QuickTime_Created,
                        // Parses something like "Thu Nov 16 10:45:07 2023".
                        // Assuming it's in UTC considering the following page and files I have.
                        // https://exiftool.org/TagNames/QuickTime.html
                        "QuickTime Movie Header", "Created", "ddd MMM dd HH':'mm':'ss yyyy", DateTimeStyles.AssumeUniversal, out dateTimeSource, out dateTime) ||
                    TryReadAndParseDateTime (metadata, DateTimeSource.Video_QuickTime_Modified,
                        "QuickTime Movie Header", "Modified", "ddd MMM dd HH':'mm':'ss yyyy", DateTimeStyles.AssumeUniversal, out dateTimeSource, out dateTime))
                    return true;
            }

            try
            {
                dateTime = File.GetCreationTime (filePath);
                dateTimeSource = DateTimeSource.FileSystem_CreationTime;
                return true;
            }

            catch
            {
            }

            try
            {
                dateTime = File.GetLastWriteTime (filePath);
                dateTimeSource = DateTimeSource.FileSystem_LastModifiedTime;
                return true;
            }

            catch
            {
            }

            // These values will be ignored.
            dateTimeSource = default;
            dateTime = default;
            return false;
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

            if (TryReadDateTime (filePath, xMediaFile.Metadata, out DateTimeSource xDateTimeSource, out DateTime xDateTime))
            {
                xMediaFile.DateTimeSource = xDateTimeSource;
                xMediaFile.DateTime = xDateTime;
            }

            return xMediaFile;
        }
    }
}
