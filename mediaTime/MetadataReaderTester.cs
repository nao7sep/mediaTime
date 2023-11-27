using System.Globalization;
using System.Text.Json;
using System.Text;
using yyLib;

namespace mediaTime
{
    public static class MetadataReaderTester
    {
#if DEBUG
        public static void Test (string [] args)
        {
            var xFiles = args.Select (x => MetadataReader.Read (x));

            Console.WriteLine (string.Join ($"{Environment.NewLine}{Environment.NewLine}", xFiles.Select (x =>
            {
                StringBuilder xBuilder = new StringBuilder ();
                xBuilder.AppendLine ($"File Name: {Path.GetFileName (x.FilePath)}");
                xBuilder.AppendLine ($"Type: {x.Type}");
                xBuilder.AppendLine ($"Model: {x.Model.GetVisibleString ()}");
                xBuilder.AppendLine ($"Date/Time Source: {x.DateTimeSource}");
                xBuilder.AppendLine ($"Date/Time: {x.DateTime!.Value.ToString ("yyyy'-'MM'-'dd HH':'mm':'ss")}");
                xBuilder.AppendLine ($"Minutes from File System Timestamp: {Math.Round (x.DateTime!.Value.ToUniversalTime ().Subtract (File.GetLastWriteTimeUtc (x.FilePath!)).TotalMinutes)}");
                return xBuilder.ToString ();
            })));

            string xFilePath = Path.Join (Environment.GetFolderPath (Environment.SpecialFolder.DesktopDirectory),
                    $"Files-{DateTime.Now.ToString ("yyyyMMdd'-'HHmmss", CultureInfo.InvariantCulture)}.json"),
                xFileContents = JsonSerializer.Serialize (xFiles, yyJson.DefaultSerializationOptions);

            File.WriteAllText (xFilePath, xFileContents, Encoding.UTF8);
        }
#endif
    }
}
