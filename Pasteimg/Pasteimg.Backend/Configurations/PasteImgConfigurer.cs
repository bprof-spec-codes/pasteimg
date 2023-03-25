using Newtonsoft.Json;

namespace Pasteimg.Backend.Configurations
{
    /// <summary>
    /// Interface for configuring the PasteImg server.
    /// </summary>
    public interface IPasteImgConfigurer
    {
        /// <summary>
        /// Gets or sets the default path for configuration file.
        /// </summary>
        string DefaultPath { get; set; }

        /// <summary>
        /// Reads the configuration file at the default path.
        /// </summary>
        /// <returns>The deserialized PasteImg configuration.</returns>
        PasteImgConfiguration ReadConfiguration();

        /// <summary>
        /// Reads the configuration file at the specified path.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        /// <returns>The deserialized PasteImg configuration.</returns>
        PasteImgConfiguration? ReadConfiguration(string path);

        /// <summary>
        /// Writes the configuration to the default path.
        /// </summary>
        /// <param name="config">The PasteImg configuration to write.</param>
        void WriteConfiguration(PasteImgConfiguration config);

        /// <summary>
        /// Writes the configuration to the specified path.
        /// </summary>
        /// <param name="config">The PasteImg configuration to write.</param>
        /// <param name="path">The path where the configuration will be written.</param>
        void WriteConfiguration(PasteImgConfiguration config, string path);
    }

    /// <summary>
    /// Configurer for the PasteImg server.
    /// </summary>
    public class PasteImgConfigurer : IPasteImgConfigurer
    {
        /// <inheritdoc/>
        public string DefaultPath { get; set; } = "pasteImgLogic.json";

        /// <inheritdoc/>

        public PasteImgConfiguration? ReadConfiguration(string path)
        {
            return JsonConvert.DeserializeObject<PasteImgConfiguration>(File.ReadAllText(path));
        }

        /// <inheritdoc/>

        public PasteImgConfiguration ReadConfiguration()
        {
            try
            {
                PasteImgConfiguration? config = ReadConfiguration(DefaultPath);
                return config;
            }
            catch
            {
                WriteConfiguration(PasteImgConfiguration.Default);
                return ReadConfiguration(DefaultPath);
            }
        }

        /// <inheritdoc/>
        public void WriteConfiguration(PasteImgConfiguration config, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        /// <inheritdoc/>
        public void WriteConfiguration(PasteImgConfiguration config)
        {
            WriteConfiguration(config, DefaultPath);
        }
    }
}