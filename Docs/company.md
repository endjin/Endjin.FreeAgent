# Company

*Minimum access level*: `Time`, unless stated otherwise.

The company object represents the company for which FreeAgent is managing the accounts.

## Attributes

| Attribute                               | Description                                                                                                                                                                                                                                        | Kind      |
| --------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| url                                     | The API endpoint for the authenticated company                                                                                                                                                                                                     | URI       |
| id                                      | The ID of the company                                                                                                                                                                                                                              | Integer   |
| name                                    | Company name                                                                                                                                                                                                                                       | String    |
| subdomain                               | Company subdomain                                                                                                                                                                                                                                  | String    |
| type                                    | One of the following: `UkLimitedCompany`, `UkLimitedLiabilityPartnership`, `UkPartnership`, `UkSoleTrader`, `UkUnincorporatedLandlord`, `UsLimitedLiabilityCompany`, `UsPartnership`, `UsSoleProprietor`, `UsCCorp`, `UsSCorp`, `UniversalCompany` | String    |
| currency                                | Base [accounting currency](currencies.md)                                                                                                                                                                                                          | String    |
| mileage_units                           | Either `miles` or `kilometers`                                                                                                                                                                                                                     | String    |
| company_start_date                      | Date the business or company was started in `YYYY-MM-DD` format                                                                                                                                                                                    | Date      |
| trading_start_date                      | Date company commenced trading (if it is different from `company_start_date`)                                                                                                                                                                      | Date      |
| first_accounting_year_end               | Date the company's first set of annual accounts are drawn up to. (normally this will be one year after the end of the month company was started for limited companies and the end of the tax year for sole traders and partnerships)               | Date      |
| annual_accounting_periods               | The periods covered by each of the company's sets of annual accounts                                                                                                                                                                               | Array     |
| freeagent_start_date                    | Transactions dated on or after this date will be used by FreeAgent to calculate accounts (normally the start of an accounting year to keep things simple)                                                                                          | Date      |
| address1                                | First line of address                                                                                                                                                                                                                              | String    |
| address2                                | Second line of address                                                                                                                                                                                                                             | String    |
| address3                                | Third line of address                                                                                                                                                                                                                              | String    |
| town                                    | Town                                                                                                                                                                                                                                               | String    |
| region                                  | Region or State                                                                                                                                                                                                                                    | String    |
| postcode                                | Post / Zip Code                                                                                                                                                                                                                                    | String    |
| country                                 | Country                                                                                                                                                                                                                                            | String    |
| company_registration_number             | Company registration number                                                                                                                                                                                                                        | String    |
| contact_email                           | Contact email address                                                                                                                                                                                                                              | String    |
| contact_phone                           | Contact phone number                                                                                                                                                                                                                               | String    |
| website                                 | Website address                                                                                                                                                                                                                                    | String    |
| business_type                           | Free-text description of the business                                                                                                                                                                                                              | String    |
| business_category                       | Company's [business category](#list-all-business-categories)                                                                                                                                                                                       | String    |
| short_date_format                       | Date format to use throughout the FreeAgent account: `dd mmm yy`, `dd-mm-yyyy`, `mm/dd/yyyy`, `yyyy-mm-dd`                                                                                                                                         | String    |
| sales_tax_name                          | Name of current [sales tax](sales_tax.md) e.g. VAT, GST, Sales Tax                                                                                                                                                                                 | String    |
| sales_tax_registration_number           | Current [sales tax](sales_tax.md) registration number                                                                                                                                                                                              | String    |
| sales_tax_effective_date                | When current [sales tax](sales_tax.md) took or will take effect, in `YYYY-MM-DD` format                                                                                                                                                            | Date      |
| sales_tax_rates                         | Current [sales tax](sales_tax.md) rates applicable to the company                                                                                                                                                                                  | Array     |
| sales_tax_is_value_added                | `true` if VAT applies, `false` otherwise                                                                                                                                                                                                           | Boolean   |
| cis_enabled                             | `true` if Construction Industry Scheme for Subcontractors is enabled, `false` otherwise Also aliased as: `cis_subcontractor`                                                                                                                       | Boolean   |
| cis_subcontractor                       | `true` if Construction Industry Scheme for Subcontractors is enabled, `false` otherwise Also aliased as: `cis_enabled`                                                                                                                             | Boolean   |
| cis_contractor                          | `true` if Construction Industry Scheme for Contractors is enabled, `false` otherwise                                                                                                                                                               | Boolean   |
| locked_attributes                       | List of attributes that cannot be modified                                                                                                                                                                                                         | Array     |
| created_at                              | Creation of the company resource (UTC)                                                                                                                                                                                                             | Timestamp |
| updated_at                              | When the company resource was last updated (UTC)                                                                                                                                                                                                   | Timestamp |
| vat_first_return_period_ends_on         | When the first VAT return period ends                                                                                                                                                                                                              | Date      |
| initial_vat_basis                       | VAT accounting basis on the VAT registration date: `Invoice`, `Cash` Only affects the automatically-created initial VAT Returns                                                                                                                    | String    |
| initially_on_frs                        | `true` if company was on a Flat Rate Scheme on the VAT registration date, `false` otherwise                                                                                                                                                        | Boolean   |
| initial_vat_frs_type                    | Flat Rate Scheme the company has registered under                                                                                                                                                                                                  | String    |
| sales_tax_deregistration_effective_date | Date of VAT de-registration, if `sales_tax_registration_status` is `De-registered`                                                                                                                                                                 | Date      |
| second_sales_tax_name                   | Name of current [second sales tax](sales_tax.md) e.g. PST in some Canadian provinces                                                                                                                                                               | String    |
| second_sales_tax_rates                  | Current [second sales tax](sales_tax.md) rates applicable to the company                                                                                                                                                                           | Array     |
| second_sales_tax_is_compound            | `true` if it is applied on top of the main [sales tax](sales_tax.md) instead of independently, `false` otherwise                                                                                                                                   | Boolean   |

