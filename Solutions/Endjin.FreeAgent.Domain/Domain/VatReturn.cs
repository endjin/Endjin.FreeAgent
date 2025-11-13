// <copyright file="VatReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a VAT (Value Added Tax) return in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// VAT returns are periodic submissions to HM Revenue &amp; Customs (HMRC) reporting the VAT collected on sales
/// and the VAT paid on purchases. UK VAT-registered businesses must file returns quarterly, monthly, or annually
/// depending on their registration type.
/// </para>
/// <para>
/// The VAT return contains the standard nine boxes required by HMRC:
/// - Box 1: VAT due on sales and other outputs
/// - Box 2: VAT due on acquisitions from other EU member states
/// - Box 3: Total VAT due (sum of Box 1 and Box 2)
/// - Box 4: VAT reclaimed on purchases and other inputs
/// - Box 5: Net VAT to pay or reclaim (difference between Box 3 and Box 4)
/// - Box 6: Total value of sales and outputs excluding VAT
/// - Box 7: Total value of purchases and inputs excluding VAT
/// - Box 8: Total value of supplies to other EU member states excluding VAT
/// - Box 9: Total value of acquisitions from other EU member states excluding VAT
/// </para>
/// <para>
/// Returns can be filed electronically through FreeAgent's integration with HMRC's Making Tax Digital (MTD) service
/// or manually marked as filed if submitted through other means.
/// </para>
/// <para>
/// API Endpoint: /v2/vat_returns
/// </para>
/// <para>
/// Minimum Access Level: Full Access
/// </para>
/// </remarks>
/// <seealso cref="VatReturnFiling"/>
/// <seealso cref="Invoice"/>
/// <seealso cref="Bill"/>
public record VatReturn
{
    /// <summary>
    /// Gets the unique URI identifier for this VAT return.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this VAT return in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the start date of the VAT return period.
    /// </summary>
    /// <value>
    /// The first date of the accounting period covered by this VAT return.
    /// </value>
    [JsonPropertyName("period_starts_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PeriodStartsOn { get; init; }

    /// <summary>
    /// Gets the end date of the VAT return period.
    /// </summary>
    /// <value>
    /// The last date of the accounting period covered by this VAT return.
    /// </value>
    [JsonPropertyName("period_ends_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PeriodEndsOn { get; init; }

    /// <summary>
    /// Gets the filing frequency for VAT returns.
    /// </summary>
    /// <value>
    /// The frequency of VAT return submissions, such as "Quarterly", "Monthly", or "Annually".
    /// </value>
    [JsonPropertyName("frequency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Frequency { get; init; }

    /// <summary>
    /// Gets the status of this VAT return.
    /// </summary>
    /// <value>
    /// The current status, such as "Draft", "Filed", or "Overdue".
    /// </value>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Gets the VAT due on sales and other outputs (Box 1).
    /// </summary>
    /// <value>
    /// The total VAT charged on sales, services, and other taxable outputs during the period.
    /// This is the VAT you have collected from customers.
    /// </value>
    [JsonPropertyName("box1_vat_due_on_sales")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box1VatDueOnSales { get; init; }

    /// <summary>
    /// Gets the VAT due on acquisitions from other EU member states (Box 2).
    /// </summary>
    /// <value>
    /// The VAT due on goods or services acquired from other EU countries under the reverse charge mechanism.
    /// Typically zero for most UK businesses post-Brexit.
    /// </value>
    [JsonPropertyName("box2_vat_due_on_acquisitions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box2VatDueOnAcquisitions { get; init; }

    /// <summary>
    /// Gets the total VAT due (Box 3).
    /// </summary>
    /// <value>
    /// The sum of Box 1 and Box 2, representing the total VAT owed to HMRC before any reclaims.
    /// </value>
    [JsonPropertyName("box3_total_vat_due")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box3TotalVatDue { get; init; }

    /// <summary>
    /// Gets the VAT reclaimed on purchases and other inputs (Box 4).
    /// </summary>
    /// <value>
    /// The total VAT paid on business purchases and expenses that can be reclaimed from HMRC.
    /// This is the VAT you have paid to suppliers.
    /// </value>
    [JsonPropertyName("box4_vat_reclaimed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box4VatReclaimed { get; init; }

    /// <summary>
    /// Gets the net VAT to pay to or reclaim from HMRC (Box 5).
    /// </summary>
    /// <value>
    /// The difference between Box 3 and Box 4 (Total VAT Due - VAT Reclaimed).
    /// A positive value means VAT is owed to HMRC; a negative value means a VAT refund is due from HMRC.
    /// </value>
    [JsonPropertyName("box5_net_vat_due")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box5NetVatDue { get; init; }

    /// <summary>
    /// Gets the total value of sales and outputs excluding VAT (Box 6).
    /// </summary>
    /// <value>
    /// The net value of all sales and outputs before VAT is added. This figure is used by HMRC
    /// for statistical purposes and to verify the VAT calculations.
    /// </value>
    [JsonPropertyName("box6_total_sales_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box6TotalSalesExVat { get; init; }

    /// <summary>
    /// Gets the total value of purchases and inputs excluding VAT (Box 7).
    /// </summary>
    /// <value>
    /// The net value of all purchases and expenses before VAT. This figure is used by HMRC
    /// for statistical purposes and to verify the VAT calculations.
    /// </value>
    [JsonPropertyName("box7_total_purchases_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box7TotalPurchasesExVat { get; init; }

    /// <summary>
    /// Gets the total value of supplies to other EU member states excluding VAT (Box 8).
    /// </summary>
    /// <value>
    /// The net value of goods and services supplied to other EU countries, typically zero-rated.
    /// This figure is typically zero for most UK businesses post-Brexit.
    /// </value>
    [JsonPropertyName("box8_total_supplies_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box8TotalSuppliesExVat { get; init; }

    /// <summary>
    /// Gets the total value of acquisitions from other EU member states excluding VAT (Box 9).
    /// </summary>
    /// <value>
    /// The net value of goods and services acquired from other EU countries.
    /// This figure is typically zero for most UK businesses post-Brexit.
    /// </value>
    [JsonPropertyName("box9_total_acquisitions_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box9TotalAcquisitionsExVat { get; init; }

    /// <summary>
    /// Gets the date when this VAT return was filed.
    /// </summary>
    /// <value>
    /// The date the return was submitted to HMRC, either electronically or manually.
    /// </value>
    [JsonPropertyName("filed_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? FiledOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether this VAT return was filed online through Making Tax Digital.
    /// </summary>
    /// <value>
    /// <c>true</c> if the return was submitted electronically through FreeAgent's MTD integration;
    /// <c>false</c> if it was marked as manually filed through other means.
    /// </value>
    [JsonPropertyName("filed_online")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? FiledOnline { get; init; }

    /// <summary>
    /// Gets the HMRC reference number for this filed VAT return.
    /// </summary>
    /// <value>
    /// The unique reference number provided by HMRC upon successful submission, used for tracking and confirmation.
    /// </value>
    [JsonPropertyName("hmrc_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HmrcReference { get; init; }
}