﻿using System.Text;
using NServiceBus.Encryption.MessageProperty;

#region ConfigureEncryption
using NServiceBus;

public static class EncryptionExtensions
{
    public static void ConfigurationEncryption(this EndpointConfiguration endpointConfiguration)
    {
        var encryptionService = new RijndaelEncryptionService(
            encryptionKeyIdentifier: "2015-10",
        key: Encoding.ASCII.GetBytes("gdDbqRpqdRbTs3mhdZh9qCaDaxJXl+e6"));
        endpointConfiguration.EnableMessagePropertyEncryption(encryptionService,
            encryptedPropertyConvention: info =>
                        {
                            return info.Name.StartsWith("Encrypted");
                        });
    }
}

#endregion