using System.Net;

namespace Pasteimg.Backend.Logic.Exceptions
{
    [HttpError(HttpStatusCode.Conflict, 777)]
    public class WrongRegisterKey : Exception
    {
        public WrongRegisterKey(): base("Wrong register key") { }
    }
}
