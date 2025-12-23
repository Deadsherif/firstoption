using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AuthService.Services
{
    public static class LocalAuthCache
    {
        public static string CallerAddinName { get; set; } = "Warning_Solver";

        private static readonly byte[] EncryptionKey = new byte[32]
        {
            0x55, 0x42, 0x1A, 0x9C, 0xD3, 0xF4, 0xA7, 0x18,
            0x32, 0x04, 0x5E, 0x6B, 0x9D, 0x0F, 0xC1, 0xB2,
            0x73, 0x8D, 0xAA, 0x19, 0x48, 0x02, 0xEF, 0xCE,
            0x67, 0x33, 0x91, 0x5B, 0xA4, 0x7E, 0xCD, 0x21
        };

        private static readonly byte[] InitializationVector = new byte[16]
        {
            0x12, 0x44, 0x3B, 0x88, 0x7E, 0x91, 0xAF, 0x04,
            0x59, 0x62, 0x37, 0x28, 0xC3, 0xDE, 0x50, 0x11
        };

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string CacheFolderGuid = "9F4E7C9b-9B06-4E9D-92BD-1FD0A3A2D7F8";
        private const string CacheFileName = "9F4E7C9b-9B06-4E9D-92BD-1FD0A3A2D7F8.BIN";

        private static string CachePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", CacheFolderGuid, CacheFileName);

        public static bool TryGetValidEntry()
        {
            try
            {
                var currentMac = SubscriptionService.GetCurrentMacAddress();
                if (string.IsNullOrWhiteSpace(currentMac))
                {
                    return false;
                }

                var cache = ReadCache();
                if (cache == null)
                {
                    return false;
                }

                var caller = GetCallerName();
                if (!cache.TryGetValue(caller, out var storedMac))
                {
                    return false;
                }

                return string.Equals(NormalizeMac(storedMac), NormalizeMac(currentMac), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static void UpsertCurrentMac()
        {
            try
            {
                var currentMac = SubscriptionService.GetCurrentMacAddress();
                if (string.IsNullOrWhiteSpace(currentMac))
                {
                    return;
                }

                var caller = GetCallerName();
                var cache = ReadCache() ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                cache[caller] = currentMac;
                WriteCache(cache);
            }
            catch
            {
                // Swallow errors to avoid blocking the user flow.
            }
        }

        private static Dictionary<string, string> ReadCache()
        {
            try
            {
                if (!File.Exists(CachePath))
                {
                    return null;
                }

                var encryptedBytes = File.ReadAllBytes(CachePath);
                var json = Decrypt(encryptedBytes);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private static void WriteCache(Dictionary<string, string> cache)
        {
            try
            {
                var directory = Path.GetDirectoryName(CachePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(cache, JsonOptions);
                var encryptedBytes = Encrypt(json);
                File.WriteAllBytes(CachePath, encryptedBytes);
            }
            catch
            {
                // Swallow errors to avoid blocking the user flow.
            }
        }

        private static byte[] Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = InitializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
                {
                    writer.Write(plainText);
                    writer.Flush();
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }

        private static string Decrypt(byte[] cipherBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = EncryptionKey;
                aes.IV = InitializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var memoryStream = new MemoryStream(cipherBytes))
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string GetCallerName()
        {
            return string.IsNullOrWhiteSpace(CallerAddinName) ? null : CallerAddinName;
        }

        private static string NormalizeMac(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(mac.Length);
            foreach (var c in mac)
            {
                if (c != ':' && c != '-' && c != ' ')
                {
                    builder.Append(char.ToUpperInvariant(c));
                }
            }
            return builder.ToString();
        }
    }
}

