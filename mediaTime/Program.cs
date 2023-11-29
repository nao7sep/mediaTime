using System.Globalization;
using System.Text;
using System.Text.Json;
using yyLib;

namespace mediaTime
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            {
                // Required to read date and time info from video files.
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                DateTime xStartNow = DateTime.Now;
                string xStartNowString = xStartNow.ToString ("yyyyMMdd'-'HHmmss");

                // -----------------------------------------------------------------------------

                // Creates an unique sorted list of files to process.

                if (args.Length == 0)
                {
                    Console.WriteLine ("Please drag & drop media files to rename to the program's execution file.");
                    return;
                }

                List <string> xFilePaths = new ();

                foreach (string xArg in args)
                {
                    if (string.IsNullOrWhiteSpace (xArg) || Path.IsPathFullyQualified (xArg) == false)
                    {
                        // Visualizing a (probably unintended) argument such as "".
                        Console.WriteLine ($"Invalid path: {xArg.GetVisibleString ()}");
                        return;
                    }

                    if (Directory.Exists (xArg))
                        xFilePaths.AddRange (Directory.EnumerateFiles (xArg, "*.*", SearchOption.AllDirectories));

                    else if (File.Exists (xArg))
                        xFilePaths.Add (xArg);

                    else
                    {
                        Console.WriteLine ($"Directory or file not found: {xArg.GetVisibleString ()}");
                        return;
                    }
                }

                if (xFilePaths.Count == 0)
                {
                    Console.WriteLine ("No files found.");
                    return;
                }

                xFilePaths = xFilePaths.Distinct (StringComparer.OrdinalIgnoreCase).Order (StringComparer.OrdinalIgnoreCase).ToList ();

                // -----------------------------------------------------------------------------

                // Displays statistics.

                var xFiles = xFilePaths.Select (x => MetadataReader.Read (x));

                int xImageFileCount = xFiles.Count (x => x.Type == MediaFileType.Image),
                    xVideoFileCount = xFiles.Count (x => x.Type == MediaFileType.Video);

                var xImageFilesWithoutDateTimeInMetadata = xFiles.Where (x =>
                    x.Type == MediaFileType.Image &&
                    (x.DateTimeSource == DateTimeSource.Image_Exif_DateTimeOriginal ||
                    x.DateTimeSource == DateTimeSource.Image_Exif_DateTimeDigitized ||
                    x.DateTimeSource == DateTimeSource.Image_Exif_DateTime) == false).ToArray ();

                var xVideoFilesWithoutDateTimeInMetadata = xFiles.Where (x =>
                    x.Type == MediaFileType.Video &&
                    (x.DateTimeSource == DateTimeSource.Video_QuickTime_Created ||
                    x.DateTimeSource == DateTimeSource.Video_QuickTime_Modified) == false).ToArray ();

                var xUnsupportedFiles = xFiles.Where (x => x.Type == MediaFileType.Unsupported).ToArray ();

                var xFilesWithoutDateTimeAtAll = xFiles.Where (x => x.DateTimeSource == null).ToArray ();

                StringBuilder xBuilder = new ();

                xBuilder.AppendLine ($"Found Files: {xFilePaths.Count} (Image Files: {xImageFileCount}, Video Files: {xVideoFileCount})");

                xBuilder.AppendLine ($"Image Files without Date/Time in Metadata: {xImageFilesWithoutDateTimeInMetadata.Length}");

                if (xImageFilesWithoutDateTimeInMetadata.Length > 0)
                    xBuilder.AppendLine (string.Join (Environment.NewLine, xImageFilesWithoutDateTimeInMetadata.Select (x => $"    {x.FilePath}")));

                xBuilder.AppendLine ($"Video Files without Date/Time in Metadata: {xVideoFilesWithoutDateTimeInMetadata.Length}");

                if (xVideoFilesWithoutDateTimeInMetadata.Length > 0)
                    xBuilder.AppendLine (string.Join (Environment.NewLine, xVideoFilesWithoutDateTimeInMetadata.Select (x => $"    {x.FilePath}")));

                xBuilder.AppendLine ($"Unsupported Files: {xUnsupportedFiles.Length} (Will be renamed using the file system's timestamps)");

                if (xUnsupportedFiles.Length > 0)
                    xBuilder.AppendLine (string.Join (Environment.NewLine, xUnsupportedFiles.Select (x => $"    {x.FilePath}")));

                xBuilder.AppendLine ($"Files without Date/Time at all: {xFilesWithoutDateTimeAtAll.Length}");

                if (xFilesWithoutDateTimeAtAll.Length > 0)
                    xBuilder.AppendLine (string.Join (Environment.NewLine, xFilesWithoutDateTimeAtAll.Select (x => $"    {x.FilePath}")));

                Console.Write (xBuilder.ToString ());

                // -----------------------------------------------------------------------------

                // Creates a JSON file with the files' metadata.

                string xFilesFilePath = Path.Join (yyApplicationDirectory.MapPath ("Logs"), $"Files-{xStartNowString}.json"),
                    xFilesFileContents = JsonSerializer.Serialize (xFiles, yyJson.DefaultSerializationOptions);

                yyDirectory.CreateParent (xFilesFilePath);
                File.WriteAllText (xFilesFilePath, xFilesFileContents);

                Console.WriteLine ($"Created: {xFilesFilePath}");

                // -----------------------------------------------------------------------------

                // Cant continue if at least one file lacks date/time info completely.

                if (xFilesWithoutDateTimeAtAll.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine ("Cant continue with files without date/time info at all.");
                    Console.WriteLine ("It is recommended to investigate them.");
                    Console.ResetColor ();
                }

                // -----------------------------------------------------------------------------

                // Shows a warning if the input files are a mixture of various types and confirms before continuation.

                int xFileTypeVariationCount =
                    (xImageFileCount > 0 ? 1 : 0) +
                    (xVideoFileCount > 0 ? 1 : 0) +
                    (xUnsupportedFiles.Length > 0 ? 1 : 0);

                int xImageFileVariationCount =
                    ((xImageFileCount - xImageFilesWithoutDateTimeInMetadata.Length) > 0 ? 1 : 0) +
                    (xImageFilesWithoutDateTimeInMetadata.Length > 0 ? 1 : 0);

                int xVideoFileVariationCount =
                    ((xVideoFileCount - xVideoFilesWithoutDateTimeInMetadata.Length) > 0 ? 1 : 0) +
                    (xVideoFilesWithoutDateTimeInMetadata.Length > 0 ? 1 : 0);

                if (xFileTypeVariationCount > 1 || xImageFileVariationCount > 1 || xVideoFileVariationCount > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine ("Dealing with a mixture of various files at once isnt recommended.");
                    Console.WriteLine ("Consider dividing them into groups and processing them group by group.");
                    Console.ResetColor ();
                }

                Console.Write ("Press any key to continue or just close this window to exit: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }

            catch (Exception xException)
            {
                yySimpleLogger.Default.TryWriteException (xException);
                Console.WriteLine (xException);
            }

            finally
            {
                Console.Write ("Press any key to close this window: ");
                Console.ReadKey (true);
                Console.WriteLine ();
            }
        }
    }
}
