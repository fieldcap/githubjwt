using System;
using System.IO;

namespace GitHubJwt;

public class FilePrivateKeySource(String filePath) : IPrivateKeySource
{
    public TextReader GetPrivateKeyReader()
    {
        return new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }
}