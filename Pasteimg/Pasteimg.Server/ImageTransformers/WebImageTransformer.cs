namespace Pasteimg.Server.Transformers
{
    public abstract class WebImageTransformer : IImageTransformer
    {
        public WebImageTransformer(int maxWidth, int maxHeight, int quality)
        {
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Quality = quality;
        }

        public int MaxHeight { get; }
        public int MaxWidth { get; }
        public int Quality { get; }

        public abstract string Transform(string path);

        public abstract byte[] Transform(byte[] content);

        protected (int width, int height) ClampSize(float width, float height)
        {
            if (width > MaxWidth)
            {
                float wratio = MaxWidth / width;
                width *= wratio;
                height *= wratio;
            }

            if (height > MaxHeight)
            {
                float hratio = MaxHeight / height;
                width *= hratio;
                height *= hratio;
            }

            return new((int)Math.Round(width, 0), (int)Math.Round(height, 0));
        }
    }
}