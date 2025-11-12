// <copyright file="PayrollPayment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollPayment
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("gross_pay")]
    public decimal? GrossPay { get; init; }

    [JsonPropertyName("income_tax")]
    public decimal? IncomeTax { get; init; }

    [JsonPropertyName("employee_ni")]
    public decimal? EmployeeNi { get; init; }

    [JsonPropertyName("employer_ni")]
    public decimal? EmployerNi { get; init; }

    [JsonPropertyName("employee_pension")]
    public decimal? EmployeePension { get; init; }

    [JsonPropertyName("employer_pension")]
    public decimal? EmployerPension { get; init; }

    [JsonPropertyName("student_loan")]
    public decimal? StudentLoan { get; init; }

    [JsonPropertyName("net_pay")]
    public decimal? NetPay { get; init; }

    [JsonPropertyName("total_cost")]
    public decimal? TotalCost { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}