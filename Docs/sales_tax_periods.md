# Sales Tax Periods

*Only available to US and Universal companies*

*Minimum access level*:

- `Estimates & Invoices` to **read**
- `Full Access` to **create, update**, or **delete**

## Attributes

Attributes marked *Required* should be present in the request to create new
sales tax periods.

| Required | Attribute                     | Description                                                      | Kind    |
| -------- | ----------------------------- | ---------------------------------------------------------------- | ------- |
|          | url                           | The unique identifier for the sales tax period                   | URI     |
| ✔        | sales_tax_name                | Name of [sales tax](sales_tax.md) e.g. VAT, GST, Sales Tax       | String  |
| ✔        | sales_tax_registration_status | One of the following: `Not Registered`, `Registered`             | String  |
| ✔        | sales_tax_rate_1              | First rate of the [sales tax](sales_tax.md)                      | Decimal |
|          | sales_tax_rate_2              | Second rate of the [sales tax](sales_tax.md)                     | Decimal |
|          | sales_tax_rate_3              | Third rate of the [sales tax](sales_tax.md)                      | Decimal |
| ✔        | sales_tax_is_value_added      | `true` if you can reclaim tax on what you buy, `false` otherwise | Boolean |
|          | sales_tax_registration_number | Do not specify if no registration number is required             | String  |
| ✔        | effective_date                | When the sales tax period takes effect, in `YYYY-MM-DD` format   | Date    |
|          | is_locked                     | `true` if no changes can be made, `false` otherwise              | Boolean |
|          | locked_reason                 | The reason for the sales tax period being locked                 | Boolean |

#### Extra attributes for Universal companies

All attributes displayed below are optional.

| Attribute                    | Description                                                                                                                           | Kind    |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| second_sales_tax_name        | Name of [second sales tax](sales_tax.md) (e.g. PST in some Canadian provinces) Do not specify if no second Sales Tax is to be charged | String  |
| second_sales_tax_rate_1      | First rate of the [second sales tax](sales_tax.md)                                                                                    | Decimal |
| second_sales_tax_rate_2      | Second rate of the [second sales tax](sales_tax.md)                                                                                   | Decimal |
| second_sales_tax_rate_3      | Third rate of the [second sales tax](sales_tax.md)                                                                                    | Decimal |
| second_sales_tax_is_compound | `true` if applied on top of the main sales tax instead of independently, `false` otherwise                                            | Boolean |

## List all sales tax periods for a company

```http
GET https://api.freeagent.com/v2/sales_tax_periods
```

### Response

```http
Status: 200 OK
```

```json
{
  "sales_tax_periods": [
    {
      "url": "https://api.freeagent.com/v2/sales_tax_periods/1",
      "sales_tax_name": "First Tax",
      "sales_tax_rate_1": "2.0",
      "sales_tax_rate_2": "3.0",
      "sales_tax_rate_3": "4.0",
      "sales_tax_is_value_added": true,
      "sales_tax_registration_status": "Registered",
      "sales_tax_registration_number": "12345678",
      "effective_date": "2016-09-21",
      "is_locked": false,
      "second_sales_tax_name": "Second Tax",
      "second_sales_tax_rate_1": "10.0",
      "second_sales_tax_rate_2": "20.0",
      "second_sales_tax_rate_3": "50.0",
      "second_sales_tax_is_compound": true
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <sales-tax-periods type="array">
    <sales-tax-period>
      <url>https://api.freeagent.com/v2/sales_tax_periods/1</url>
      <sales-tax-name>First Tax</sales-tax-name>
      <sales-tax-rate-1 type="decimal">2.0</sales-tax-rate-1>
      <sales-tax-rate-2 type="decimal">3.0</sales-tax-rate-2>
      <sales-tax-rate-3 type="decimal">4.0</sales-tax-rate-3>
      <sales-tax-is-value-added type="boolean">true</sales-tax-is-value-added>
      <sales-tax-registration-status>Registered</sales-tax-registration-status>
      <sales-tax-registration-number>12345678</sales-tax-registration-number>
      <effective-date type="date">2016-09-21</effective-date>
      <is-locked type="boolean">false</is-locked>
      <second-sales-tax-name>Second Tax</second-sales-tax-name>
      <second-sales-tax-rate-1 type="decimal">10.0</second-sales-tax-rate-1>
      <second-sales-tax-rate-2 type="decimal">20.0</second-sales-tax-rate-2>
      <second-sales-tax-rate-3 type="decimal">50.0</second-sales-tax-rate-3>
      <second-sales-tax-is-compound type="boolean">true</second-sales-tax-is-compound>
    </sales-tax-period>
  </sales-tax-periods>
</freeagent>
```
Show as JSON

## Get a single sales tax period

