using Pasteimg.Server.Models.Entity;
using System.Net.Sockets;

namespace Pasteimg.Server.Models.Error
{
    /// <summary>
    /// A kivétel akkor dobódik, amikor a kliens megpróbál megadni egy jelszót egy védett tartalomhoz,
    /// de múltban már kizárta a rendszer a sikertelen próbálkozásai miatt. 
    /// </summary>
    public class LockoutException : PasteImgException
    {
        public LockoutException(string id) : 
            base(typeof(Upload), id, $"Resource access is locked out. Id: {id}")
        {
        }

    }
}