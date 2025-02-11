using System;
using System.IO;

namespace GitHubJwt;

public class EnvironmentVariablePrivateKeySource : IPrivateKeySource
{
    private readonly String _environmentVariableName;

    public EnvironmentVariablePrivateKeySource(String environmentVariableName)
    {
        if (String.IsNullOrEmpty(environmentVariableName))
        {
            throw new ArgumentNullException(nameof(environmentVariableName));
        }

        _environmentVariableName = environmentVariableName;
    }

    public TextReader GetPrivateKeyReader()
    {
        var privateKeyPem = Environment.GetEnvironmentVariable(_environmentVariableName).HydrateRsaVariable();
        return new StringReader(privateKeyPem);
    }

}