namespace Pasteimg.Server.Logic
{
    public interface IKeyGenerator
    {
        string GenerateKey(IEnumerable<string> collection);
    }
    public class KeyGenerator : IKeyGenerator
    {
        public string GenerateKey(IEnumerable<string> collection)
        {
            string key;

            do
            {
                key = Guid.NewGuid().ToString().Replace("-","");

            }
            while (collection.Contains(key));
            return key;

        }
    }
}