namespace mediaTime
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

        FileSystem_CreationTime,
        FileSystem_LastModifiedTime
    }
}
