using Pasteimg.Server.Models.Entity;
using System.Net.Sockets;

namespace Pasteimg.Server.Models.Error
{
    public class LockoutException : PasteImgException
    {
        public LockoutException(string id) : 
            base(typeof(Upload), id, $"Resource access is locked out. Id: {id}")
        {
        }

    }
}