```http
GET https://api.freeagent.com/v2/sales_tax_periods/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "sales_tax_period": {
    "url": "https://api.freeagent.com/v2/sales_tax_periods/2",
    "sales_tax_name": "First Tax",
    "sales_tax_rate_1": "2.0",
    "sales_tax_rate_2": "3.0",
    "sales_tax_rate_3": "4.0",
    "sales_tax_is_value_added": true,
    "sales_tax_registration_status": "Registered",
    "sales_tax_registration_number": "12345678",
    "effective_date": "2016-09-21",
    "is_locked": false,
    "second_sales_tax_name": "Second Tax",
    "second_sales_tax_rate_1": "10.0",
    "second_sales_tax_rate_2": "20.0",
    "second_sales_tax_rate_3": "50.0",
    "second_sales_tax_is_compound": true
  }
}
```
Show as XML

```xml
<freeagent>
  <sales-tax-period>
    <url>https://api.freeagent.com/v2/sales_tax_periods/2</url>
    <sales-tax-name>First Tax</sales-tax-name>
    <sales-tax-rate-1 type="decimal">2.0</sales-tax-rate-1>
    <sales-tax-rate-2 type="decimal">3.0</sales-tax-rate-2>
    <sales-tax-rate-3 type="decimal">4.0</sales-tax-rate-3>
    <sales-tax-is-value-added type="boolean">true</sales-tax-is-value-added>
    <sales-tax-registration-status>Registered</sales-tax-registration-status>
    <sales-tax-registration-number>12345678</sales-tax-registration-number>
    <effective-date type="date">2016-09-21</effective-date>
    <is-locked type="boolean">false</is-locked>
    <second-sales-tax-name>Second Tax</second-sales-tax-name>
    <second-sales-tax-rate-1 type="decimal">10.0</second-sales-tax-rate-1>
    <second-sales-tax-rate-2 type="decimal">20.0</second-sales-tax-rate-2>
    <second-sales-tax-rate-3 type="decimal">50.0</second-sales-tax-rate-3>
    <second-sales-tax-is-compound type="boolean">true</second-sales-tax-is-compound>
  </sales-tax-period>
</freeagent>
```
Show as JSON

## Create a sales tax period

```http
POST https://api.freeagent.com/v2/sales_tax_periods
```

Payload should have a root `sales_tax_period` element, containing
elements listed under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/sales_tax_periods/123
```

```json
{
  "sales_tax_period": {
    "url": "https://api.freeagent.com/v2/sales_tax_periods/2",
    "sales_tax_name": "First Tax",
    "sales_tax_rate_1": "2.0",
    "sales_tax_rate_2": "3.0",
    "sales_tax_rate_3": "4.0",
    "sales_tax_is_value_added": true,
    "sales_tax_registration_status": "Registered",
    "sales_tax_registration_number": "12345678",
    "effective_date": "2016-09-21",
    "is_locked": false,
    "second_sales_tax_name": "Second Tax",
    "second_sales_tax_rate_1": "10.0",
    "second_sales_tax_rate_2": "20.0",
    "second_sales_tax_rate_3": "50.0",
    "second_sales_tax_is_compound": true
  }
}
```
Show as XML

```xml
<freeagent>
  <sales-tax-period>
    <url>https://api.freeagent.com/v2/sales_tax_periods/2</url>
    <sales-tax-name>First Tax</sales-tax-name>
    <sales-tax-rate-1 type="decimal">2.0</sales-tax-rate-1>
    <sales-tax-rate-2 type="decimal">3.0</sales-tax-rate-2>
    <sales-tax-rate-3 type="decimal">4.0</sales-tax-rate-3>
    <sales-tax-is-value-added type="boolean">true</sales-tax-is-value-added>
    <sales-tax-registration-status>Registered</sales-tax-registration-status>
    <sales-tax-registration-number>12345678</sales-tax-registration-number>
    <effective-date type="date">2016-09-21</effective-date>
    <is-locked type="boolean">false</is-locked>
    <second-sales-tax-name>Second Tax</second-sales-tax-name>
    <second-sales-tax-rate-1 type="decimal">10.0</second-sales-tax-rate-1>
    <second-sales-tax-rate-2 type="decimal">20.0</second-sales-tax-rate-2>
    <second-sales-tax-rate-3 type="decimal">50.0</second-sales-tax-rate-3>
    <second-sales-tax-is-compound type="boolean">true</second-sales-tax-is-compound>
  </sales-tax-period>
</freeagent>
```
Show as JSON

## Update a sales tax period

```http
PUT https://api.freeagent.com/v2/sales_tax_periods/:id
```

Payload should have a root `sales_tax_period` element, containing
elements listed under Attributes that you wish
to update.

### Response

```http
Status: 200 OK
```

## Delete a sales tax period

**Note: Deleting a locked period is not allowed.**

```http
DELETE https://api.freeagent.com/v2/sales_tax_periods/:id
```

### Response

```http
Status: 200 OK
```