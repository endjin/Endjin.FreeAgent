# Bills

*Minimum access level*: `Bills`, unless stated otherwise.

## Attributes

| Required | Attribute                  | Description                                                                                                                                                                                                                                                                                                                                                                                                                                | Kind      |
| -------- | -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- |
|          | url                        | The unique identifier for the bill                                                                                                                                                                                                                                                                                                                                                                                                         | URI       |
| ✔        | contact                    | [Contact](contacts.md) being billed                                                                                                                                                                                                                                                                                                                                                                                                        | URI       |
| ✔        | reference                  | Free-text reference                                                                                                                                                                                                                                                                                                                                                                                                                        | String    |
| ✔        | dated_on                   | Date of bill                                                                                                                                                                                                                                                                                                                                                                                                                               | Date      |
| ✔        | due_on                     | Due date of bill                                                                                                                                                                                                                                                                                                                                                                                                                           | Date      |
|          | paid_on                    | Date of most recent payment, only returned for fully paid bills and bill refunds [[Learn more about bill refunds](https://support.freeagent.com/hc/en-gb/articles/115001222824-Record-a-bill-credit-note)]                                                                                                                                                                                                                                 | Date      |
|          | status                     | Bill's payment status, determined based on the presence of associated [bank transaction explanations](bank_transaction_explanations.md). Can be one of the following: `Zero Value`, `Open`, `Paid`, `Overdue`, `Refunded` - if the bill's total value is a negative number (making it a [bill refund](https://support.freeagent.com/hc/en-gb/articles/115001222824-Record-a-bill-credit-note)), and it's been fully paid                   | String    |
|          | long_status                | Bill's payment status along with the due date as a relative date to 'today' For example: Open - due in 21 days                                                                                                                                                                                                                                                                                                                             | String    |
|          | currency                   | Bill's [currency](currencies.md) Defaults to the company's native currency                                                                                                                                                                                                                                                                                                                                                                 | String    |
|          | input_total_values_inc_tax | Whether bill items are entered including or excluding sales tax on the web app or mobile app. From the API, either `total_value` or `total_value_ex_tax` can be used interchangeably by an API integration as desired to set or retrieve the amount with or without sales tax. Defaults to `false` if the bill is in native currency, and to `true` otherwise                                                                              | Boolean   |
|          | total_value                | Total value of the bill, calculated from the bill item totals                                                                                                                                                                                                                                                                                                                                                                              | Decimal   |
|          | due_value                  | Due value of the bill                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |
|          | native_due_value           | Due value of the bill in the company's native [currency](currencies.md), calculated using the exchange rate on the bill's `paid_on` date if the bill is fully paid or a bill refund; Otherwise, today's `exchange_rate` is used instead                                                                                                                                                                                                    | Decimal   |
|          | net_value                  | Net value of the bill                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |
|          | exchange_rate              | Rate at which bill amount is converted into company's native [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                     | Decimal   |
|          | sales_tax_value            | Total value of sales tax, calculated from the bill item amounts                                                                                                                                                                                                                                                                                                                                                                            | Decimal   |
|          | second_sales_tax_value     | [Universal accounts only] Total value of second sales tax, calculated from the bill item amounts                                                                                                                                                                                                                                                                                                                                           | Decimal   |
|          | is_paid_by_hire_purchase   | Whether the bill will be paid using a hire purchase agreement Defaults to `false`                                                                                                                                                                                                                                                                                                                                                          | Boolean   |
|          | ec_status                  | Bill's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge` Please note that `EC Goods` and `EC Services` are no longer valid options if the bill is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the bill is dated 1/1/2021 or later. | String    |
|          | comments                   | Free-text comments                                                                                                                                                                                                                                                                                                                                                                                                                         | String    |
|          | project                    | [Project](projects.md) billed for                                                                                                                                                                                                                                                                                                                                                                                                          | URI       |
| ?        | rebill_type                | One of the following, if rebilling a project: `cost`, `markup`, `price`                                                                                                                                                                                                                                                                                                                                                                    | String    |
| ?        | rebill_factor              | How much to rebill for Required when `rebill_type` is `markup` or `price`                                                                                                                                                                                                                                                                                                                                                                  | Decimal   |
|          | rebill_to_project          | Same as `project`                                                                                                                                                                                                                                                                                                                                                                                                                          | URI       |
| ?        | property                   | The [property](properties.md) pertaining to this bill. Only accepted and required for companies with type `UkUnincorporatedLandlord`.                                                                                                                                                                                                                                                                                                      | URI       |
|          | recurring                  | Frequency at which the bill will recur. Can be one of the following: `Weekly`, `Two Weekly`, `Four Weekly`, `Two Monthly`, `Quarterly`, `Biannually`, `Annually`, `2-Yearly`                                                                                                                                                                                                                                                               | String    |
|          | recurring_end_date         | When the bill should stop recurring in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                 | Date      |
|          | attachment                 | Explanation attachment (max 5MB), in the following format: `data` (binary data of the file being attached encoded as base64), `file_name`, `description`, `content_type` can be one of the following: `image/png` `image/x-png` `image/jpeg` `image/jpg` `image/gif` `application/x-pdf`, `image/png`, `image/x-png`, `image/jpeg`, `image/jpg`, `image/gif`, `application/x-pdf`                                                          | Object    |
|          | cis_deduction_band         | The CIS band of the bill, taken from the CIS band of the contact when the bill is created, specified as the name of a [Construction Industry Scheme band](cis_bands.md). One of the following: `cis_gross`, `cis_standard`, `cis_higher` This attribute will be present if the bill has any `bill_items` with a CIS for Contractors `category` (using codes 096-099)                                                                       | String    |
|          | cis_deduction_rate         | The CIS deduction rate of the bill, taken from the rate of the contact when the bill was created, as a decimal number This attribute will be present if the bill has any `bill_items` with a CIS for Contractors `category` (using codes 096-099)                                                                                                                                                                                          | Decimal   |
|          | cis_deduction              | The value of the CIS deduction on the bill This attribute will be present if the bill has any `bill_items` with a CIS for Contractors `category` (using codes 096-099)                                                                                                                                                                                                                                                                     | Decimal   |
|          | cis_deduction_suffered     | The value of the CIS deduction suffered on payments for the bill This attribute will be present if the bill has any `bill_items` with a CIS for Contractors `category` (using codes 096-099)                                                                                                                                                                                                                                               | Decimal   |
| ✔        | bill_items                 | Array of bill item data structures, up to a maximum of 40. See [Bill Item Attributes](#bill-item-attributes).                                                                                                                                                                                                                                                                                                                              | Array     |
|          | created_at                 | Creation of the bill resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                        | Timestamp |
|          | updated_at                 | When the bill resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                              | Timestamp |

## Bill Item Attributes

| Required | Attribute               | Description                                                                                                                                                                                                                                                                                                                                                                                          | Kind    |
| -------- | ----------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
|          | url                     | Identifier of the bill item to update/delete, or blank to create a new bill item. Optional when creating bills, required when updating.                                                                                                                                                                                                                                                              | URI     |
|          | bill                    | The parent bill of the item                                                                                                                                                                                                                                                                                                                                                                          | URI     |
| ✔        | category                | [Accounting category](categories.md) of the bill item                                                                                                                                                                                                                                                                                                                                                | URI     |
| ?        | description             | Description of the bill item Required when `category` is of [capital asset type](capital_asset_types.md) Not relevant when `unit` is `Stock`.                                                                                                                                                                                                                                                        | String  |
| ✔        | total_value             | Value of the item including taxes                                                                                                                                                                                                                                                                                                                                                                    | Decimal |
|          | total_value_ex_tax      | Value of the item excluding taxes, may be used instead of `total_value`                                                                                                                                                                                                                                                                                                                              | Decimal |
|          | manual_sales_tax_amount | Amount of [sales tax](sales_tax.md) for the bill item, in the company's native currency                                                                                                                                                                                                                                                                                                              | Decimal |
|          | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates                                                                                                                                                                                                                                                                                                                                                  | Decimal |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                                                                                                                                                                                                                                                 | Decimal |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE`, `EXEMPT` or `OUT_OF_SCOPE` for [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                    | String  |
|          | second_sales_tax_status | [Universal accounts only] [Sales tax period](sales_tax_periods.md) defines a second sales tax                                                                                                                                                                                                                                                                                                        | String  |
|          | unit                    | One of the following: `-no unit-` (default), `Hours`, `Days`, `Weeks`, `Months`, `Years`, `Products`, `Services`, `Training`, `Stock`                                                                                                                                                                                                                                                                | String  |
|          | quantity                | Quantity of the `unit` Present when `category` is not of [capital asset type](capital_asset_types.md) or Purchase of Stock Presently this will be defaulted to `1`                                                                                                                                                                                                                                   | Decimal |
| ?        | stock_item              | [Stock item](stock_items.md) being purchased, required when category is set to Stock                                                                                                                                                                                                                                                                                                                 | URI     |
|          | stock_item_description  | Description of the bill item's stock item, only returned when stock item is set                                                                                                                                                                                                                                                                                                                      | String  |
| ?        | stock_altering_quantity | Quantity of `stock_item` units purchased, required when category is set to Stock                                                                                                                                                                                                                                                                                                                     | Decimal |
|          | depreciation_schedule   | Note! This field is deprecated. Fetch the capital asset using the link in the capital_asset field of the response to view full details of its depreciation profile. Number of years over which the [asset](capital_assets.md) should be depreciated for straight line depreciation, otherwise 0 for backwards compatibility while the field is deprecated. Only relevant for capital asset purchase. | String  |
|          | capital_asset           | A link to the [asset](capital_assets.md) purchased with this bill item. Read-only. See [depreciation profiles](depreciation_profiles.md) for more details on what to include in this field for create/update requests. Only relevant for capital asset purchase.                                                                                                                                     | URI     |
|          | project                 | The [project](projects.md) being billed                                                                                                                                                                                                                                                                                                                                                              | URI     |
|          | cis_deduction_rate      | The CIS deduction rate of the bill item, taken from the rate of the contact when the bill was created, as a decimal number This attribute will be present if the bill item has a CIS for Contractors `category` (using codes 096-099)                                                                                                                                                                | Decimal |
| ✔        | url                     | URL to identify the bill item to update                                                                                                                                                                                                                                                                                                                                                              | URI     |
| ✔        | url                     | URL to identify the bill item to delete                                                                                                                                                                                                                                                                                                                                                              | URI     |
| ✔        | _destroy                | Should be equal to `1`                                                                                                                                                                                                                                                                                                                                                                               | Integer |

## List all bills

```http
GET https://api.freeagent.com/v2/bills
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/bills?view=open
```

- `all`: (default)
- `open`: Show only open bills.
- `overdue`: Show only overdue bills.
- `open_or_overdue`: Show only open or overdue bills.
- `open_or_overdue_payments`: Show only open or overdue bill payments (bills with positive total value).
- `open_or_overdue_refunds`: Show only open or overdue bill refunds (bills with negative total value).
- `paid`: Show only paid bills.
- `recurring`: Show only recurring bills.
- `hire_purchase`: Show only hire purchase bills.
- `cis`: Show only bills with any CIS bill items.

#### Date Filters

```http
GET https://api.freeagent.com/v2/bills?from_date=2012-01-01&to_date=2012-03-31
```

```http
GET https://api.freeagent.com/v2/bills?updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

### Response

```http
Status: 200 OK
```

```json
{ "bills":[{
  "url":"https://api.freeagent.com/v2/bills/1",
  "contact":"https://api.freeagent.com/v2/contacts/1",
  "reference":"acsad",
  "dated_on":"2020-07-28",
  "due_on":"2020-08-27",
  "currency":"GBP",
  "total_value":"213.0",
  "net_value":"-177.5",
  "exchange_rate":"0.09702361",
  "paid_value":"164.5",
  "due_value":"13.0",
  "native_due_value":"1.27",
  "sales_tax_value":"-35.5",
  "status":"Open",
  "long_status": "Open - due in about 1 month",
  "rebill_type": "price",
  "rebill_factor": "20",
  "rebill_to_project": "https://api.freeagent.com/v2/projects/1",
  "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/1",
  "updated_at":"2020-07-28T12:43:36Z",
  "created_at":"2020-07-28T12:43:36Z",
  "attachment":
    {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
    },
  "cis_deduction_band":"cis_standard",
  "cis_deduction_rate":"0.2",
  "cis_deduction":"35.5",
  "cis_deduction_suffered":"32.9",
  "is_paid_by_hire_purchase":false
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bills type="array">
    <bill>
      <url>https://api.freeagent.com/v2/bills/1</url>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <reference>acsad</reference>
      <dated-on type="date">2020-07-28</dated-on>
      <due-on type="date">2020-08-27</due-on>
      <currency>GBP</currency>
      <total-value type="decimal">213.0</total-value>
      <net-value type="decimal">-177.5</net-value>
      <exchange-rate type="decimal">0.09702361</exchange-rate>
      <paid-value type="decimal">164.5</paid-value>
      <due-value type="decimal">13.0</due-value>
      <native-due-value type="decimal">1.27</native-due-value>
      <sales-tax-value type="decimal">-35.5</sales-tax-value>
      <sales-tax-rate type="decimal">20.0</sales-tax-rate>
      <status>Open</status>
      <long-status>Open - due in about 1 month</long-status>
      <rebill-type>price</rebill-type>
      <rebill-factor type="decimal">20</rebill-factor>
      <rebill-to-project>https://api.freeagent.com/v2/projects/1</rebill-to-project>
      <rebilled-on-invoice-item>https://api.freeagent.com/v2/invoices/1</rebilled-on-invoice-item>
      <updated-at type="datetime">2020-07-28T12:43:36Z</updated-at>
      <created-at type="datetime">2020-07-28T12:43:36Z</created-at>
      <attachment>
        <url>https://api.freeagent.com/v2/attachments/3</url>
        <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
        <content-type>image/png</content-type>
        <file-name>barcode.png</file-name>
        <file-size type="integer">7673</file-size>
      </attachment>
      <cis-deduction-band>cis_standard</cis-deduction-band>
      <cis-deduction-rate type="decimal">0.2</cis-deduction-rate>
      <cis-deduction type="decimal">35.5</cis-deduction>
      <cis-deduction-suffered type="decimal">32.9</cis-deduction-suffered>
      <is-paid-by-hire-purchase type="boolean">false</is-paid-by-hire-purchase>
    </bill>
  </bills>
</freeagent>
```
Show as JSON

## List all bills with nested bill items

You can include bill items nested into the list of bills which increases request size but
removes the need to request the bills separately to see bill item information.

```http
GET https://api.freeagent.com/v2/bills?nested_bill_items=true
```

### Response

```http
Status: 200 OK
```

```json
{ "bills":[{
  "url":"https://api.freeagent.com/v2/bills/1",
  "contact":"https://api.freeagent.com/v2/contacts/1",
  "reference":"REF 001",
  "dated_on":"2020-07-28",
  "due_on":"2020-08-27",
  "currency":"GBP",
  "total_value":"213.0",
  "net_value":"-177.5",
  "exchange_rate":"0.61342",
  "paid_value":"200.0",
  "due_value":"13.0",
  "sales_tax_value":"-35.5",
  "status":"Open",
  "long_status": "Open - due in about 1 month",
  "rebill_type": "price",
  "rebill_factor": "20",
  "rebill_to_project": "https://api.freeagent.com/v2/projects/1",
  "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/1",
  "updated_at":"2020-07-28T12:43:36Z",
  "created_at":"2020-07-28T12:43:36Z",
  "attachment":
    {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
    },
  "bill_items":
    [
      {
        "url":"https://api.freeagent.com/v2/bill_items/1",
        "bill":"https://api.freeagent.com/v2/bills/1",
        "description":"Alex Gregory - Bill REF 001",
        "category":"https://api.freeagent.com/v2/categories/609-1",
        "quantity":"1.0",
        "unit":"Stock",
        "total_value":"213.0",
        "total_value_ex_tax":"177.5",
        "sales_tax_status":"TAXABLE",
        "second_sales_tax_status":"TAXABLE",
        "sales_tax_rate":"20.0",
        "sales_tax_value":"-35.5",
        "second_sales_tax_rate":"0.0",
        "second_sales_tax_value":"0.0",
        "stock_item":"https://api.freeagent.com/v2/stock_item/42"
      }
    ],
  "is_paid_by_hire_purchase":false
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bills type="array">
    <bill>
      <url>https://api.freeagent.com/v2/bills/1</url>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <reference>REF100</reference>
      <dated-on type="date">2020-07-28</dated-on>
      <due-on type="date">2020-08-27</due-on>
      <currency>GBP</currency>
      <total-value type="decimal">213.0</total-value>
      <net-value type="decimal">-177.5</net-value>
      <exchange-rate type="decimal">0.61342</exchange-rate>
      <paid-value type="decimal">200.0</paid-value>
      <due-value type="decimal">13.0</due-value>
      <sales-tax-value type="decimal">-35.5</sales-tax-value>
      <sales-tax-rate type="decimal">20.0</sales-tax-rate>
      <status>Open</status>
      <long-status>Open - due in about 1 month</long-status>
      <rebill-type>price</rebill-type>
      <rebill-factor type="decimal">20</rebill-factor>
      <rebill-to-project>https://api.freeagent.com/v2/projects/1</rebill-to-project>
      <rebilled-on-invoice-item>https://api.freeagent.com/v2/invoices/1</rebilled-on-invoice-item>
      <updated-at type="datetime">2020-07-28T12:43:36Z</updated-at>
      <created-at type="datetime">2020-07-28T12:43:36Z</created-at>
      <attachment>
        <url>https://api.freeagent.com/v2/attachments/3</url>
        <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
        <content-type>image/png</content-type>
        <file-name>barcode.png</file-name>
        <file-size type="integer">7673</file-size>
      </attachment>
      <bill-items type="array">
        <bill-item>
          <url>https://api.freeagent.com/v2/bill_items/1</url>
          <bill>https://api.freeagent.com/v2/bills/1</bill>
          <category>https://api.freeagent.com/v2/categories/609-1</category>
          <description>Alex Gregory - Bill REF100</description>
          <quantity type="decimal">1.0</quantity>
          <sales-tax-rate type="decimal">20.0</sales-tax-rate>
          <sales-tax-status>TAXABLE</sales-tax-status>
          <sales-tax-value type="decimal">-35.5</sales-tax-value>
          <second-sales-tax-rate type="decimal">0.0</second-sales-tax-rate>
          <second-sales-tax-status>TAXABLE</second-sales-tax-status>
          <second-sales-tax-value type="decimal">0.0</second-sales-tax-value>
          <stock-item>https://api.freeagent.com/v2/stock_item/42</stock-item>
          <total-value type="decimal">213.0</total-value>
          <total-value-ex-tax type="decimal">177.5</total-value-ex-tax>
          <unit>Stock</unit>
        </bill-item>
      </bill-items>
      <is-paid-by-hire-purchase type="boolean">false</is-paid-by-hire-purchase>
    </bill>
  </bills>
</freeagent>
```
Show as JSON

## Get a single bill

```http
GET https://api.freeagent.com/v2/bills/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "bill":{
  "url":"https://api.freeagent.com/v2/bills/1",
  "contact":"https://api.freeagent.com/v2/contacts/1",
  "reference":"REF100",
  "dated_on":"2020-09-14",
  "due_on":"2020-10-14",
  "currency":"GBP",
  "total_value":"100.0",
  "net_value":"-83.33",
  "exchange_rate":"0.673193",
  "paid_value":"80.0",
  "due_value":"20.0",
  "sales_tax_value":"-16.67",
  "status":"Open",
  "long_status":"Open - due in about 1 month",
  "rebill_type": "price",
  "rebill_factor": "20",
  "rebill_to_project": "https://api.freeagent.com/v2/projects/1",
  "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/1",
  "updated_at":"2020-09-14T16:00:41Z",
  "created_at":"2020-09-14T16:00:41Z",
  "attachment":
    {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
    },
    "bill_items":
    [
      {
        "url":"https://api.freeagent.com/v2/bill_items/1",
        "bill":"https://api.freeagent.com/v2/bills/1",
        "description":"Alex Gregory - Bill REF100",
        "category":"https://api.freeagent.com/v2/categories/609-1",
        "quantity":"1.0",
        "unit":"Stock",
        "total_value":"100.0",
        "total_value_ex_tax":"83.33",
        "sales_tax_status":"TAXABLE",
        "second_sales_tax_status":"TAXABLE",
        "sales_tax_rate":"20.0",
        "sales_tax_value":"-16.67",
        "second_sales_tax_rate":"0.0",
        "second_sales_tax_value":"0.0",
        "stock_item":"https://api.freeagent.com/v2/stock_item/42"
      }
    ],
  "is_paid_by_hire_purchase":false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bill>
    <url>https://api.freeagent.com/v2/bills/1</url>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <reference>REF100</reference>
    <dated-on type="date">2020-09-14</dated-on>
    <due-on type="date">2020-10-14</due-on>
    <currency>GBP</currency>
    <total-value type="decimal">100.0</total-value>
    <net-value type="decimal">-83.33</net-value>
    <paid-value type="decimal">80.0<paid-value>
    <exchange-rate type="decimal">0.67319</exchange-rate>
    <due-value type="decimal">20.0</due-value>
    <sales-tax-value type="decimal">-16.67</sales-tax-value>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <status>Open</status>
    <long-status>Open - due in about 1 month</long-status>
    <rebill-type>price</rebill-type>
    <rebill-factor type="decimal">20</rebill-factor>
    <rebill-to-project>https://api.freeagent.com/v2/projects/1</rebill-to-project>
    <rebilled-on-invoice-item>https://api.freeagent.com/v2/invoices/1</rebilled-on-invoice-item>
    <updated-at type="datetime">2020-09-14T16:00:41Z</updated-at>
    <created-at type="datetime">2020-09-14T16:00:41Z</created-at>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
    <bill-items type="array">
      <bill-item>
        <url>https://api.freeagent.com/v2/bill_items/1</url>
        <bill>https://api.freeagent.com/v2/bills/1</bill>
        <category>https://api.freeagent.com/v2/categories/609-1</category>
        <description>Alex Gregory - Bill REF100</description>
        <quantity type="decimal">1.0</quantity>
        <sales-tax-rate type="decimal">20.0</sales-tax-rate>
        <sales-tax-status>TAXABLE</sales-tax-status>
        <sales-tax-value type="decimal">-16.67</sales-tax-value>
        <second-sales-tax-rate type="decimal">0.0</second-sales-tax-rate>
        <second-sales-tax-status>TAXABLE</second-sales-tax-status>
        <second-sales-tax-value type="decimal">0.0</second-sales-tax-value>
        <stock-item>https://api.freeagent.com/v2/stock_item/42</stock-item>
        <total-value type="decimal">100.0</total-value>
        <total-value-ex-tax type="decimal">83.33</total-value-ex-tax>
        <unit>Stock</unit>
      </bill-item>
    </bill-items>
    <is-paid-by-hire-purchase type="boolean">false</is-paid-by-hire-purchase>
  </bill>
</freeagent>
```
Show as JSON

## List all bills related to a contact

```http
GET https://api.freeagent.com/v2/bills?contact=https://api.freeagent.com/v2/contacts/2
```

## List all bills related to a project

```http
GET https://api.freeagent.com/v2/bills?project=https://api.freeagent.com/v2/projects/2
```

## Create a bill

```http
POST https://api.freeagent.com/v2/bills
```

Payload must have a root `bill` element, containing elements listed
under Attributes.

### Example Request Body

```json
{
  "bill": {
    "contact": "https://api.freeagent.com/v2/contacts/1",
    "reference": "REF100",
    "dated_on": "2020-09-14",
    "due_on": "2020-10-14",
    "bill_items": [
      {
        "category": "https://api.freeagent.com/v2/categories/609",
        "description": "Alex Gregory - Bill REF100",
        "sales_tax_rate": "20.0",
        "stock_altering_quantity": "1.0",
        "stock_item": "https://api.freeagent.com/v2/stock_item/42",
        "total_value": "100.0"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<bill>
  <contact>https://api.freeagent.com/v2/contacts/1</contact>
  <reference>REF100</reference>
  <dated-on type="date">2020-09-14</dated-on>
  <due-on type="date">2020-10-14</due-on>
  <bill-items type="array">
    <bill-item>
      <category>https://api.freeagent.com/v2/categories/609</category>
      <description>Alex Gregory - Bill REF100</description>
      <sales-tax-rate type="decimal">20.0</sales-tax-rate>
      <stock-altering-quantity type="decimal">1.0</stock-altering-quantity>
      <stock-item>https://api.freeagent.com/v2/stock_item/42</stock-item>
      <total-value type="decimal">100.0</total-value>
    </bill-item>
  </bill-items>
</bill>
```
Show as JSON

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/bills/12
```

```json
{ "bill":{
  "url":"https://api.freeagent.com/v2/bills/12",
  "contact":"https://api.freeagent.com/v2/contacts/1",
  "reference":"REF100",
  "dated_on":"2020-09-14",
  "due_on":"2020-10-14",
  "currency":"GBP",
  "total_value":"100.0",
  "net_value":"83.33",
  "exchange_rate":"0.67319",
  "paid_value":"80.0",
  "due_value":"20.0",
  "sales_tax_value":"-16.67",
  "status":"Open",
  "long_status":"Open - due in about 1 month",
  "rebill_type": "price",
  "rebill_factor": "20",
  "rebill_to_project": "https://api.freeagent.com/v2/projects/1",
  "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/1",
  "updated_at":"2020-09-14T16:00:41Z",
  "created_at":"2020-09-14T16:00:41Z",
  "attachment":
    {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
    },
  "bill_items":
    [
      {
        "url":"https://api.freeagent.com/v2/bill_items/1",
        "bill":"https://api.freeagent.com/v2/bills/1",
        "description":"Alex Gregory - Bill REF100",
        "category":"https://api.freeagent.com/v2/categories/609-1",
        "quantity":"1.0",
        "unit":"Stock",
        "total_value":"100.0",
        "total_value_ex_tax":"83.33",
        "sales_tax_status":"TAXABLE",
        "second_sales_tax_status":"TAXABLE",
        "sales_tax_rate":"20.0",
        "sales_tax_value":"-16.67",
        "second_sales_tax_rate":"0.0",
        "second_sales_tax_value":"0.0",
        "stock_item":"https://api.freeagent.com/v2/stock_item/42"
      }
    ],
  "is_paid_by_hire_purchase":false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bill>
    <url>https://api.freeagent.com/v2/bills/12</url>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <reference>REF100</reference>
    <dated-on type="date">2020-09-14</dated-on>
    <due-on type="date">2020-10-14</due-on>
    <currency>GBP</currency>
    <total-value type="decimal">100.0</total-value>
    <net-value type="decimal">83.33</net-value>
    <exchange-rate>0.93183</exchanage-rate>
    <paid-value type="decimal">80.0</paid-value>
    <due-value type="decimal">20.0</due-value>
    <sales-tax-value type="decimal">-16.67</sales-tax-value>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <status>Open</status>
    <long-status>Open - due in about 1 month</long-status>
    <rebill-type>price</rebill-type>
    <rebill-factor type="decimal">20</rebill-factor>
    <rebill-to-project>https://api.freeagent.com/v2/projects/1</rebill-to-project>
    <rebilled-on-invoice-item>https://api.freeagent.com/v2/invoices/1</rebilled-on-invoice-item>
    <updated-at type="datetime">2020-09-14T16:00:41Z</updated-at>
    <created-at type="datetime">2020-09-14T16:00:41Z</created-at>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
    <bill-items type="array">
      <bill-item>
        <url>https://api.freeagent.com/v2/bill_items/1</url>
        <bill>https://api.freeagent.com/v2/bills/1</bill>
        <category>https://api.freeagent.com/v2/categories/609-1</category>
        <description>Alex Gregory - Bill REF100</description>
        <quantity type="decimal">1.0</quantity>
        <sales-tax-rate type="decimal">20.0</sales-tax-rate>
        <sales-tax-status>TAXABLE</sales-tax-status>
        <sales-tax-value type="decimal">-16.67</sales-tax-value>
        <second-sales-tax-rate type="decimal">0.0</second-sales-tax-rate>
        <second-sales-tax-status>TAXABLE</second-sales-tax-status>
        <second-sales-tax-value type="decimal">0.0</second_sales-tax-value>
        <stock-item>https://api.freeagent.com/v2/stock_item/42</stock-item>
        <total-value type="decimal">100.0</total-value>
        <total-value-ex-tax type="decimal">83.33</total-value-ex-tax>
        <unit>Stock</unit>
      </bill-item>
    </bill-items>
    <is-paid-by-hire-purchase type="boolean">false</is-paid-by-hire-purchase>
  </bill>
</freeagent>
```
Show as JSON

## Update a bill

```http
PUT https://api.freeagent.com/v2/bills/:id
```

Payload must have a root `bill` element, containing elements listed
under Attributes that should be updated.

Bill items must have a `url` attribute, either set to the URL identifier of
an existing bill item to update or delete or to an empty string to create a new
bill item.

### Example Single Item Request Body

This example updates the bill's reference and updates the description & total
value on the bill item.

```json
{
  "bill": {
    "reference": "REF100",
    "bill_items": [
      {
        "url": "https://api.freeagent.com/v2/bill_items/100001",
        "description": "Updating an existing bill item",
        "total_value": "172.50"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<bill>
  <reference>REF100</reference>
  <bill-items type="array">
    <bill-item>
      <url>https://api.freeagent.com/v2/bill_items/100001</url>
      <description>Updating an existing bill item</description>
      <total-value type="decimal">172.50</total-value>
    </bill-item>
  </bill-items>
</bill>
```
Show as JSON

### Example Multi-Item Request Body

This example updates the bill's reference, updates the description & total
value on one bill item, deletes a second bill item and creates a third bill
item.

```json
{
  "bill": {
    "reference": "REF100",
    "bill_items": [
      {
        "url": "https://api.freeagent.com/v2/bill_items/100001",
        "description": "Updating an existing bill item",
        "total_value": "172.50"
      },
      {
        "url": "https://api.freeagent.com/v2/bill_items/200002",
        "_destroy": 1
      },
      {
        "url": "",
        "category": "https://api.freeagent.com/v2/categories/285",
        "description": "New bill item for accommodation fees",
        "total_value": "200.00"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<bill>
  <reference>REF100</reference>
  <bill-items type="array">
    <bill-item>
      <url>https://api.freeagent.com/v2/bill_items/100001</url>
      <description>Updating an existing bill item</description>
      <total-value type="decimal">172.50</total-value>
    </bill-item>
    <bill-item>
      <url>https://api.freeagent.com/v2/bill_items/200002</url>
      <_destroy type="integer">1</_destroy>
    </bill-item>
    <bill-item>
      <url></url>
      <category>https://api.freeagent.com/v2/categories/285</category>
      <description>New bill item for accommodation fees</description>
      <total-value type="decimal">200.0</total-value>
    </bill-item>
  </bill-items>
</bill>
```
Show as JSON

### Response

```http
Status: 200 OK
```

## Delete a bill

```http
DELETE https://api.freeagent.com/v2/bills/:id
```

### Response

```http
Status: 200 OK
```