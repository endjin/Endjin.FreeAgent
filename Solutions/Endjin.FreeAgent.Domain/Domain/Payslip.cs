// <copyright file="Payslip.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record Payslip
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("gross_salary")]
    public decimal? GrossSalary { get; init; }

    [JsonPropertyName("net_salary")]
    public decimal? NetSalary { get; init; }

    [JsonPropertyName("income_tax")]
    public decimal? IncomeTax { get; init; }

    [JsonPropertyName("employee_nic")]
    public decimal? EmployeeNic { get; init; }

    [JsonPropertyName("employee_pension")]
    public decimal? EmployeePension { get; init; }

    [JsonPropertyName("employer_nic")]
    public decimal? EmployerNic { get; init; }

    [JsonPropertyName("employer_pension")]
    public decimal? EmployerPension { get; init; }

    [JsonPropertyName("student_loan")]
    public decimal? StudentLoan { get; init; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }
}