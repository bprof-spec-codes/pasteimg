using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    [HttpError(HttpStatusCode.Conflict, 777)]
    public class RegisterError : Exception
    {
        public RegisterError(string msg): base(msg) { }
    }
}
