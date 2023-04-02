namespace Pasteimg.Backend.Logic
{
    /// <summary>
    /// Represents the session settings for configuring the behavior of the session handler.
    /// </summary>
    public class SessionSettings
    {
        /// <summary>
        /// Gets or initializes the idle timeout for the session.
        /// </summary>
        public TimeSpan IdleTimeout { get; init; }
        /// <summary>
        /// Gets or initializes the I/O timeout for the session.
        /// </summary>
        public TimeSpan IOTimeout { get; init; }
    }
}