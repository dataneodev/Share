namespace Vero.Shared.Helpers
{
    public static class FileNameHelper
    {
        public static string FormatFileName(string rawFileName) => string.Concat(rawFileName.Split(Path.GetInvalidFileNameChars()))
            .Replace(" ", "_")
            .Trim();
    }
}