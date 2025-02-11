using System;

namespace GitHubJwt;

public interface IGitHubJwtFactory
{
    String CreateEncodedJwtToken(TimeSpan? iatOffset = null);
}