namespace Pasteimg.Backend.Configurations
{
    /// <summary>
    /// Represents the visitor configuration settings.
    /// </summary>
    public class VisitorConfiguration
    {
        /// <summary>
        /// Gets or sets the time, in minutes, after failed attempts tracking is reset for a visitor who is not currently locked out.
        /// </summary>
        public int LockoutTresholdInMinutes { get; init; }

        /// <summary>
        /// Gets or sets the maximum number of failed attempts allowed for a visitor.
        /// </summary>
        public int MaxFailedAttempt { get; init; }
    }
}