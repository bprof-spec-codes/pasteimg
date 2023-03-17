namespace Pasteimg.Server.Transformers
{
    public class BackgroundTransformationRequest
    {
        public BackgroundTransformationRequest(string path, IImageTransformer transformer)
        {
            Path = path;
            Transformer = transformer;
        }

        public string Path { get; }
        public IImageTransformer Transformer { get; }

        public string Transform()
        {
            return Transformer.Transform(Path);
        }
    }
}