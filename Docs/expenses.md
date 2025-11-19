# Expenses

*Minimum access level*: `My Money`, unless stated otherwise.

## Attributes

| Required | Attribute               | Description                                                                                                                                                                                                                                                                                                                                                                                                                                         | Kind         |
| -------- | ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------ |
|          | url                     | The unique identifier for the expense                                                                                                                                                                                                                                                                                                                                                                                                               | URI          |
| ✔        | user                    | Expense [claimant](users.md)                                                                                                                                                                                                                                                                                                                                                                                                                        | URI          |
| ✔        | category                | One of the [accounting categories](categories.md), or `Mileage`                                                                                                                                                                                                                                                                                                                                                                                     | URL / String |
| ✔        | dated_on                | Date of expense in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                              | Date         |
|          | currency                | Expense's [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                                                                                 | String       |
| ?        | gross_value             | Total value expressed in the given `currency`. When a negative value is given, this will be a payment to the claimant. When a positive value is given, this will be a refund due from the claimant. Required if `category` is not `Mileage`                                                                                                                                                                                                         | Decimal      |
|          | native_gross_value      | Total value in the company's native [currency](currencies.md) Automatically converted from `gross_value` unless specified                                                                                                                                                                                                                                                                                                                           | Decimal      |
|          | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates                                                                                                                                                                                                                                                                                                                                                                                                 | Decimal      |
|          | sales_tax_value         | Total value of [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                                                            | Decimal      |
|          | native_sales_tax_value  | Total value of [sales tax](sales_tax.md) in the company's native currency                                                                                                                                                                                                                                                                                                                                                                           | Decimal      |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                                                                                                                                                                                                                                                                                                | Decimal      |
|          | manual_sales_tax_amount | Amount of [sales tax](sales_tax.md) for the transaction, in the company's native currency                                                                                                                                                                                                                                                                                                                                                           | Decimal      |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE`, `EXEMPT` or `OUT_OF_SCOPE` for [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                   | String       |
|          | second_sales_tax_status | [Universal accounts only] Similar to sales_tax_status, returned only if the relevant [sales tax period](sales_tax_periods.md) defines a second sales tax                                                                                                                                                                                                                                                                                            | String       |
|          | ec_status               | Expense's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge` Please note that `EC Goods` and `EC Services` are no longer valid options if the expense is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the expense is dated 1/1/2021 or later. | String       |
| ✔        | description             | Free-text description                                                                                                                                                                                                                                                                                                                                                                                                                               | String       |
|          | receipt_reference       | Receipt reference                                                                                                                                                                                                                                                                                                                                                                                                                                   | String       |
| ?        | stock_item              | [Stock item](stock_items.md) being purchased, required when category is set to Purchase of stock                                                                                                                                                                                                                                                                                                                                                    | URI          |
|          | stock_item_description  | Description of the expense's stock item, only returned when stock item is set                                                                                                                                                                                                                                                                                                                                                                       | String       |
| ?        | stock_altering_quantity | Quantity of `stock_item` units purchased, required when category is set to Purchase of stock                                                                                                                                                                                                                                                                                                                                                        | Decimal      |
|          | project                 | [Project](projects.md) to rebill                                                                                                                                                                                                                                                                                                                                                                                                                    | URI          |
|          | rebill_type             | One of the following, if rebilling a [project](projects.md): `cost`, `markup`, `price`                                                                                                                                                                                                                                                                                                                                                              | String       |
|          | rebill_factor           | How much to rebill for Required when `rebill_type` is `markup` or `price`                                                                                                                                                                                                                                                                                                                                                                           | Decimal      |
|          | rebill_to_project       | Same as `project`                                                                                                                                                                                                                                                                                                                                                                                                                                   | URI          |
|          | rebilled_on_invoice     | [Invoice](invoices.md) where the expense has been rebilled as an invoice item                                                                                                                                                                                                                                                                                                                                                                       | URI          |
| ?        | property                | The [property](properties.md) pertaining to this expense. Only accepted and required for companies with type `UkUnincorporatedLandlord`, and only if the expense is not for a capital asset purchase.                                                                                                                                                                                                                                               | URI          |
|          | recurring               | Frequency at which the expense will recur. Can be one of the following: `Weekly`, `Two Weekly`, `Four Weekly`, `Two Monthly`, `Quarterly`, `Biannually`, `Annually`, `2-Yearly`                                                                                                                                                                                                                                                                     | String       |
|          | next_recurs_on          | When expense recurs next, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                    | Date         |
|          | recurring_end_date      | When expense stops recurring, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                | Date         |
|          | attachment              | New explanation attachment (max 5MB), in the following format: `data` (binary data of the file being attached encoded as base64), `file_name`, `description`, `content_type` can be one of the following: `image/png` `image/x-png` `image/jpeg` `image/jpg` `image/gif` `application/x-pdf`, `image/png`, `image/x-png`, `image/jpeg`, `image/jpg`, `image/gif`, `application/x-pdf` To link an existing attachment, you can use `file` instead.   | Object       |
|          | created_at              | Creation of the expense resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                              | Timestamp    |
|          | updated_at              | When the expense resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                                    | Timestamp    |
| ✔        | mileage                 | Number of miles travelled                                                                                                                                                                                                                                                                                                                                                                                                                           | Decimal      |
| ✔        | vehicle_type            | One of the following: `Car`, `Motorcycle`, `Bicycle`                                                                                                                                                                                                                                                                                                                                                                                                | String       |
| ?        | engine_type             | Applicable if `vehicle_type` is `Car` or `Motorcycle`: `Petrol` (default), `Diesel`, `LPG`, `Electric`, `Electric (Home charger)`, `Electric (Public charger)` The valid electric options change over time. The electric options available on your mileage claim date are defined in `engine_type_and_size_options` from [mileage_settings](expenses.md#get-mileage-settings).                                                                      | String       |
| ?        | engine_size             | Applicable if `vehicle_type` is `Car` or `Motorcycle`. For valid engine sizes, see `engine_type_and_size_options` from [mileage_settings](expenses.md#get-mileage-settings). Defaults to the first option for selected `engine_type`.                                                                                                                                                                                                               | String       |
|          | reclaim_mileage         | One of the following: `0` (Default - don't reclaim, just rebill), `1` (At Approved Mileage Allowance Payments (AMAP) rate)                                                                                                                                                                                                                                                                                                                          | Integer      |
|          | initial_rate_mileage    | Rate at which mileage is being reclaimed if under the [HMRC threshold](https://www.freeagent.com/glossary/mileage-claim)                                                                                                                                                                                                                                                                                                                            | Decimal      |
|          | reclaim_mileage_rate    | Rate at which mileage is being reclaimed                                                                                                                                                                                                                                                                                                                                                                                                            | Decimal      |
|          | rebill_mileage_rate     | Rate at which mileage is being rebilled                                                                                                                                                                                                                                                                                                                                                                                                             | Decimal      |
|          | have_vat_receipt        | `true` if having a VAT receipt when reclaiming, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                   | Boolean      |
|          | capital_asset           | A link to the [asset](capital_assets.md) purchased with this expense. Read-only. See [depreciation profiles](depreciation_profiles.md) for more details on what to include in this field for create/update requests.                                                                                                                                                                                                                                | URI          |
|          | depreciation_schedule   | Note! This field is deprecated. Fetch the capital asset using the link in the capital_asset field of the response to view full details of its depreciation profile. Number of years over which the [asset](capital_assets.md) should be depreciated for straight line depreciation, otherwise 0 for backwards compatibility while the field is deprecated.                                                                                          | String       |

## List all expenses

```http
GET https://api.freeagent.com/v2/expenses
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/expenses?view=recent
```

- `recent`: Show only recent expenses.
- `recurring`: Show recurring expenses.

#### Date Filters

```http
GET https://api.freeagent.com/v2/expenses?from_date=2012-01-01&to_date=2012-03-31
```

```http
GET https://api.freeagent.com/v2/expenses?updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

#### Project Filters

```http
GET https://api.freeagent.com/v2/expenses?project=https://api.freeagent.com/v2/projects/2
```

- `project`: show only expenses related to this project

### Response

```http
Status: 200 OK
```

```json
{ "expenses":[
  {
    "url":"https://api.freeagent.com/v2/expenses/1",
    "user":"https://api.freeagent.com/v2/users/1",
    "category":"https://api.freeagent.com/v2/categories/285",
    "dated_on":"2011-08-24",
    "currency":"USD",
    "gross_value":"-20.0",
    "native_gross_value":"-12.0",
    "sales_tax_rate":"1.0",
    "sales_tax_value": "-0.2",
    "native_sales_tax_value": "-0.12",
    "sales_tax_status": "TAXABLE",
    "description":"Some description",
    "rebilled_on_invoice": "https://api.freeagent.com/v2/invoices/1",
    "manual_sales_tax_amount":"0.12",
    "updated_at":"2011-08-24T08:10:40Z",
    "created_at":"2011-08-24T08:10:40Z",
    "attachment":
      {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
      }
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <expenses type="array">
    <expense>
      <url>https://api.freeagent.com/v2/expenses/1</url>
      <user>https://api.freeagent.com/v2/users/1</user>
      <category>https://api.freeagent.com/v2/categories/285</category>
      <dated-on type="date">2011-08-24</dated-on>
      <currency>USD</currency>
      <gross-value type="decimal">-20.0</gross-value>
      <native-gross-value type="decimal">-12.0</native-gross-value>
      <sales-tax-rate type="decimal">20.0</sales-tax-rate>
      <description>asd</description>
      <rebilled_on_invoice>https://api.freeagent.com/v2/invoices/1</rebilled_on_invoice>
      <manual-sales-tax-amount type="decimal">0.12</manual-sales-tax-amount>
      <updated-at type="datetime">2011-08-24T08:10:40Z</updated-at>
      <created-at type="datetime">2011-08-24T08:10:40Z</created-at>
      <attachment>
        <url>https://api.freeagent.com/v2/attachments/3</url>
        <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
        <content-type>image/png</content-type>
        <file-name>barcode.png</file-name>
        <file-size type="integer">7673</file-size>
      </attachment>
    </expense>
  </expenses>
</freeagent>
```
Show as JSON

## Get a single expense

```http
GET https://api.freeagent.com/v2/expenses/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "expense":
  {
    "user":"https://api.freeagent.com/v2/users/1",
    "category":"https://api.freeagent.com/v2/categories/285",
    "dated_on":"2011-08-24",
    "currency":"USD",
    "gross_value":"-20.0",
    "native_gross_value":"-12.0",
    "sales_tax_rate":"1.0",
    "sales_tax_value": "-0.2",
    "native_sales_tax_value": "-0.12",
    "sales_tax_status": "TAXABLE",
    "description":"Some description",
    "rebilled_on_invoice": "https://api.freeagent.com/v2/invoices/1",
    "manual_sales_tax_amount":"0.12",
    "updated_at":"2011-08-24T08:10:40Z",
    "created_at":"2011-08-24T08:10:40Z",
    "attachment":
      {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
      }
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <expense>
    <user>https://api.freeagent.com/v2/users/1</user>
    <category>https://api.freeagent.com/v2/categories/285</category>
    <dated-on type="date">2011-08-24</dated-on>
    <currency>USD</currency>
    <gross-value type="decimal">-20.0</gross-value>
    <native-gross-value type="decimal">-12.0</native-gross-value>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <description>asd</description>
    <rebilled_on_invoice>https://api.freeagent.com/v2/invoices/1</rebilled_on_invoice>
    <manual-sales-tax-amount type="decimal">0.12</manual-sales-tax-amount>
    <updated-at type="datetime">2011-08-24T08:10:40Z</updated-at>
    <created-at type="datetime">2011-08-24T08:10:40Z</created-at>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
  </expense>
</freeagent>
```
Show as JSON

## Create an expense

```http
POST https://api.freeagent.com/v2/expenses
```

Payload should have a root `expense` element, containing elements listed
under Attributes.

**Note:** Generally, sales tax on expenses incurred in a foreign currency will not be reclaimable and should be omitted. If `sales_tax_rate` is specified on a foreign currency expense, it will be ignored and the rate set to zero. If you can reclaim sales tax from HMRC/your local government agency, then the amount reclaimable should be entered in your native currency using the `manual_sales_tax_amount` field.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/expenses/4
```

```json
{ "expense":
  {
    "user":"https://api.freeagent.com/v2/users/1",
    "category":"https://api.freeagent.com/v2/categories/285",
    "dated_on":"2011-08-24",
    "currency":"USD",
    "gross_value":"-20.0",
    "native_gross_value":"-12.0",
    "sales_tax_rate":"1.0",
    "sales_tax_value": "-0.2",
    "native_sales_tax_value": "-0.12",
    "sales_tax_status": "TAXABLE",
    "description":"Some description",
    "manual_sales_tax_amount":"0.12",
    "updated_at":"2011-08-24T08:10:40Z",
    "created_at":"2011-08-24T08:10:40Z",
    "attachment":
      {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673
      }
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <expense>
    <user>https://api.freeagent.com/v2/users/1</user>
    <category>https://api.freeagent.com/v2/categories/285</category>
    <dated-on type="date">2011-08-24</dated-on>
    <currency>USD</currency>
    <gross-value type="decimal">-20.0</gross-value>
    <native-gross-value type="decimal">-12.0</native-gross-value>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <description>asd</description>
    <manual-sales-tax-amount type="decimal">0.12</manual-sales-tax-amount>
    <updated-at type="datetime">2011-08-24T08:10:40Z</updated-at>
    <created-at type="datetime">2011-08-24T08:10:40Z</created-at>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
  </expense>
</freeagent>
```
Show as JSON

### Batch create

You can also post an array of expenses inside an `expenses` tag:

```json
{ "expenses":
  [{
    "user":"https://api.freeagent.com/v2/users/1",
    "category":"https://api.freeagent.com/v2/categories/285",
    "dated_on":"2011-08-24",
    "gross_value":"-12.0",
    "sales_tax_rate":"20.0",
    "description":"Some description",
    "manual_sales_tax_amount":"0.12",
    "updated_at":"2011-08-24T08:10:40Z",
    "created_at":"2011-08-24T08:10:40Z"
  },
  {
    "user":"https://api.freeagent.com/v2/users/1",
    "category":"https://api.freeagent.com/v2/categories/285",
    "dated_on":"2011-08-24",
    "gross_value":"-5.0",
    "sales_tax_rate":"20.0",
    "description":"Some description",
    "manual_sales_tax_amount":"0.12",
    "updated_at":"2011-08-25T08:10:40Z",
    "created_at":"2011-08-25T08:10:40Z"
  }]
}
```
Show as XML

## Update an expense

```http
PUT https://api.freeagent.com/v2/expenses/:id
```

Payload should have a root `expense` element, containing elements listed
under Attributes that should be updated.

**Note:** Generally, sales tax on expenses incurred in a foreign currency will not be reclaimable and should be omitted. If `sales_tax_rate` is specified on a foreign currency expense, it will be ignored and the rate set to zero. If you can reclaim sales tax from HMRC/your local government agency, then the amount reclaimable should be entered in your native currency using the `manual_sales_tax_amount` field.

### Response

```http
Status: 200 OK
```

## Delete an expense

```http
DELETE https://api.freeagent.com/v2/expenses/:id
```

### Response

```http
Status: 200 OK
```

## Get mileage settings

```http
GET https://api.freeagent.com/v2/expenses/mileage_settings
```

### Response

```http
Status: 200 OK
```

```json
{
  "mileage_settings":{
    "engine_type_and_size_options":[
      {
        "from":"1970-01-01",
        "to":"2011-05-31",
        "value":{
          "Petrol":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Diesel":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "LPG":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ]
        }
      },
      {
        "from":"2011-06-01",
        "to":"2019-02-28",
        "value":{
          "Petrol":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Diesel":[
            "Up to 1600cc",
            "1601-2000cc",
            "Over 2000cc"
          ],
          "LPG":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ]
        }
      },
      {
        "from":"2019-03-01",
        "to":"2025-08-31",
        "value":{
          "Petrol":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Diesel":[
            "Up to 1600cc",
            "1601-2000cc",
            "Over 2000cc"
          ],
          "LPG":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Electric":[
            "All"
          ]
        }
      },
      {
        "from":"2025-09-01",
        "to":"2099-12-31",
        "value":{
          "Petrol":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Diesel":[
            "Up to 1600cc",
            "1601-2000cc",
            "Over 2000cc"
          ],
          "LPG":[
            "Up to 1400cc",
            "1401-2000cc",
            "Over 2000cc"
          ],
          "Electric (Home charger)":[
            "All"
          ],
          "Electric (Public charger)":[
            "All"
          ]
        }
      }
    ],
    "mileage_rates":[
      {
        "from":"1970-01-01",
        "to":"2011-04-05",
        "value":{
          "Car":{
            "basic_rate":"0.4",
            "additional_rate":"0.25"
          },
          "Motorcycle":{
            "basic_rate":"0.24",
            "additional_rate":"0.24"
          },
          "Bicycle":{
            "basic_rate":"0.2",
            "additional_rate":"0.2"
          },
          "basic_rate_limit":10000
        }
      },
      {
        "from":"2011-04-06",
        "to":"2099-12-31",
        "value":{
          "Car":{
            "basic_rate":"0.45",
            "additional_rate":"0.25"
          },
          "Motorcycle":{
            "basic_rate":"0.24",
            "additional_rate":"0.24"
          },
          "Bicycle":{
            "basic_rate":"0.2",
            "additional_rate":"0.2"
          },
          "basic_rate_limit":10000
        }
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <mileage-settings>
    <engine-type-and-size-options type="array">
      <engine-type-and-size-option>
        <from type="date">1970-01-01</from>
        <to type="date">2011-05-31</to>
        <value>
          <Petrol type="array">
            <Petrol>Up to 1400cc</Petrol>
            <Petrol>1401-2000cc</Petrol>
            <Petrol>Over 2000cc</Petrol>
          </Petrol>
          <Diesel type="array">
            <Diesel>Up to 1400cc</Diesel>
            <Diesel>1401-2000cc</Diesel>
            <Diesel>Over 2000cc</Diesel>
          </Diesel>
          <LPG type="array">
            <LPG>Up to 1400cc</LPG>
            <LPG>1401-2000cc</LPG>
            <LPG>Over 2000cc</LPG>
          </LPG>
        </value>
      </engine-type-and-size-option>
      <engine-type-and-size-option>
        <from type="date">2011-06-01</from>
        <to type="date">2019-02-28</to>
        <value>
          <Petrol type="array">
            <Petrol>Up to 1400cc</Petrol>
            <Petrol>1401-2000cc</Petrol>
            <Petrol>Over 2000cc</Petrol>
          </Petrol>
          <Diesel type="array">
            <Diesel>Up to 1600cc</Diesel>
            <Diesel>1601-2000cc</Diesel>
            <Diesel>Over 2000cc</Diesel>
          </Diesel>
          <LPG type="array">
            <LPG>Up to 1400cc</LPG>
            <LPG>1401-2000cc</LPG>
            <LPG>Over 2000cc</LPG>
          </LPG>
        </value>
      </engine-type-and-size-option>
      <engine-type-and-size-option>
        <from type="date">2019-03-01</from>
        <to type="date">2025-08-31</to>
        <value>
          <Petrol type="array">
            <Petrol>Up to 1400cc</Petrol>
            <Petrol>1401-2000cc</Petrol>
            <Petrol>Over 2000cc</Petrol>
          </Petrol>
          <Diesel type="array">
            <Diesel>Up to 1600cc</Diesel>
            <Diesel>1601-2000cc</Diesel>
            <Diesel>Over 2000cc</Diesel>
          </Diesel>
          <LPG type="array">
            <LPG>Up to 1400cc</LPG>
            <LPG>1401-2000cc</LPG>
            <LPG>Over 2000cc</LPG>
          </LPG>
          <Electric type="array">
            <Electric>All</Electric>
          </Electric>
        </value>
      </engine-type-and-size-option>
      <engine-type-and-size-option>
        <from type="date">2025-09-01</from>
        <to type="date">2099-12-31</to>
        <value>
          <Petrol type="array">
            <Petrol>Up to 1400cc</Petrol>
            <Petrol>1401-2000cc</Petrol>
            <Petrol>Over 2000cc</Petrol>
          </Petrol>
          <Diesel type="array">
            <Diesel>Up to 1600cc</Diesel>
            <Diesel>1601-2000cc</Diesel>
            <Diesel>Over 2000cc</Diesel>
          </Diesel>
          <LPG type="array">
            <LPG>Up to 1400cc</LPG>
            <LPG>1401-2000cc</LPG>
            <LPG>Over 2000cc</LPG>
          </LPG>
          <ElectricHomeCharger type="array">
            <ElectricHomeCharger>All</ElectricHomeCharger>
          </ElectricHomeCharger>
          <ElectricPublicCharger type="array">
            <ElectricPublicCharger>All</ElectricPublicCharger>
          </ElectricPublicCharger>
        </value>
      </engine-type-and-size-option>
    </engine-type-and-size-options>
    <mileage-rates type="array">
      <mileage-rate>
        <from type="date">1970-01-01</from>
        <to type="date">2011-04-05</to>
        <value>
          <Car>
            <basic-rate type="decimal">0.4</basic-rate>
            <additional-rate type="decimal">0.25</additional-rate>
          </Car>
          <Motorcycle>
            <basic-rate type="decimal">0.24</basic-rate>
            <additional-rate type="decimal">0.24</additional-rate>
          </Motorcycle>
          <Bicycle>
            <basic-rate type="decimal">0.2</basic-rate>
            <additional-rate type="decimal">0.2</additional-rate>
          </Bicycle>
          <basic-rate-limit type="integer">10000</basic-rate-limit>
        </value>
      </mileage-rate>
      <mileage-rate>
        <from type="date">2011-04-06</from>
        <to type="date">2099-12-31</to>
        <value>
          <Car>
            <basic-rate type="decimal">0.45</basic-rate>
            <additional-rate type="decimal">0.25</additional-rate>
          </Car>
          <Motorcycle>
            <basic-rate type="decimal">0.24</basic-rate>
            <additional-rate type="decimal">0.24</additional-rate>
          </Motorcycle>
          <Bicycle>
            <basic-rate type="decimal">0.2</basic-rate>
            <additional-rate type="decimal">0.2</additional-rate>
          </Bicycle>
          <basic-rate-limit type="integer">10000</basic-rate-limit>
        </value>
      </mileage-rate>
    </mileage-rates>
  </mileage-settings>
</freeagent>
```
Show as JSON