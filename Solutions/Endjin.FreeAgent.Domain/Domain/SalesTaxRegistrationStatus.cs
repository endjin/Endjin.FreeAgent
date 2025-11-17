namespace Endjin.FreeAgent.Domain.Domain;

/// <summary>
/// Represents the sales tax registration status for a sales tax period.
/// </summary>
public enum SalesTaxRegistrationStatus
{
    /// <summary>
    /// Indicates the entity is not registered for sales tax.
    /// </summary>
    NotRegistered,

    /// <summary>
    /// Indicates the entity is registered for sales tax.
    /// </summary>
    Registered
}