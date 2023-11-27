using System.Globalization;
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

                MetadataReaderTester.Test (args);
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
