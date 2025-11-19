# Sales Tax and VAT

There are several different ways to set the `sales_tax_rate` or
`manual_sales_tax` amount, depending on what is passed in the request and which
resource you are requesting. Depending on what is set, the API will respond
with different fields.

## Expenses, Bills and Bill items

Normally there is no need to specify the `sales_tax_rate` or the
`manual_sales_tax_amount` for an expense, bill or bill item yourself. If you leave both of these
fields out, the correct `sales_tax_rate` will be set for the category of the
expense or bill. The same will happen if you include `"sales_tax_rate": "Auto"` in your request body.
If you later retrieve an expense or bill set in this manner, the response will
include the `sales_tax_rate` field, as well as the calculated `sales_tax_value`.

If you include a `sales_tax_rate` when creating an expense, bill or bill item through the API this
is used to calculate a `sales_tax_value`. This `sales_tax_value` is calculated from the
gross value and the `sales_tax_rate` percentage supplied. It will be returned in the
response and in future requests in addition to the rate.

For example, if you posted the JSON below, the response would include
a `sales_tax_value` of 16.67, 20% of 83.33. These two amounts, when
added together, give the gross value of 100.

```json
{ "expense":
    {
        "user": "https://api.freeagent.com/v2/users/1",
        "category": "https://api.freeagent.com/v2/categories/285",
        "dated_on": "2019-10-22",
        "gross_value": "-100.0",
        "sales_tax_rate": "20",
        "description": "Example of setting a sales_tax_rate manually",
        "receipt_reference": "001",
    }
}
```

If you include a `manual_sales_tax_amount` when creating an expense, bill or bill item through the
API, it will set this and return the `manual_sales_tax_amount` alongside `sales_tax_value`
in the response and for all other requests for this expense, bill or bill item in the future. No
`sales_tax_rate` will be returned but you can calculate it yourself, as shown below.

If you specify both a `manual_sales_tax_amount` and a `sales_tax_rate`, the
`sales_tax_rate` will be ignored and the `sales_tax_value` will be calculated based only
on the `manual_sales_tax_amount` (both value and amount will be returned in the response).

To calculate the `sales_tax_rate` from the `manual_sales_tax_amount` and the
`gross_value` you can use the following formula:

```json
net_value = abs(gross_value) - manual_sales_tax_amount
sales_tax_rate = (manual_sales_tax_amount / net_value) * 100
```

If you need to manually mark an expense/bill/bill item as sales tax exempt or out of scope, you can use `"sales_tax_status":
"EXEMPT` or `"sales_tax_status": "OUT_OF_SCOPE"` in your request.

## Invoices

Invoices are much simpler than expenses and bills; their sales tax should always be set using the `sales_tax_rate`
parameter for each `invoice_item`. An example of this is shown below.

```json
{ "invoice":
  {
    "contact":"https://api.freeagent.com/v2/contacts/2",
    "dated_on":"2019-12-12T00:00:00+00:00",
    "due_on":"2019-12-17T00:00:00+00:00",
    "reference":"003",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "net_value":"0.0",
    "status":"Draft",
    "comments":"An example invoice comment.",
    "omit_header":false,
    "payment_terms_in_days":5,
    "ec_status":"EC Goods",
    "invoice_items":[
      {
        "description":"Test InvoiceItem",
        "item_type":"Hours",
        "price":"100.0",
        "quantity":"1.0",
        "sales_tax_rate":"20"
      }
    ]
  }
}
```

The `sales_tax_rate` field specifies what percentage of the item's price will be added to the invoice as sales tax.
It is recommended that it is included for every new invoice item. If `sales_tax_rate` isn't included explicitly in
the API request body, the invoice's sales tax settings will be determined by the contact's `charge_sales_tax`
attribute which can be set to `Always`, `Never` or `Auto` (which, for a VAT-registered UK-based company, is
equivalent to the "Only if contact is also based in the United Kingdom VAT area" option from the FreeAgent web UI).

If you need to manually mark an invoice item as sales tax exempt, you can use `"sales_tax_status": "EXEMPT` instead
of a rate in your request.

The created invoice will include an additional `sales_tax_value` attribute which is the sum of sales tax from all
invoice items. This is a read-only attribute which cannot be set manually.

## EC VAT MOSS

Sales can be tagged as VAT MOSS by providing an `ec_status` of `EC VAT MOSS`.

When creating a MOSS Invoice or Estimate you must also provide a `place_of_supply`.
When creating a Bank Transaction Explanation, you must provide a `place_of_supply`
and a `sales_tax_rate`.

The `place_of_supply` field must be a country name from the following list of EU members:

- Austria
- Belgium
- Bulgaria
- Croatia
- Cyprus
- Czech Republic
- Denmark
- Estonia
- Finland
- France
- Germany
- Greece
- Hungary
- Ireland
- Italy
- Latvia
- Lithuania
- Luxembourg
- Malta
- Netherlands
- Poland
- Portugal
- Romania
- Slovakia
- Slovenia
- Spain
- Sweden

You can get the available MOSS rates for the `sales_tax_rate` field from the following endpoint:

```http
GET https://api.freeagent.com/v2/ec_moss/sales_tax_rates?country=Austria&date=2017-01-01
```

- country - The country specified as `place_of_supply`
- date - The transaction date in `YYYY-MM-DD` format

### Response

```http
Status: 200 OK
```

```json
{
  "sales_tax_rates": [
    {
      "percentage": "20.0",
      "band": "Standard"
    },
    {
      "percentage": "13.0",
      "band": "Reduced"
    },
    {
      "percentage": "12.0",
      "band": "Parking"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <sales-tax-rates type="array">
    <sales-tax-rate>
      <percentage type="decimal">20.0</percentage>
      <band>Standard</band>
    </sales-tax-rate>
    <sales-tax-rate>
      <percentage type="decimal">13.0</percentage>
      <band>Reduced</band>
    </sales-tax-rate>
    <sales-tax-rate>
      <percentage type="decimal">12.0</percentage>
      <band>Parking</band>
    </sales-tax-rate>
  </sales-tax-rates>
</freeagent>
```
Show as JSON