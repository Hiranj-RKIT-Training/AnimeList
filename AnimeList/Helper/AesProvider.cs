using System;
using System.Security.Cryptography;
using System.Text;

namespace AnimeList.Helper
{
    /// <summary>
    /// Provides AES encryption functionality using a fixed key and IV.
    /// </summary>
    public class AesProvider
    {
        // 32-byte key for AES-256 encryption (must be kept secure)
        private static readonly string _generatedKey = "0123456789ABCDEF0123456789ABCDEF"; // 256-bit key
        // 16-byte initialization vector (IV) for AES block size (128-bit)
        private static readonly string _iv = "0123456789ABCDEF"; // 128-bit IV

        /// <summary>
        /// Encrypts the given plain text using AES encryption.
        /// </summary>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted text as a Base64-encoded string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input plain text is null.</exception>
        public static string Encryption(string plainText)
        {
            // Check if the plainText is null, and throw an exception if it is
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText), "Input plain text cannot be null.");

            // Convert plain text to bytes
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            // Convert the key and IV to bytes
            var keyBytes = Encoding.UTF8.GetBytes(_generatedKey);
            var ivBytes = Encoding.UTF8.GetBytes(_iv);

            // Use the RijndaelManaged class (AES) for encryption
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                // Set the encryption parameters
                rijndael.Key = keyBytes; // Set the key
                rijndael.IV = ivBytes;   // Set the IV
                rijndael.Padding = PaddingMode.PKCS7; // Ensure the padding mode is PKCS7 for correct block size handling
                rijndael.Mode = CipherMode.CBC; // Use CBC (Cipher Block Chaining) mode for encryption

                // Create the encryptor using the key and IV
                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                // Perform the encryption and return the encrypted bytes
                var encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

                // Convert the encrypted bytes to a Base64 string and return it
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }
}
