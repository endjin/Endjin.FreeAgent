// <copyright file="VatReturn.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record VatReturn
{
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    [JsonPropertyName("period_starts_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PeriodStartsOn { get; init; }

    [JsonPropertyName("period_ends_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? PeriodEndsOn { get; init; }

    [JsonPropertyName("frequency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Frequency { get; init; }

    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    [JsonPropertyName("box1_vat_due_on_sales")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box1VatDueOnSales { get; init; }

    [JsonPropertyName("box2_vat_due_on_acquisitions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box2VatDueOnAcquisitions { get; init; }

    [JsonPropertyName("box3_total_vat_due")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box3TotalVatDue { get; init; }

    [JsonPropertyName("box4_vat_reclaimed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box4VatReclaimed { get; init; }

    [JsonPropertyName("box5_net_vat_due")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box5NetVatDue { get; init; }

    [JsonPropertyName("box6_total_sales_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box6TotalSalesExVat { get; init; }

    [JsonPropertyName("box7_total_purchases_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box7TotalPurchasesExVat { get; init; }

    [JsonPropertyName("box8_total_supplies_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box8TotalSuppliesExVat { get; init; }

    [JsonPropertyName("box9_total_acquisitions_ex_vat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Box9TotalAcquisitionsExVat { get; init; }

    [JsonPropertyName("filed_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? FiledOn { get; init; }

    [JsonPropertyName("filed_online")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? FiledOnline { get; init; }

    [JsonPropertyName("hmrc_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HmrcReference { get; init; }
}