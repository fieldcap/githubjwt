using System;

namespace GitHubJwt;

public class GitHubJwtFactoryOptions
{
    public String ClientId { get; set; }
    public Int32 AppIntegrationId { get; set; }
    public Int32 ExpirationSeconds { get; set; }
}