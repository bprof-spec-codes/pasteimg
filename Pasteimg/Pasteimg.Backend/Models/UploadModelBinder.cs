﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// A custom model binder for the <see cref="Upload"/>, which extracts data from Form and binds it to model.
    /// </summary>
    public class UploadModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds Form data to <see cref="Upload"/>.
        /// </summary>
        /// <param name="bindingContext">The ModelBindingContext containing information about the model binding process.</param>
        /// <returns>A Task that represents the asynchronous model binding operation.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ValueProvider is CompositeValueProvider provider)
            {
                Dictionary<string, Image> images = new Dictionary<string, Image>();

                foreach (var imageKey in provider.GetKeysFromPrefix(nameof(Upload.Images)))
                {
                    var props = provider.GetKeysFromPrefix(imageKey.Value);
                    if (!images.ContainsKey(imageKey.Key))
                    {
                        images[imageKey.Key] = new Image();
                    }

                    string? description = provider.GetValue(props[nameof(Image.Description)]).FirstValue;
                    bool.TryParse(provider.GetValue(props[nameof(Image.NSFW)]).FirstValue, out bool nsfw);
                    images[imageKey.Key].Description = description;
                    images[imageKey.Key].NSFW = nsfw;
                }
                var files = GetFiles(provider);
                foreach (var item in files)
                {
                    if (TryGetIndex(item.Name, out string index))
                    {
                        images[index].Content = FormFileToContent(item);
                    }
                }
                Upload model = new Upload();
                var password = provider.GetValue(nameof(Upload.Password)).FirstValue;
                model.Password = !string.IsNullOrEmpty(password) ? password : null;
                model.Images = images.Select(kv => kv.Value).ToList();
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Converts a given <see cref="IFormFile"/> to a <see cref="Content"/> object.
        /// </summary>
        /// <param name="file">The file to convert.</param>
        /// <returns>A new <see cref="Content"/> object containing the data from the file.</returns>
        private Content FormFileToContent(IFormFile file)
        {
            using Stream content = file.OpenReadStream();
            using MemoryStream stream = new MemoryStream();
            content.CopyTo(stream);
            byte[] data = stream.ToArray();
            return new Content()
            {
                ContentType = file.ContentType,
                FileName = file.FileName,
                Data = data
            };
        }
        /// <summary>
        /// Retrieves a collection of files from a value provider.
        /// </summary>
        /// <param name="provider">The value provider to retrieve the files from.</param>
        /// <returns>A collection of files, or null if none were found.</returns>
        private IFormFileCollection? GetFiles(IValueProvider? provider)
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
        /// Try to get the index from the given key.
        /// </summary>
        /// <param name="key">The key to parse for the index.</param>
        /// <param name="index">An out parameter that will contain the parsed index if successful.</param>
        /// <returns> True if the index was successfully parsed, false otherwise.</returns>
        private bool TryGetIndex(string key, out string index)
        {
            index = "";
            if (key.Length > 0)
            {
                int left = key.IndexOf("[");
                int right = key.IndexOf("]");
                index = key.Substring(left + 1, right - left - 1);
                return left >= 0 && right >= 0;
            }
            else return false;
        }
    }
}