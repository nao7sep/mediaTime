using System.Globalization;
using System.Text.Json;
using System.Text;
using yyLib;

namespace yyMediaTime
{
#if DEBUG
    public static class MetadataReaderTester
    {
        public static void Test (string [] args)
        {
            var xFiles = args.Select (x => MetadataReader.Read (x));

            Console.WriteLine (string.Join ($"{Environment.NewLine}{Environment.NewLine}", xFiles.Select (x =>
            {
                StringBuilder xBuilder = new ();

                xBuilder.AppendLine ($"File Name: {Path.GetFileName (x.FilePath)}");
                xBuilder.AppendLine ($"Type: {x.Type}");
                xBuilder.AppendLine ($"Model: {x.Model.GetVisibleString ()}");

                if (x.DateTimeSource != null)
                {
                    xBuilder.AppendLine ($"Date/Time Source: {x.DateTimeSource}");
                    xBuilder.AppendLine ($"Date/Time: {x.DateTime!.Value.ToString ("yyyy'-'MM'-'dd HH':'mm':'ss")}");

                    DateTime xAssumedLocalTime =
                        x.DateTimeSource == DateTimeSource.Video_QuickTime_Created || x.DateTimeSource == DateTimeSource.Video_QuickTime_Modified ?
                            x.DateTime!.Value.ToLocalTime () : x.DateTime!.Value;

                    xBuilder.AppendLine ($"Minutes from File System Timestamp: {Math.Round (xAssumedLocalTime.Subtract (File.GetLastWriteTime (x.FilePath!)).TotalMinutes)}");
                }

                else
                {
                    xBuilder.AppendLine ("Date/Time Source: (N/A)");
                    xBuilder.AppendLine ("Date/Time: (N/A)");
                }

                return xBuilder.ToString ();
            })));

            string xFilePath = Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.DesktopDirectory),
                    $"Files-{DateTime.Now.ToString ("yyyyMMdd'-'HHmmss", CultureInfo.InvariantCulture)}.json"),
                xFileContents = JsonSerializer.Serialize (xFiles, yyJson.DefaultSerializationOptions);

            File.WriteAllText (xFilePath, xFileContents, Encoding.UTF8);
        }
    }
#endif
}
