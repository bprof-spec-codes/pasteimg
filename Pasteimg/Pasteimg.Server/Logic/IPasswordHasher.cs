using System.Security.Cryptography;
using System.Text;
namespace Pasteimg.Server.Logic
{
    public interface IPasswordHasher
    {
        string? CreateHash(string? password);
    }
    public class PasswordHasher : IPasswordHasher
    {
        public string? CreateHash(string? password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                using (SHA256 hash = SHA256.Create())
                {

                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    Random random = new Random(password.Sum(c => ~c));
                    for (int i = 0; i < bytes.Length; i++)
                    {

                        bytes[i] = (byte)(~random.Next(bytes.Length));
                    }

                    return BitConverter.ToString(bytes).Replace("-","").ToLower();
                }
            }
            else return null;
        }
         
        }
    }
