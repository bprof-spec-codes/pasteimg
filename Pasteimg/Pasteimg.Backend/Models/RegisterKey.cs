using System.Diagnostics.CodeAnalysis;

namespace Pasteimg.Backend.Models
{
    public class RegisterKey : IEqualityComparer<RegisterKey>
    {
        public int Key { get; set; }
        public DateTime Creation { get; set; }

        public bool Equals(RegisterKey? x, RegisterKey? y)
        {
            if(x.Key == y.Key) return true;
            return false;
        }

        public int GetHashCode([DisallowNull] RegisterKey obj)
        {
            throw new NotImplementedException();
        }
    }
}
