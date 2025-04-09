using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;

namespace GitHubJwt;

public class GitHubJwtFactory(IPrivateKeySource privateKeySource, GitHubJwtFactoryOptions options) : IGitHubJwtFactory
{
    private static readonly Int64 TicksSince197011 = new DateTime(1970, 1, 1).Ticks;

    public String CreateEncodedJwtToken(TimeSpan? iatOffset = null)
    {
        var utcNow = DateTime.UtcNow.Add(iatOffset ?? TimeSpan.Zero);

        var payload = new Dictionary<String, Object>
        {
            {"iat", ToUtcSeconds(utcNow)},
            {"exp", ToUtcSeconds(utcNow.AddSeconds(options.ExpirationSeconds))},
            {"iss", (Object)options.ClientId ?? options.AppIntegrationId}
        };

        // Generate JWT
        using (var rsa = new RSACryptoServiceProvider())
        {
            var rsaParams = ToRsaParameters(GetPrivateKey());
            rsa.ImportParameters(rsaParams);
            return JWT.Encode(payload, rsa, JwsAlgorithm.RS256);
        }
    }

    private RsaPrivateCrtKeyParameters GetPrivateKey()
    {
        using (var privateKeyReader = privateKeySource.GetPrivateKeyReader())
        {
            var pemReader = new PemReader(privateKeyReader);
            var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            return (RsaPrivateCrtKeyParameters)keyPair.Private;
        }
    }

    private static RSAParameters ToRsaParameters(RsaPrivateCrtKeyParameters privKey)
    {
        var rp = new RSAParameters
        {
            Modulus = privKey.Modulus.ToByteArrayUnsigned(),
            Exponent = privKey.PublicExponent.ToByteArrayUnsigned(),
            P = privKey.P.ToByteArrayUnsigned(),
            Q = privKey.Q.ToByteArrayUnsigned()
        };

        rp.D = ConvertRsaParametersField(privKey.Exponent, rp.Modulus.Length);
        rp.DP = ConvertRsaParametersField(privKey.DP, rp.P.Length);
        rp.DQ = ConvertRsaParametersField(privKey.DQ, rp.Q.Length);
        rp.InverseQ = ConvertRsaParametersField(privKey.QInv, rp.Q.Length);
        return rp;
    }

    private static Byte[] ConvertRsaParametersField(BigInteger n, Int32 size)
    {
        var bs = n.ToByteArrayUnsigned();

        if (bs.Length == size)
        {
            return bs;
        }

        if (bs.Length > size)
        {
            throw new ArgumentException("Specified size too small", nameof(size));
        }

        var padded = new Byte[size];
        Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
        return padded;
    }

    private static Int64 ToUtcSeconds(DateTime dt)
    {
        return (dt.ToUniversalTime().Ticks - TicksSince197011) / TimeSpan.TicksPerSecond;
    }
}