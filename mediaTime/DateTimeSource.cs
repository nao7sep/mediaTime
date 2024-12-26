namespace yyMediaTime
{
    public enum DateTimeSource
    {
        // ChatGPT:

            // EXIF Date and Time Tags Reading Order:

            // 1. DateTimeOriginal: The primary tag for the original date and time the photo was taken.
            //    Use this tag first as it provides the most accurate timestamp of when the picture was actually shot.

            // 2. DateTimeDigitized: If DateTimeOriginal is not available, fall back to this tag.
            //    It represents the date and time the image was digitized. For digital cameras, it's often the same as DateTimeOriginal.
            //    This tag is more relevant for scanned images or digital copies of analog photos.

            // 3. DateTime: Use this tag if the above two are not available.
            //    It generally indicates the last modification date and time of the image file.
            //    However, be aware that it can be altered by image editing software and might not reflect the original capture time.

        Image_Exif_DateTimeOriginal,
        Image_Exif_DateTimeDigitized,
        Image_Exif_DateTime,

        // ChatGPT:

            // QuickTime Video File Timestamps Reading Order:

            // 1. Created: This timestamp indicates when the video file was originally created.
            //    It should be read first as it's the closest indication of the actual recording time of the video content.
            //    However, be aware that in some cases, it might reflect the file creation time, which can be different from the recording time.

            // 2. Modified: This timestamp shows when the video file was last modified.
            //    Use this if you are interested in tracking when the file underwent editing, conversion, or any other form of modification.
            //    Remember, this timestamp does not indicate the original recording time but rather the last time the file itself was altered.

        Video_QuickTime_Created,
        Video_QuickTime_Modified,

        // ChatGPT:

            // Determining Image Creation Date and Time Without Metadata:

            // Use filesystem timestamps in the following order:

            // For Windows and macOS:

            // 1. Creation Time (Windows) / Date Created (macOS):
            //    - This timestamp should be your first reference to determine when the file was created.
            //    - Be cautious: This timestamp can change if the file is copied or moved.

            // 2. Last Modified Time (Windows) / Date Modified (macOS):
            //    - Use this if the Creation Time isn't reliable or appears inaccurate.
            //    - It indicates the last time the file's contents were changed.

            // For Linux:

            // Since Linux traditionally doesn't store the file creation time, use the following:

            // 1. mtime (Modification Time):
            //    - The primary timestamp for when the file's content was last modified.

            // 2. ctime (Change Time):
            //    - Use this if mtime is too recent and might be due to a recent content change.
            //    - Reflects the last time the file's metadata changed.
            //    - Note: ctime updates for changes to the file's inode information, not just its content.

        // Added later: When a file is copied, Windows may set the Creation Time to the current time,
        // making it a lot later than the Last Modified Time.

        // However, if we just use the smaller value of the two, we may retrieve an epoch time value from somewhere.
        // Like, I've seen files where one or more of the timestamps' years were 1970/1980 while the others seemed accurate
        // most likely because the data was missing and one of the programs that dealt with them just returned one of the internal offset values.
        // https://en.wikipedia.org/wiki/Unix_time
        // http://elm-chan.org/docs/fat_e.html

        // One should-be-OK workaround is to read the Creation Time if it's available,
        // continue with reading the Last Modified Time and use the smaller value that is equal to or later than the year 1980 + 1.
        // I dont really remember seeing the Last Modified Time looking like an epoch time value, though.

        FileSystem_CreationTime,
        FileSystem_LastModifiedTime
    }
}
