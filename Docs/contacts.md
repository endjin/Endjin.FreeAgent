# Contacts

*Minimum access level*: `Time`, unless stated otherwise.

## Attributes

| Required | Attribute                         | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   | Kind      |
| -------- | --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                               | The unique identifier for the contact                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         | URI       |
| ?        | first_name                        | First name Required if `organisation_name` is not specified                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   | String    |
| ?        | last_name                         | Last name Required if `organisation_name` is not specified                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | String    |
| ?        | organisation_name                 | Organisation name Required if `first_name` and `last_name` are not specified                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | String    |
|          | active_projects_count             | Numer of active [projects](projects.md) belonging to the contact                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | String    |
|          | direct_debit_mandate_state        | Included if the contact has a current pre-authorised GoCardless Direct Debit agreement. `setup`: the agreement has been created in the FreeAgent UI and the email is to be sent to the customer, `pending`: the customer has been sent the mandate email, `inactive`: the customer has completed the mandate agreement and we are waiting on confirmation from GoCardless, `active`: GoCardless have confirmed the mandate and it is now usable, `failed`: agreement failed, the customer's details were rejected by GoCardless (or the Bank) as invalid      | String    |
|          | created_at                        | Creation of the contact resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Timestamp |
|          | updated_at                        | When the contact resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | Timestamp |
|          | email                             | Email Max 2 addresses can be entered during free trial                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        | String    |
|          | billing_email                     | Billing Email Max 2 addresses can be entered during free trial                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |           |
|          | phone_number                      | Telephone                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | String    |
|          | mobile                            | Mobile number                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 | String    |
|          | address1                          | First line of the address                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | String    |
|          | address2                          | Second line of the address                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | String    |
|          | address3                          | Third line of the address                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | String    |
|          | town                              | Town                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          | String    |
|          | region                            | Region or State                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               | String    |
|          | postcode                          | Post / Zip Code                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               | String    |
|          | country                           | Country                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | String    |
|          | uses_contact_invoice_sequence     | `true` if using contact-level invoice sequence, `false` otherwise Can be overridden by project-level sequences                                                                                                                                                                                                                                                                                                                                                                                                                                                | Boolean   |
|          | contact_name_on_invoices          | `true` if invoices should show the contact name as well as an organization name, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                                                                            | Boolean   |
|          | charge_sales_tax                  | One of the following: `Auto` (default), `Always`, `Never`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     | String    |
|          | sales_tax_registration_number     | If applicable, and needing to be displayed on invoices. Also required if EC Sales of Goods or Services to this contact need to be reported.                                                                                                                                                                                                                                                                                                                                                                                                                   | String    |
|          | status                            | One of the following: `Active`, `Hidden`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | String    |
|          | default_payment_terms_in_days     | Default payment terms in days                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 | Integer   |
|          | locale                            | Invoice / Estimate language. One of the following: `bg`: Bulgarian, `ca`: Catalan, `cy`: Welsh, `cz`: Czech, `de`: German, `dk`: Danish, `en`: English, `en-US`: English (United States), `es`: Spanish, `et`: Estonian, `fi`: Finnish, `fr`: French, `fr-BE`: French (Belgium), `fr-CA`: French (Canada), `is`: Icelandic, `it`: Italian, `lv-LV`: Latvian, `nl`: Dutch, `nl-BE`: Dutch (Belgium), `nk`: Norwegian, `pl-PL`: Polish, `pt-BR`: Brazilian Portuguese, `ro`: Romanian, `rs`: Serbian, `ru`: Russian, `se`: Swedish, `sk`: Slovak, `tr`: Turkish | String    |
|          | is_cis_subcontractor              | `true` if contact is a CIS subcontractor, `false` otherwise This attribute will not be present if CIS for Contractors is not enabled for the company                                                                                                                                                                                                                                                                                                                                                                                                          | Boolean   |
| ?        | cis_deduction_rate                | The current CIS band of the contact, specified as the name of a [Construction Industry Scheme band](cis_bands.md). One of the following: `cis_gross`, `cis_standard`, `cis_higher` This attribute will not be present if CIS for Contractors is not enabled for the company. Required if `is_cis_subcontractor` is `true`                                                                                                                                                                                                                                     | String    |
|          | unique_tax_reference              | The Unique Tax Reference of the contact. Should be 10 numbers without spaces This attribute will not be present if CIS for Contractors is not enabled for the company                                                                                                                                                                                                                                                                                                                                                                                         | String    |
|          | subcontractor_verification_number | The Subcontractor Verification Number of the contact, received from HMRC. Should begin with V, then 10 numbers, then optionally up to 2 letters This attribute will not be present if CIS for Contractors is not enabled for the company or it is blank                                                                                                                                                                                                                                                                                                       | String    |

