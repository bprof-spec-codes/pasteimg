namespace Pasteimg.Server.Configurations
{
    public class VisitorConfiguration
    {
        public int LockoutTresholdInMinutes { get; init; }
        public int MaxFailedAttempt { get; init; }
    }
}