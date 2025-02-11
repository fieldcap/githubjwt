using System;
using System.IO;

namespace GitHubJwt;

public class StringPrivateKeySource : IPrivateKeySource
{
    protected readonly String Key;

    public StringPrivateKeySource(String key)
    {
        if (String.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        Key = key;
    }

    public TextReader GetPrivateKeyReader()
    {
        return new StringReader(Key.HydrateRsaVariable());
    }
}