using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Pasteimg.Server.Models
{
    public static class IFormFileExtension
    {
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

        public static byte[] ToArray(this IFormFile file)
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
    }
}