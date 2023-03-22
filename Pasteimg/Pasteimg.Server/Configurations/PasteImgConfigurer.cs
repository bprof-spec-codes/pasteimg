using Newtonsoft.Json;

namespace Pasteimg.Server.Configurations
{
    public interface IPasteImgConfigurer
    {
        string DefaultPath { get; set; }

        PasteImgConfiguration ReadConfiguration();

        void WriteConfiguration(PasteImgConfiguration config);
    }

    public class PasteImgConfigurer : IPasteImgConfigurer
    {
        public string DefaultPath { get; set; } = "pasteImgLogic.json";

        public PasteImgConfiguration? ReadConfiguration(string path)
        {
            return JsonConvert.DeserializeObject<PasteImgConfiguration>(File.ReadAllText(path));
        }

        public PasteImgConfiguration ReadConfiguration()
        {
            try
            {
                PasteImgConfiguration? config = ReadConfiguration(DefaultPath);
                PasteImgConfiguration.Validate(config);
                return config;
            }
            catch
            {
                WriteConfiguration(PasteImgConfiguration.Default);
                return ReadConfiguration(DefaultPath);
            }
        }

        public void WriteConfiguration(PasteImgConfiguration config, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public void WriteConfiguration(PasteImgConfiguration config)
        {
            WriteConfiguration(config, DefaultPath);
        }
    }
}