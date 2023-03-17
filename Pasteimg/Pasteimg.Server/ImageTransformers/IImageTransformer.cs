namespace Pasteimg.Server.Transformers
{
    public interface IImageTransformer
    {
        byte[] Transform(byte[] content);

        string Transform(string path);
    }
}