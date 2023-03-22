namespace Pasteimg.Server.Models.Entity
{
    /// <summary>
    /// Alapvető interface-e az adatbázis entitásoknak. 
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Visszatér az entitás egyedi azonosító kulcsával.
        /// </summary>
        /// <returns></returns>
        object[] GetKey();
    }
}