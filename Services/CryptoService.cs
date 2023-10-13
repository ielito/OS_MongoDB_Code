using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MongoDB_Code.Services
{
    public class CryptoService
        {
            public static bool IsBase64String(string s)
            {
                s = s.Trim();
                return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            }

            public static string EncryptString(string text, string keyString)
            {
                var key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(keyString));

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;

                    using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }

                            var iv = aesAlg.IV;
                            var decryptedContent = msEncrypt.ToArray();
                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }

            public static string DecryptString(string cipherText, string keyString)
            {
                try
                {
                    if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(keyString))
                    {
                        throw new ArgumentException("CipherText and KeyString must not be null or empty.");
                    }

                    if (!IsBase64String(cipherText))
                    {
                        throw new FormatException("The cipherText is not a valid Base64 string.");
                    }

                    var fullCipher = Convert.FromBase64String(cipherText);

                    if (fullCipher.Length <= 16)
                    {
                        throw new ArgumentException("The cipherText does not contain enough data to extract the IV.");
                    }

                    var iv = new byte[16];
                    var cipher = new byte[fullCipher.Length - iv.Length];

                    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                    var key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(keyString));

                    using (var aesAlg = Aes.Create())
                    {
                        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                        {
                            using (var msDecrypt = new MemoryStream(cipher))
                            {
                                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    using (var srDecrypt = new StreamReader(csDecrypt))
                                    {
                                        return srDecrypt.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (OverflowException ex)
                {
                    throw new Exception("OverflowException during decryption. Ensure that the cipherText and keyString are valid and not causing arithmetic overflows.", ex);
                }
                catch (CryptographicException ex)
                {
                    throw new Exception("CryptographicException during decryption. Ensure that the cipherText and keyString are valid and decryptable.", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred during decryption.", ex);
                }
            }

        }
    }