// <copyright file="ContactBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests.Builders;

using System;

public class ContactBuilder
{
    private Uri? url = new("https://api.freeagent.com/v2/contacts/1");
    private string? firstName = "John";
    private string? lastName = "Doe";
    private string? organisationName = "Acme Corp";
    private string? email = "john.doe@acme.com";
    private string? billingEmail = null;
    private string? phoneNumber = "555-0100";
    private string? mobile = null;
    private string? address1 = "123 Main St";
    private string? address2 = null;
    private string? address3 = null;
    private string? town = "Springfield";
    private string? region = "IL";
    private string? postcode = "62701";
    private string? country = "USA";
    private bool contactNameOnInvoices = false;
    private string? locale = "en";
    private bool usesContactInvoiceSequence = false;
    private string? chargeSalesTax = "Auto";
    private string? salesTaxRegistrationNumber = null;
    private int? activeProjectsCount = 0;
    private decimal? accountBalance = null;
    private string? status = "Active";
    private DateTimeOffset createdAt = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset updatedAt = new(2024, 1, 31, 0, 0, 0, TimeSpan.Zero);
    private string? directDebitMandateState = null;
    private int? defaultPaymentTermsInDays = null;
    private bool? isCisSubcontractor = null;
    private string? cisDeductionRate = null;
    private string? uniqueTaxReference = null;
    private string? subcontractorVerificationNumber = null;

    public ContactBuilder WithUrl(Uri? url)
    {
        this.url = url;
        return this;
    }

    public ContactBuilder WithFirstName(string? firstName)
    {
        this.firstName = firstName;
        return this;
    }

    public ContactBuilder WithLastName(string? lastName)
    {
        this.lastName = lastName;
        return this;
    }

    public ContactBuilder WithOrganisationName(string? organisationName)
    {
        this.organisationName = organisationName;
        return this;
    }

    public ContactBuilder WithEmail(string? email)
    {
        this.email = email;
        return this;
    }

    public ContactBuilder WithPhoneNumber(string? phoneNumber)
    {
        this.phoneNumber = phoneNumber;
        return this;
    }

    public ContactBuilder WithAddress(string? address1, string? town = null, string? postcode = null)
    {
        this.address1 = address1;
        if (town != null)
        {
            this.town = town;
        }

        if (postcode != null)
        {
            this.postcode = postcode;
        }

        return this;
    }

    public ContactBuilder WithActiveProjects(int count)
    {
        this.activeProjectsCount = count;
        return this;
    }

    public ContactBuilder WithAccountBalance(decimal? accountBalance)
    {
        this.accountBalance = accountBalance;
        return this;
    }

    public ContactBuilder WithStatus(string? status)
    {
        this.status = status;
        return this;
    }

    public ContactBuilder AsInactive()
    {
        this.status = "Hidden";
        return this;
    }

    public ContactBuilder WithDirectDebitMandateState(string? directDebitMandateState)
    {
        this.directDebitMandateState = directDebitMandateState;
        return this;
    }

    public ContactBuilder WithDefaultPaymentTermsInDays(int? defaultPaymentTermsInDays)
    {
        this.defaultPaymentTermsInDays = defaultPaymentTermsInDays;
        return this;
    }

    public ContactBuilder AsCisSubcontractor(bool isCisSubcontractor = true)
    {
        this.isCisSubcontractor = isCisSubcontractor;
        return this;
    }

    public ContactBuilder WithCisDeductionRate(string? cisDeductionRate)
    {
        this.cisDeductionRate = cisDeductionRate;
        return this;
    }

    public ContactBuilder WithUniqueTaxReference(string? uniqueTaxReference)
    {
        this.uniqueTaxReference = uniqueTaxReference;
        return this;
    }

    public ContactBuilder WithSubcontractorVerificationNumber(string? subcontractorVerificationNumber)
    {
        this.subcontractorVerificationNumber = subcontractorVerificationNumber;
        return this;
    }

    public ContactBuilder WithCisDetails(string? deductionRate = "cis_standard", string? utr = "1234567890", string? verificationNumber = "V1234567890")
    {
        this.isCisSubcontractor = true;
        this.cisDeductionRate = deductionRate;
        this.uniqueTaxReference = utr;
        this.subcontractorVerificationNumber = verificationNumber;
        return this;
    }

    public Contact Build() => new()
    {
        Url = url,
        FirstName = firstName,
        LastName = lastName,
        OrganisationName = organisationName,
        Email = email,
        BillingEmail = billingEmail,
        PhoneNumber = phoneNumber,
        Mobile = mobile,
        Address1 = address1,
        Address2 = address2,
        Address3 = address3,
        Town = town,
        Region = region,
        Postcode = postcode,
        Country = country,
        ContactNameOnInvoices = contactNameOnInvoices,
        Locale = locale,
        UsesContactInvoiceSequence = usesContactInvoiceSequence,
        ChargeSalesTax = chargeSalesTax,
        SalesTaxRegistrationNumber = salesTaxRegistrationNumber,
        ActiveProjectsCount = activeProjectsCount,
        AccountBalance = accountBalance,
        Status = status,
        CreatedAt = createdAt,
        UpdatedAt = updatedAt,
        DirectDebitMandateState = directDebitMandateState,
        DefaultPaymentTermsInDays = defaultPaymentTermsInDays,
        IsCisSubcontractor = isCisSubcontractor,
        CisDeductionRate = cisDeductionRate,
        UniqueTaxReference = uniqueTaxReference,
        SubcontractorVerificationNumber = subcontractorVerificationNumber
    };

    public static implicit operator Contact(ContactBuilder builder) => builder.Build();
}
