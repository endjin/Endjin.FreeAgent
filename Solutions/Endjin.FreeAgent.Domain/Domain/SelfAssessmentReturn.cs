// <copyright file="SelfAssessmentReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SelfAssessmentReturn
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("period_starts_on")]
    public DateOnly? PeriodStartsOn { get; init; }

    [JsonPropertyName("period_ends_on")]
    public DateOnly? PeriodEndsOn { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("income_tax_due")]
    public decimal? IncomeTaxDue { get; init; }

    [JsonPropertyName("national_insurance_due")]
    public decimal? NationalInsuranceDue { get; init; }

    [JsonPropertyName("capital_gains_tax_due")]
    public decimal? CapitalGainsTaxDue { get; init; }

    [JsonPropertyName("student_loan_repayment_due")]
    public decimal? StudentLoanRepaymentDue { get; init; }

    [JsonPropertyName("total_tax_due")]
    public decimal? TotalTaxDue { get; init; }

    [JsonPropertyName("payments_on_account_due")]
    public decimal? PaymentsOnAccountDue { get; init; }

    [JsonPropertyName("filed_on")]
    public DateOnly? FiledOn { get; init; }

    [JsonPropertyName("filed_online")]
    public bool? FiledOnline { get; init; }

    [JsonPropertyName("utr_number")]
    public string? UtrNumber { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}