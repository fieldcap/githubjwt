using System;
using System.Collections.Generic;

namespace GitHubJwt.Tests;

public class EnvironmentVariableScope : IDisposable
{
    public Dictionary<String, String> OriginalValues { get; }

    public EnvironmentVariableScope(params String[] environmentVariableNames)
    {
        OriginalValues = new();
        foreach (var environmentVariableName in environmentVariableNames)
        {
            BackupEnvironmentVariable(environmentVariableName);
        }
    }

    public void UpdateEnvironmentVariable(String name, String value)
    {
        if (!OriginalValues.ContainsKey(name))
        {
            BackupEnvironmentVariable(name);
        }

        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose()
    {
        foreach (var kvp in OriginalValues)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }
    }

    private void BackupEnvironmentVariable(String name)
    {
        OriginalValues.Add(
            name,
            Environment.GetEnvironmentVariable(name));
    }
}