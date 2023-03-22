using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Pasteimg.Server.Models
{
    /// <summary>
    ///  <see cref="IFormFile"/>-lal kapcsolatos kiegészítő-, segítőmetódusok.
    /// </summary>
    public static class IFormFileExtension
    {
        /// <summary>
        /// Kinyeri <see cref="IValueProvider"/>-ből a küldő form fájljait.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
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
        /// FormFile tartalmát átalakítja byte tömbbé.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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