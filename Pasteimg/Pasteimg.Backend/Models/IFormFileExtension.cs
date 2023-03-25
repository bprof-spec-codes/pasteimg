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
        /// Get a collection of uploaded files from the specified <see cref="IValueProvider"></see>.
        /// </summary>
        /// <param name="provider">An IValueProvider instance.</param>
        /// <returns>A collection of uploaded files, or null if no files were uploaded.</returns>
        public static IFormFileCollection? GetFiles(this IValueProvider? provider)
        {
            if (provider is FormValueProvider formValueProvider)
            {
                return (formValueProvider.GetType()
                        .GetField("_values", BindingFlags.NonPublic | BindingFlags.Instance)?
                        .GetValue(formValueProvider) as FormCollection)?.Files;
            }
            else if (provider is CompositeValueProvider compositeProvider)
            {
                return GetFiles(compositeProvider.FirstOrDefault(vp => vp is FormValueProvider));
            }
            else return null;
        }

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