// <copyright file="VatBasis.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines the valid VAT accounting basis options for UK companies.
/// </summary>
/// <remarks>
/// <para>
/// The VAT basis determines when VAT is due to be paid to HMRC.
/// This setting affects how VAT returns are calculated and when VAT becomes payable.
/// </para>
/// </remarks>
public static class VatBasis
{
    /// <summary>
    /// Gets the invoice basis (accrual basis) where VAT is due when invoices are issued/received.
    /// </summary>
    /// <remarks>
    /// Under invoice accounting, VAT is charged on sales when invoices are issued and
    /// reclaimed on purchases when invoices are received, regardless of payment status.
    /// </remarks>
    public const string Invoice = "Invoice";

    /// <summary>
    /// Gets the cash basis where VAT is due when payments are made/received.
    /// </summary>
    /// <remarks>
    /// Under cash accounting, VAT is only charged on sales when payment is received and
    /// reclaimed on purchases when payment is made. This can help with cash flow for smaller businesses.
    /// </remarks>
    public const string Cash = "Cash";

    /// <summary>
    /// Determines whether the specified value is a valid VAT basis.
    /// </summary>
    /// <param name="value">The VAT basis value to validate.</param>
    /// <returns><see langword="true"/> if the value is a valid VAT basis; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(string? value)
    {
        return value switch
        {
            Invoice => true,
            Cash => true,
            _ => false,
        };
    }

    /// <summary>
    /// Gets all valid VAT basis values.
    /// </summary>
    /// <returns>An array containing all valid VAT basis strings.</returns>
    public static string[] GetValidValues()
    {
        return new[]
        {
            Invoice,
            Cash,
        };
    }
}