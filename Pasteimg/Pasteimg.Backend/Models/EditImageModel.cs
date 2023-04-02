namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Represents the model used for editing an image.
    /// </summary>
    public record EditImageModel
    {
        /// <summary>
        /// Gets or sets the description of the image.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image is NSFW (not safe for work).
        /// </summary>
        public bool NSFW { get; set; }
    }
}