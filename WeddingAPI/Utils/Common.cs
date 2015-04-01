using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WeddingAPI.Utils
{
    public class Common
    {
        private const int SaltValueSize = 50;

        public static string GenerateSaltValue()
        {
            var utf16 = new UnicodeEncoding();

            // Create a random number object seeded from the value
            // of the last random seed value. This is done
            // interlocked because it is a static value and we want
            // it to roll forward safely.

            var random = new Random(unchecked((int)DateTime.Now.Ticks));

            // Create an array of random values.

            var saltValue = new byte[SaltValueSize];

            random.NextBytes(saltValue);

            // Convert the salt value to a string. Note that the resulting string
            // will still be an array of binary values and not a printable string. 
            // Also it does not convert each byte to a double byte.

            string saltValueString = utf16.GetString(saltValue);

            // Return the salt value as a string.

            return saltValueString;
        }

        public static string HashPassword(string password, string saltValue)
        {
            HashAlgorithm hash = new SHA256CryptoServiceProvider();
            var encoding = new UnicodeEncoding();

            if (password != null)
            {
                // If the salt string is null or the length is invalid then
                // create a new valid salt value.

                if (saltValue == null)
                {
                    // Generate a salt string.
                    saltValue = GenerateSaltValue();
                }

                // Convert the salt string and the password string to a single
                // array of bytes. Note that the password string is Unicode and
                // therefore may or may not have a zero in every other byte.

                var binarySaltValue = new byte[SaltValueSize];

                binarySaltValue[0] = byte.Parse(saltValue.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[1] = byte.Parse(saltValue.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[2] = byte.Parse(saltValue.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[3] = byte.Parse(saltValue.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);

                var valueToHash = new byte[SaltValueSize + encoding.GetByteCount(password)];
                byte[] binaryPassword = encoding.GetBytes(password);

                // Copy the salt value and the password to the hash buffer.

                binarySaltValue.CopyTo(valueToHash, 0);
                binaryPassword.CopyTo(valueToHash, SaltValueSize);

                byte[] hashValue = hash.ComputeHash(valueToHash);

                // The hashed password is the salt plus the hash value (as a string).

                // Return the hashed password as a string.

                return hashValue.Aggregate(saltValue, (current, hexdigit) => current + hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat));
            }

            return null;
        }
    }
}