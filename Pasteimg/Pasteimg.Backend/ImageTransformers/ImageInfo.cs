namespace Pasteimg.Backend.ImageTransformers
{
    /// <summary>
    /// Represents the image information such as width, height, file size, format, path.
    /// </summary>
    /// <param name="Width">The width of the image.</param>
    /// <param name="Height">The height of the image.</param>
    /// <param name="FileSize">The filesize of the image in bytes.</param>
    /// <param name="Format">The format of the image.</param>
    /// <param name="Path">The path of the image.</param>
    public record ImageInfo(int Width, int Height, long FileSize, string Format, string? Path)
    {
        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        public string? FileName => Path is not null ? System.IO.Path.GetFileNameWithoutExtension(Path) : null;
        /// <summary>
        /// Gets the file extension without the leading period.
        /// </summary>
        public string? Extension => Path is not null ? System.IO.Path.GetExtension(Path).Trim('.') : null;
    }
}