## General company information

The list of basic information about a company held by FreeAgent.

```http
GET https://api.freeagent.com/v2/company
```

### Response

```http
Status: 200 OK
```

```json
{ "company":
  {
    "url":"https://api.freeagent.com/v2/company",
    "id":"12345",
    "name":"My Company",
    "subdomain":"mycompany",
    "type":"UkLimitedCompany",
    "currency":"GBP",
    "mileage_units":"miles",
    "company_start_date":"2020-05-01",
    "freeagent_start_date":"2020-05-01",
    "first_accounting_year_end":"2011-04-30",
    "annual_accounting_periods": [
      { "starts_on":"2020-05-01", "ends_on":"2021-04-30" },
      { "starts_on":"2021-05-01", "ends_on":"2022-04-30" }
    ],
    "company_registration_number":"123456",
    "sales_tax_registration_status":"Registered",
    "sales_tax_registration_number":"123456",
    "cis_enabled": true,
    "cis_subcontractor": true,
    "cis_contractor": false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <company>
    <url>https://api.freeagent.com/v2/company</url>
    <id>12345</id>
    <name>My Company</name>
    <subdomain>mycompany</subdomain>
    <type>UkLimitedCompany</type>
    <currency>GBP</currency>
    <mileage-units>miles</mileage-units>
    <company-start-date type="date">2010-05-01</company-start-date>
    <freeagent-start-date type="date">2010-05-01</freeagent-start-date>
    <first-accounting-year-end type="date">2010-05-01</first-accounting-year-end>
    <annual-accounting-periods type="array">
      <annual-accounting-period>
        <starts-on type="date">2020-05-01</starts-on>
        <ends-on type="date">2021-04-30</ends-on>
      </annual-accounting-period>
      <annual-accounting-period>
        <starts-on type="date">2021-05-01</starts-on>
        <ends-on type="date">2022-04-30</ends-on>
      </annual-accounting-period>
    </annual-accounting-periods>
    <sales-tax-registration-status>Registered</sales-tax-registration-status>
    <sales-tax-registration-number>123456</sales-tax-registration-number>
    <cis-enabled type="boolean">true</cis-enabled>
    <cis-subcontractor type="boolean">true</cis-subcontractor>
    <cis-contractor type="boolean">false</cis-contractor>
  </company>
</freeagent>
```
Show as JSON

