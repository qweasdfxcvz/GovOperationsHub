using Microsoft.Extensions.Configuration;
using GovOperationsHub.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GovOperationsHub.Infrastructure.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionService(IConfiguration configuration)
        {
            // مفتاح تشفير بطول 32 بايت (256-bit) و IV بطول 16 بايت
            var secretKey = configuration["SecuritySettings:EncryptionKey"]
                ?? "GovOperationsHub_Syria_SecretKey_2026_Key32!";
            var secretIv = configuration["SecuritySettings:EncryptionIV"]
                ?? "GovHub_IV_16Byte";

            _key = Encoding.UTF8.GetBytes(secretKey.PadRight(32).Substring(0, 32));
            _iv = Encoding.UTF8.GetBytes(secretIv.PadRight(16).Substring(0, 16));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                var buffer = Convert.FromBase64String(cipherText);
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(buffer);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch
            {
                // في حال كانت القيمة غير مشفرة مسبقاً أو حدث خطأ فك التشفير
                return cipherText;
            }
        }
    }
}
