// <copyright file="PayrollProfile.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollProfile
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("employee_id")]
    public string? EmployeeId { get; init; }

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; init; }

    [JsonPropertyName("ni_letter")]
    public string? NiLetter { get; init; }

    [JsonPropertyName("annual_salary")]
    public decimal? AnnualSalary { get; init; }

    [JsonPropertyName("pension_scheme")]
    public string? PensionScheme { get; init; }

    [JsonPropertyName("pension_contribution_percentage")]
    public decimal? PensionContributionPercentage { get; init; }

    [JsonPropertyName("student_loan_type")]
    public string? StudentLoanType { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}