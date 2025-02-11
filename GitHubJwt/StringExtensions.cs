using System;
using System.Text;

namespace GitHubJwt;

internal static class StringExtensions
{
    private const String BeginRsaPrivateKey = "-----BEGIN RSA PRIVATE KEY-----";
    private const String EndRsaPrivateKey = "-----END RSA PRIVATE KEY-----";

    public static String HydrateRsaVariable(this String input)
    {
        var stringBuilder = new StringBuilder();
        if(!input.StartsWith(BeginRsaPrivateKey))
        {
            stringBuilder.AppendLine(BeginRsaPrivateKey);
        }

        stringBuilder.AppendLine(input);

        if(!input.EndsWith(EndRsaPrivateKey))
        {
            stringBuilder.AppendLine(EndRsaPrivateKey);
        }

        return stringBuilder.ToString();
    }
}