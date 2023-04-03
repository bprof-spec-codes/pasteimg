using Microsoft.AspNetCore.Session;

namespace Pasteimg.Backend.Logic
{
    /// <summary>
    /// Interface defining a session handler.
    /// </summary>
    public interface ISessionHandler
    {
        /// <summary>
        /// Creates a new session and returns the session key.
        /// </summary>
        string CreateNewSession();

        /// <summary>
        /// Retrieves an existing session based on a session key.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>The session if it exists, null otherwise.</returns>
        ISession? GetSession(string? sessionKey);
        /// <summary>
        /// Retrieves an existing session based on a session key asynchronously.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>The session if it exists, null otherwise.</returns>
        Task<ISession?> GetSessionAsync(string? sessionKey);
    }
    /// <summary>
    /// Class implementing a session handler.
    /// </summary>
    public class SessionHandler : ISessionHandler
    {
        private readonly ISessionStore sessionStore;
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionHandler"/> class.
        /// </summary>
        /// <param name="sessionStore">The session store.</param>
        /// <param name="settings">The session settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when the settings parameter is null.</exception>
        public SessionHandler(ISessionStore sessionStore, SessionSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.sessionStore = sessionStore;
            IdleTimeout = settings.IdleTimeout;
            IOTimeout = settings.IOTimeout;
        }
        /// <summary>
        /// Gets the session idle timeout.
        /// </summary>
        public TimeSpan IdleTimeout { get; }
        /// <summary>
        /// Gets the session I/O timeout.
        /// </summary>
        public TimeSpan IOTimeout { get; }
        /// <summary>
        /// Creates a new session and returns the session key.
        /// </summary>
        /// <returns>The session key.</returns>
        public string CreateNewSession()
        {
            string sessionKey = Guid.NewGuid().ToString();
            ISession session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, true);
            return sessionKey;
        }
        /// <summary>
        /// Retrieves an existing session based on a session key.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>The session if it exists, null otherwise.</returns>
        public ISession? GetSession(string? sessionKey)
        {
            if (sessionKey is null)
            {
                return null;
            }

            var session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, false);
            session.LoadAsync().Wait();
            return session;
        }
        /// <summary>
        /// Retrieves an existing session based on a session key asynchronously.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <returns>The session if it exists, null otherwise.</returns>
        public async Task<ISession?> GetSessionAsync(string? sessionKey)
        {
            if (sessionKey is null)
            {
                return null;
            }
            var session = sessionStore.Create(sessionKey, IdleTimeout, IOTimeout, () => true, false);
            await session.LoadAsync();
            return session;
        }
    }
}