namespace Pasteimg.Backend.Models.Entity
{
    /// <summary>
    ///  Defines a common structure for entities in a data model or system.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets an array of objects that uniquely identify the entity.
        /// </summary>
        /// <returns>An array of objects containing the entity's unique key values.</returns>
        object[] GetKey();
    }
}