## List all business categories

The values returned by this endpoint can be used for the `business_category` attribute.

```http
GET https://api.freeagent.com/v2/company/business_categories
```

### Response

```http
Status: 200 OK
```

```json
{
  "business_categories" : [
    "Accounting & Bookkeeping",
    "Administration",
    "Agriculture",
    "Apparel & Fashion",
    "Architecture & Planning",
    "Arts & Crafts",
    "Automotive",
    "Aviation",
    "Biotechnology",
    "Builder",
    "Business / Management Consulting",
    "Childcare",
    "Cleaning",
    "Commercial Property",
    "Communications",
    "Courier",
    "Design",
    "Driver (Taxi / Private)",
    "Education",
    "Electrician",
    "Energy",
    "Engineering",
    "Entertainment",
    "Events",
    "Film & TV",
    "Financial Services",
    "Floristry",
    "Food & Beverages",
    "Hair & Beauty",
    "Gambling & Casinos",
    "Health & Social Care",
    "Health, Wellness and Fitness",
    "Hospitality",
    "IT Contractor / Consulting",
    "Joiner",
    "Landscape Gardener",
    "Legal Services",
    "Leisure & Tourism",
    "Logistics",
    "Marketing & Advertising",
    "Music",
    "Painter and Decorator",
    "Performing Arts",
    "Photography",
    "Plasterer",
    "Plumber",
    "Property / Landlord",
    "Retail",
    "Social Clubs",
    "Software Development",
    "Utilities",
    "Vet & Pet Care",
    "Web Design"
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <business-categories type="array">
    <business-category>Accounting &amp; Bookkeeping</business-category>
    <business-category>Administration</business-category>
    <business-category>Agriculture</business-category>
    <business-category>Apparel &amp; Fashion</business-category>
    <business-category>Architecture &amp; Planning</business-category>
    <business-category>Arts &amp; Crafts</business-category>
    <business-category>Automotive</business-category>
    <business-category>Aviation</business-category>
    <business-category>Biotechnology</business-category>
    <business-category>Builder</business-category>
    <business-category>Business / Management Consulting</business-category>
    <business-category>Childcare</business-category>
    <business-category>Cleaning</business-category>
    <business-category>Commercial Property</business-category>
    <business-category>Communications</business-category>
    <business-category>Courier</business-category>
    <business-category>Design</business-category>
    <business-category>Driver (Taxi / Private)</business-category>
    <business-category>Education</business-category>
    <business-category>Electrician</business-category>
    <business-category>Energy</business-category>
    <business-category>Engineering</business-category>
    <business-category>Entertainment</business-category>
    <business-category>Events</business-category>
    <business-category>Film &amp; TV</business-category>
    <business-category>Financial Services</business-category>
    <business-category>Floristry</business-category>
    <business-category>Food &amp; Beverages</business-category>
    <business-category>Gambling &amp; Casinos</business-category>
    <business-category>Hair &amp; Beauty</business-category>
    <business-category>Health &amp; Social Care</business-category>
    <business-category>Health, Wellness and Fitness</business-category>
    <business-category>Hospitality</business-category>
    <business-category>IT Contractor / Consulting</business-category>
    <business-category>Joiner</business-category>
    <business-category>Landscape Gardener</business-category>
    <business-category>Legal Services</business-category>
    <business-category>Leisure &amp; Tourism</business-category>
    <business-category>Logistics</business-category>
    <business-category>Marketing &amp; Advertising</business-category>
    <business-category>Music</business-category>
    <business-category>Other</business-category>
    <business-category>Painter and Decorator</business-category>
    <business-category>Performing Arts</business-category>
    <business-category>Photography</business-category>
    <business-category>Plasterer</business-category>
    <business-category>Plumber</business-category>
    <business-category>Property / Landlord</business-category>
    <business-category>Retail</business-category>
    <business-category>Social Clubs</business-category>
    <business-category>Software Development</business-category>
    <business-category>Utilities</business-category>
    <business-category>Vet &amp; Pet Care</business-category>
    <business-category>Web Design</business-category>
  </business-categories>
</freeagent>
```
Show as JSON

