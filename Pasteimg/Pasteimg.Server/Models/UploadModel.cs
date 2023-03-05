using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Pasteimg.Server.Models
{

    [ModelBinder(BinderType = typeof(UploadModelBinder))]
    public class UploadModel : UploadBase
    {
        public List<ImageModel> Images { get; set; }

        public UploadModel()
        {
            Images = new List<ImageModel>();
        }
    }

    public class UploadModelBinder : IModelBinder
    {
     
        private bool TryGetIndex(string key, out string index)
        {
            index="";
            if(key.Length>0)
            {
                int left = key.IndexOf("[");
                int right = key.IndexOf("]");
                index = key.Substring(left + 1, right - left - 1);
                return left >= 0 && right >= 0;
            }
            else return false;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ValueProvider is CompositeValueProvider provider)
            {
                Dictionary<string, ImageModel> images = new Dictionary<string, ImageModel>();
                
                foreach (var imageKey in provider.GetKeysFromPrefix(nameof(UploadModel.Images)))
                {
                    var props = provider.GetKeysFromPrefix(imageKey.Value);
                    if (!images.ContainsKey(imageKey.Key))
                    {
                        images[imageKey.Key] = new ImageModel();
                    }

                    string? description= provider.GetValue(props[nameof(ImageModel.Description)]).FirstValue;
                    bool.TryParse(provider.GetValue(props[nameof(ImageModel.NSFW)]).FirstValue,out bool nsfw);
                    images[imageKey.Key].Description = description;
                    images[imageKey.Key].NSFW = nsfw;
                }
                var files = provider.GetFiles();
                foreach (var item in files)
                {
                    if(TryGetIndex(item.Name,out string index))
                    {
                        images[index].Content = item;
                    }
                }
                UploadModel model = new UploadModel();
                var password = provider.GetValue(nameof(Upload.Password)).FirstValue;
                model.Password = !string.IsNullOrEmpty(password) ? password : null;
                model.Images = images.Select(kv => kv.Value).ToList();
                bindingContext.Result = ModelBindingResult.Success(model);

            }
            return Task.CompletedTask;
        }
    }
}
