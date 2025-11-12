// <copyright file="FreeAgentOptionsValidator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Validator for FreeAgentOptions to ensure they are valid at startup.
/// </summary>
public class FreeAgentOptionsValidator : IValidateOptions<FreeAgentOptions>
{
    public ValidateOptionsResult Validate(string? name, FreeAgentOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("FreeAgentOptions cannot be null");
        }

        try
        {
            options.Validate();

            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail(ex.Message);
        }
    }
}