## Information about upcoming tax events

*Minimum access level*: `Tax, Accounting and Users`.

```http
GET https://api.freeagent.com/v2/company/tax_timeline
```

### Response

```http
Status: 200 OK
```

```json
{ "timeline_items":[
  {
    "description":"VAT Return 09 11",
    "nature":"Electronic Submission and Payment Due",
    "dated_on":"2011-11-07",
    "amount_due":"-214.16",
    "is_personal":false
  },
  {
    "description":"Accounting Period Ending 31 May 11",
    "nature":"Companies House First Accounts Due",
    "dated_on":"2012-02-01",
    "is_personal":false
  },
  {
    "description":"Corporation Tax, year ending 31 May 11",
    "nature":"Payment Due",
    "dated_on":"2012-03-01",
    "amount_due":3543.7,
    "is_personal":false
  },
  {
    "description":"Annual Return as at 01 May 12",
    "nature":"Companies House Annual Return Due",
    "dated_on":"2012-05-29",
    "is_personal":false
  },
  {
    "description":"Corporation Tax, year ending 31 May 11",
    "nature":"Submission Due",
    "dated_on":"2012-05-31",
    "is_personal":false
  },
  {
    "description":"Accounting Period Ending 31 May 12",
    "nature":"Companies House Accounts Due",
    "dated_on":"2013-02-28",
    "is_personal":false
  },
  {
    "description":"Corporation Tax, year ending 31 May 12",
    "nature":"Payment Due",
    "dated_on":"2013-03-01",
    "amount_due":0,
    "is_personal":false
  },
  {
    "description":"Annual Return as at 01 May 13",
    "nature":"Companies House Annual Return Due",
    "dated_on":"2013-05-29",
    "is_personal":false
  },
  {
    "description":"Corporation Tax, year ending 31 May 12",
    "nature":"Submission Due",
    "dated_on":"2013-05-31",
    "is_personal":false
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <timeline-items type="array">
    <timeline-item>
      <description>VAT Return 08 11</description>
      <nature>Electronic Submission and Payment Due</nature>
      <dated-on type="date">2011-10-07</dated-on>
      <amount-due type="decimal">751.66</amount-due>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>VAT Return 11 11</description>
      <nature>Electronic Submission and Payment Due</nature>
      <dated-on type="date">2012-01-07</dated-on>
      <amount-due type="decimal">399.99</amount-due>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Accounting Period Ending 31 Jul 11</description>
      <nature>Companies House First Accounts Due</nature>
      <dated-on type="date">2012-04-01</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Corporation Tax, year ending 31 Jul 11</description>
      <nature>Payment Due</nature>
      <dated-on type="date">2012-05-01</dated-on>
      <amount-due type="decimal">2793.44</amount-due>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Annual Return as at 01 Jul 12</description>
      <nature>Companies House Annual Return Due</nature>
      <dated-on type="date">2012-07-29</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Corporation Tax, year ending 31 Jul 11</description>
      <nature>Submission Due</nature>
      <dated-on type="date">2012-07-31</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Accounting Period Ending 31 Jul 12</description>
      <nature>Companies House Accounts Due</nature>
      <dated-on type="date">2013-04-30</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Corporation Tax, year ending 31 Jul 12</description>
      <nature>Payment Due</nature>
      <dated-on type="date">2013-05-01</dated-on>
      <amount-due type="decimal">360.0</amount-due>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Annual Return as at 01 Jul 13</description>
      <nature>Companies House Annual Return Due</nature>
      <dated-on type="date">2013-07-29</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
    <timeline-item>
      <description>Corporation Tax, year ending 31 Jul 12</description>
      <nature>Submission Due</nature>
      <dated-on type="date">2013-07-31</dated-on>
      <is-personal type="boolean">false</is-personal>
    </timeline-item>
  </timeline-items>
</freeagent>
```
Show as JSON