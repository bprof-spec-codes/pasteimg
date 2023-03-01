using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using NsfwSpyNS;
using System.Drawing.Imaging;

namespace Pasteimg.Server.Logic
{
    public interface INsfwRecognizer
    {
         bool IsNsfw(byte[] binary);
    }

    public class NsfwRecognizer : INsfwRecognizer
    {
        static NsfwSpy nsfwSpy = new NsfwSpy();
        enum NsfwClass
        {
            Neutral, Pornography, Sexy, Hentai
        }
        private float treshhold=0.5f;
        public float Treshhold { get=>treshhold; set=>Math.Clamp(value,0f,1f); }
       


        private bool Decide(Dictionary<string,float> values)
        {
            float pornValue = values[NsfwClass.Pornography.ToString()];
            float hentaiValue = values[NsfwClass.Hentai.ToString()];
            return pornValue >treshhold ||hentaiValue >treshhold;
        }
        public bool IsNsfw(byte[] binary)
        {
            try
            {
                return Decide(nsfwSpy.ClassifyImage(binary).ToDictionary());
            }
            catch (ClassificationFailedException)
            {
                try
                {
                    return nsfwSpy.ClassifyGif(binary, new VideoOptions() { EarlyStopOnNsfw = true }).IsNsfw;
                }
                catch(ClassificationFailedException)
                {
                    return false;
                }
            }
        }
    }
}