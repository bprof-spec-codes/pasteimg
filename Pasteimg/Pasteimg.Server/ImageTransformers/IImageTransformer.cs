namespace Pasteimg.Server.ImageTransformers
{
    public interface IImageTransformer
    {
        ImageInfo GetImageInfo(byte[] content);

        ImageInfo GetImageInfo(string path);

        byte[] Transform(byte[] content);

        string Transform(byte[] content, string outputPath);
    }
}