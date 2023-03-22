namespace Pasteimg.Server.ImageTransformers
{
    public record ImageInfo(int Width, int Height, long FileSize, string Format,string? Path)
    {
        public string? FileName => Path is not null ? System.IO.Path.GetFileNameWithoutExtension(Path) : null;
        public string? Extension => Path is not null ? System.IO.Path.GetExtension(Path).Trim('.') : null;
    }
}