## List all contacts

```http
GET https://api.freeagent.com/v2/contacts
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/contacts?view=active
```

- `all`: Show all contacts.
- `active`: Show only active contacts (default).
- `clients`: Show all clients.
- `suppliers`: Show only active suppliers.
- `active_projects`: Show only clients with active projects.
- `completed_projects`: Show only clients with completed invoices.
- `open_clients`: Show only clients with open invoices.
- `open_suppliers`: Show only suppliers with open bills.
- `hidden`: Show only hidden contacts.

#### Sort Orders

```http
GET https://api.freeagent.com/v2/contacts?sort=updated_at
```

- `name`: Sort by the concatenation of `organisation_name`, `last_name` and `first_name` (default).
- `created_at`: Sort by the time the contact was created.
- `updated_at`: Sort by the time the contact was last modified.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/contacts?sort=-updated_at
```

#### Date Filters

```http
GET https://api.freeagent.com/v2/contacts?updated_since=2025-03-15T09:00:00.000Z
```

- `updated_since`: Only return contacts that have been updated since the provided date and time (format ISO 8601).

### Response

```http
Status: 200 OK
```

```json
{ "contacts":[
  {
    "url":"https://api.freeagent.com/v2/contacts/2",
    "first_name":"test",
    "last_name":"me",
    "organisation_name":"Acme Ltd",
    "email":"test@example.com",
    "billing_email":"billing@example.com",
    "phone_number":"12345678",
    "mobile":"9876543210",
    "address1":"11 George Street",
    "address2": "South Court",
    "address3": "Flat 6",
    "town":"London",
    "region": "Southwark",
    "postcode":"SE1 6HA",
    "country":"United Kingdom",
    "contact_name_on_invoices":true,
    "default_payment_terms_in_days":30,
    "locale":"en",
    "account_balance":"-100.0",
    "uses_contact_invoice_sequence":false,
    "charge_sales_tax":"Auto",
    "sales_tax_registration_number":"ST12345",
    "active_projects_count":1,
    "direct_debit_mandate_state":"active",
    "status":"Active",
    "is_cis_subcontractor":true,
    "cis_deduction_rate":"cis_standard",
    "unique_tax_reference":"1234567890",
    "subcontractor_verification_number":"V1234567890",
    "created_at":"2011-09-14T16:00:41Z",
    "updated_at":"2011-09-16T09:34:41Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <contacts type="array">
    <contact>
      <url>https://api.freeagent.com/v2/contacts/2</url>
      <first-name>test</first-name>
      <last-name>me</last-name>
      <organisation-name>Acme Ltd</organisation-name>
      <email>test@example.com</email>
      <billing-email>billing@example.com</billing-email>
      <phone-number>12345678</phone-number>
      <mobile>9876543210</mobile>
      <address1>11 George Street</address1>
      <address2>South Court</address2>
      <address3>Flat 6</address3>
      <town>London</town>
      <region>Southwark</region>
      <postcode>SE1 6HA</postcode>
      <country>United Kingdom</country>
      <contact-name-on-invoices type="boolean">true</contact-name-on-invoices>
      <default-payment-terms-in-days>30</default-payment-terms-in-days>
      <locale>en</locale>
      <account-balance type="decimal">-100.0</account-balance>
      <uses-contact-invoice-sequence type="boolean">false</uses-contact-invoice-sequence>
      <charge-sales-tax>Auto</charge-sales-tax>
      <sales-tax-registration-number>ST12345</sales-tax-registration-number>
      <active-projects-count type="integer">1</active-projects-count>
      <direct-debit-mandate-state>active</direct-debit-mandate-state>
      <status>Active</status>
      <is-cis-subcontractor type="boolean">true</is-cis-subcontractor>
      <cis-deduction-rate>cis_standard</cis-deduction-rate>
      <unique-tax-reference>1234567890</unique-tax-reference>
      <subcontractor-verification-number>V1234567890</subcontractor-verification-number>
      <created-at type="datetime">2011-09-14T16:00:41Z</created-at>
      <updated-at type="datetime">2011-09-16T09:34:41Z</updated-at>
    </contact>
  </contacts>
</freeagent>
```
Show as JSON

## Get a single contact

```http
GET https://api.freeagent.com/v2/contacts/:id
```

### Response

```http
Status: 200 OK
```

```json
{"contact":
  {
    "url":"https://api.freeagent.com/v2/contacts/2",
    "first_name":"test",
    "last_name":"me",
    "organisation_name":"Acme Ltd",
    "email":"test@example.com",
    "billing_email":"billing@example.com",
    "phone_number":"12345678",
    "mobile":"9876543210",
    "address1":"11 George Street",
    "address2": "South Court",
    "address3": "Flat 6",
    "town":"London",
    "region": "Southwark",
    "postcode":"SE1 6HA",
    "country":"United Kingdom",
    "contact_name_on_invoices":true,
    "default_payment_terms_in_days":30,
    "locale":"en",
    "account_balance":"-100.0",
    "uses_contact_invoice_sequence":false,
    "charge_sales_tax":"Auto",
    "sales_tax_registration_number":"ST12345",
    "active_projects_count":1,
    "direct_debit_mandate_state":"active",
    "status":"Active",
    "is_cis_subcontractor":true,
    "cis_deduction_rate":"cis_standard",
    "unique_tax_reference":"1234567890",
    "subcontractor_verification_number":"V1234567890",
    "created_at":"2011-09-14T16:00:41Z",
    "updated_at":"2011-09-16T09:34:41Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <contact>
    <url>https://api.freeagent.com/v2/contacts/2</url>
    <first-name>test</first-name>
    <last-name>me</last-name>
    <organisation-name>Acme Ltd</organisation-name>
    <email>test@example.com</email>
    <billing-email>billing@example.com</billing-email>
    <phone-number>12345678</phone-number>
    <mobile>9876543210</mobile>
    <address1>11 George Street</address1>
    <address2>South Court</address2>
    <address3>Flat 6</address3>
    <town>London</town>
    <region>Southwark</region>
    <postcode>SE1 6HA</postcode>
    <country>United Kingdom</country>
    <contact-name-on-invoices type="boolean">true</contact-name-on-invoices>
    <default-payment-terms-in-days>30</default-payment-terms-in-days>
    <locale>en</locale>
    <account-balance type="decimal">-100.0</account-balance>
    <uses-contact-invoice-sequence type="boolean">false</uses-contact-invoice-sequence>
    <charge-sales-tax>Auto</charge-sales-tax>
    <sales-tax-registration-number>ST12345</sales-tax-registration-number>
    <active-projects-count type="integer">1</active-projects-count>
    <direct-debit-mandate-state>active</direct-debit-mandate-state>
    <direct-debit-mandate>
      <currency>GBP</currency>
      <max-amount type="decimal">5000.0</max-amount>
      <remaining-amount type="decimal">5000.0</remaining-amount>
      <next-interval-starts-on type="date">2017-06-30</next-interval-starts-on>
    </direct-debit-mandate>
    <status>Active</status>
    <is-cis-subcontractor type="boolean">true</is-cis-subcontractor>
    <cis-deduction-rate>cis_standard</cis-deduction-rate>
    <unique-tax-reference>1234567890</unique-tax-reference>
    <subcontractor-verification-number>V1234567890</subcontractor-verification-number>
    <created-at type="datetime">2011-09-14T16:00:41Z</created-at>
    <updated-at type="datetime">2011-09-16T09:34:41Z</updated-at>
  </contact>
</freeagent>
```
Show as JSON

## Create a contact

```http
POST https://api.freeagent.com/v2/contacts
```

Payload should have a root `contact` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/contacts/70
```

```json
{ "contact":
  {
    "url":"https://api.freeagent.com/v2/contacts/70",
    "first_name":"test",
    "last_name":"me",
    "organisation_name":"Acme Ltd",
    "email":"test@example.com",
    "billing_email":"billing@example.com",
    "phone_number":"12345678",
    "mobile":"9876543210",
    "address1":"11 George Street",
    "address2": "Kings Court",
    "address3": "Flat 6",
    "town":"London",
    "region": "Southwark",
    "postcode":"SE1 6HA",
    "country":"United Kingdom",
    "contact_name_on_invoices":true,
    "default_payment_terms_in_days":30,
    "locale":"en",
    "account_balance":"-100.0",
    "uses_contact_invoice_sequence":false,
    "charge_sales_tax":"Auto",
    "sales_tax_registration_number":"ST12345",
    "active_projects_count":0,
    "status":"Active",
    "is_cis_subcontractor":true,
    "cis_deduction_rate":"cis_standard",
    "unique_tax_reference":"1234567890",
    "subcontractor_verification_number":"V1234567890",
    "created_at":"2011-09-14T16:00:41Z",
    "updated_at":"2011-09-16T09:34:41Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <contact>
    <url>https://api.freeagent.com/v2/contacts/70</url>
    <first-name>test</first-name>
    <last-name>me</last-name>
    <organisation-name>Acme Ltd</organisation-name>
    <email>test@example.com</email>
    <billing-email>billing@example.com</billing-email>
    <phone-number>12345678</phone-number>
    <mobile>9876543210</mobile>
    <address1>11 George Street</address1>
    <address2>South Court</address2>
    <address3>Flat 6</address3>
    <town>London</town>
    <region>Southwark</region>
    <postcode>SE1 6HA</postcode>
    <country>United Kingdom</country>
    <contact-name-on-invoices type="boolean">true</contact-name-on-invoices>
    <default-payment-terms-in-days>30</default-payment-terms-in-days>
    <locale>en</locale>
    <account-balance type="decimal">-100.0</account-balance>
    <uses-contact-invoice-sequence type="boolean">false</uses-contact-invoice-sequence>
    <charge-sales-tax>Auto</charge-sales-tax>
    <sales-tax-registration-number>ST12345</sales-tax-registration-number>
    <active-projects-count type="integer">0</active-projects-count>
    <status>Active</status>
    <is-cis-subcontractor type="boolean">true</is-cis-subcontractor>
    <cis-deduction-rate>cis_standard</cis-deduction-rate>
    <unique-tax-reference>1234567890</unique-tax-reference>
    <subcontractor-verification-number>V1234567890</subcontractor-verification-number>
    <created-at type="datetime">2011-09-14T16:00:41Z</created-at>
    <updated-at type="datetime">2011-09-16T09:34:41Z</updated-at>
  </contact>
</freeagent>
```
Show as JSON

## Update a contact

```http
PUT https://api.freeagent.com/v2/contacts/:id
```

Payload should have a root `contact` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a contact

```http
DELETE https://api.freeagent.com/v2/contacts/:id
```

### Response

```http
Status: 200 OK
```