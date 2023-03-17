namespace Pasteimg.Server.Configurations
{
    public class VisitorConfiguration
    {
        public int LockoutTimeInMinutes { get; init; }
        public int MaxFailedAttempt { get; init; }
    }
}