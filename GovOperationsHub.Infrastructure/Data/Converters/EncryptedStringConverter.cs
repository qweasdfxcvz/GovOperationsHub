using GovOperationsHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Infrastructure.Data.Converters
{
    public class EncryptedStringConverter : ValueConverter<string, string>
    {
        public EncryptedStringConverter(IEncryptionService encryptionService, ConverterMappingHints? mappingHints = null)
        : base(
            plainText => encryptionService.Encrypt(plainText),
            cipherText => encryptionService.Decrypt(cipherText),
            mappingHints)
        {
        }
    }
}
