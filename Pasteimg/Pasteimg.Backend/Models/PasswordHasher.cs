using System.Security.Cryptography;
using System.Text;

namespace Pasteimg.Backend.Models
{
    /// <summary>
    /// Defines an interface for creating password hashes.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Creates a hash for a given password.
        /// </summary>
        /// <param name="password">The password to create a hash for.</param>
        /// <returns>The hashed password.</returns>
        string? CreateHash(string? password);
    }
    /// <summary>
    /// Provides an implementation of the <see cref="IPasswordHasher"/> interface using the SHA256 algorithm.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Creates a hash for a given password using the SHA256 algorithm.
        /// </summary>
        /// <param name="password">The password to create a hash for.</param>
        /// <returns>The hashed password.</returns>
        public string? CreateHash(string? password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                using (SHA256 hash = SHA256.Create())
                {
                    int randomSeed = 0;
                    unchecked
                    {
                        foreach (var chr in password)
                        {
                            randomSeed += ~chr;
                        }
                    }

                    Random random = new Random(randomSeed);
                    byte[] salt = new byte[10];
                    random.NextBytes(salt);
                    password += new string(salt.Select(b => (char)b).ToArray());
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    for (int i = bytes.Length - 1; i > 1; i--)
                    {
                        int j = random.Next(0, i + 1);
                        if (i != j)
                        {
                            byte temp = bytes[i];
                            bytes[i] = bytes[j];
                            bytes[j] = temp;
                        }
                    }

                    return BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }
            }
            else return null;
        }
    }
}