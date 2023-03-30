namespace Pasteimg.Backend.Logic
{
    public class SessionSettings
    {
        public TimeSpan IdleTimeout { get; init; }
        public TimeSpan IOTimeout { get; init; }
    }
}