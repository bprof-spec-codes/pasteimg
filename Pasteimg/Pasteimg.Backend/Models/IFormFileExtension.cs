using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Extension methods for IFormFile and IValueProvider classes.
    /// </summary>
    public static class IFormFileExtension
    {
        /// <summary>
        /// Read the content of an IFormFile into a byte array.
        /// </summary>
        /// <param name="file">An IFormFile instance.</param>
        /// <returns>A byte array containing the file content, or an empty byte array if the file is null.</returns>
        public static byte[] ToArray(this IFormFile file)
        {
            if (file is not null)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    using (MemoryStream destination = new MemoryStream())
                    {
                        stream.CopyTo(destination);
                        return destination.ToArray();
                    }
                }
            }
            else return new byte[] { };
        }
    }
}