using Microsoft.AspNetCore.Session;
using Pasteimg.Backend.Controllers;

namespace Pasteimg.Backend.Logic
{
    public interface ISessionHandler
    {
        string CreateNewSession();
        ISession? GetSession(string? sessionKey);
        Task<ISession?> GetSessionAsync(string? sessionKey);
    }
    public class SessionHandler : ISessionHandler
    {
        private readonly ISessionStore sessionStore;
        public TimeSpan IdleTimeout { get; }
        public TimeSpan IOTimeout { get; }
        public SessionHandler(ISessionStore sessionStore,SessionSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.sessionStore = sessionStore;
            IdleTimeout = settings.IdleTimeout;
            IOTimeout = settings.IOTimeout;
        }
        public string CreateNewSession()
        {
            string sessionKey = Guid.NewGuid().ToString();
            ISession session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, true);
            return sessionKey;
        }
        public ISession? GetSession(string? sessionKey)
        {
            if(sessionKey is null)
            {
                return null;
            }

            var session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, false);
            session.LoadAsync().Wait();
            return session;

        }
        public async Task<ISession?> GetSessionAsync(string? sessionKey)
        {
            if(sessionKey is null)
            {
                return null;
            }
            var session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, false);
            await session.LoadAsync();
            return session;

        }
    }
}