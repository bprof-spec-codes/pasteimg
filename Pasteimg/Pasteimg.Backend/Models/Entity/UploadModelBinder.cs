using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pasteimg.Backend.Models.Entity
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
                var files = provider.GetFiles();
                foreach (var item in files)
                {
                    if (TryGetIndex(item.Name, out string index))
                    {
                        images[index].Content = item;
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