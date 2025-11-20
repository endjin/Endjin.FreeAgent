# Bank Transaction Explanations

*Minimum access level*: `Banking`, unless stated otherwise.

## Attributes

| Required | Attribute               | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Kind    |
| -------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------- |
|          | url                     | The unique identifier for the explanation                                                                                                                                                                                                                                                                                                                                                                                                                                      | URI     |
| ?        | bank_account            | [Bank account](bank_accounts.md) in which the explained transaction will be created Required if `bank_transaction` is not specified                                                                                                                                                                                                                                                                                                                                            | URI     |
| ?        | bank_transaction        | [Transaction](bank_transactions.md) that is being explained Required if `bank_account` is not specified                                                                                                                                                                                                                                                                                                                                                                        | URI     |
|          | type                    | Read-only explanation type, e.g. Payment, Invoice Receipt                                                                                                                                                                                                                                                                                                                                                                                                                      | String  |
|          | ec_status               | Transaction's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge`, `EC VAT MOSS` Please note that `EC Goods` and `EC Services` are no longer valid options if the transaction is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the transaction is dated 1/1/2021 or later. | String  |
| ?        | place_of_supply         | Place of supply when `ec_status` is [EC VAT MOSS](sales_tax.md#ec-vat-moss)                                                                                                                                                                                                                                                                                                                                                                                                    | String  |
| ✔        | dated_on                | Date of the explanation                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Date    |
| ✔        | gross_value             | Value of the explanation in the Bank Account's native [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                                                                | Decimal |
| ?        | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates or an [EC VAT MOSS](sales_tax.md#ec-vat-moss-rates) rate. Required for EC VAT MOSS explanation                                                                                                                                                                                                                                                                                                                             | Decimal |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                                                                                                                                                                                                                                                                                                                           | Decimal |
|          | sales_tax_value         | Value of [sales tax](sales_tax.md) for the transaction                                                                                                                                                                                                                                                                                                                                                                                                                         | Decimal |
|          | second_sales_tax_value  | [Universal accounts only] Value of [second sales tax](sales_tax.md) for the transaction                                                                                                                                                                                                                                                                                                                                                                                        | Decimal |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE`, `EXEMPT` or `OUT_OF_SCOPE` for [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                              | String  |
|          | second_sales_tax_status | [Universal accounts only] Similar to sales_tax_status, returned only if the relevant [sales tax period](sales_tax_periods.md) defines a second sales tax                                                                                                                                                                                                                                                                                                                       | String  |
| ?        | description             | Description Not required for transfers or invoice receipts                                                                                                                                                                                                                                                                                                                                                                                                                     | String  |
|          | category                | [Accounting category](categories.md) of the explanation                                                                                                                                                                                                                                                                                                                                                                                                                        | URI     |
|          | cheque_number           | Cheque number                                                                                                                                                                                                                                                                                                                                                                                                                                                                  | String  |
|          | attachment              | Explanation [attachment](attachments.md) (max 5MB), in the following format: `data` (binary data of the file being attached encoded as base64), `file_name`, `description`, `content_type` can be one of the following: `image/png` `image/x-png` `image/jpeg` `image/jpg` `image/gif` `application/x-pdf`, `image/png`, `image/x-png`, `image/jpeg`, `image/jpg`, `image/gif`, `application/x-pdf`                                                                            | Object  |
|          | marked_for_review       | `true` if the explanation has been guessed and awaiting approval, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                            | Boolean |
|          | is_money_in             | `true` if money in, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                                                          | Boolean |
|          | is_money_out            | `true` if money out, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                                                         | Boolean |
|          | is_money_paid_to_user   | `true` if money is paid to or received from user, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                            | Boolean |
|          | is_locked               | `true` when the explanation cannot be changed, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                               | Boolean |
|          | locked_attributes       | List of attributes that cannot be modified                                                                                                                                                                                                                                                                                                                                                                                                                                     | Array   |
|          | locked_reason           | The reason for the explanation being locked                                                                                                                                                                                                                                                                                                                                                                                                                                    | String  |
|          | is_deletable            | `true` when the explanation can be deleted, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                                  | Boolean |
| ✔        | project                 | The [project](projects.md) being linked or rebilled                                                                                                                                                                                                                                                                                                                                                                                                                            | URI     |
| ✔        | rebill_type             | One of the following: `cost`, `markup`, `price`                                                                                                                                                                                                                                                                                                                                                                                                                                | String  |
| ?        | rebill_factor           | How much to rebill for Required when `rebill_type` is `markup` or `price`                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal |
|          | receipt_reference       | Receipt reference                                                                                                                                                                                                                                                                                                                                                                                                                                                              | String  |
| ✔        | paid_invoice            | [Invoice](invoices.md) that has been paid, or credit note that has been refunded                                                                                                                                                                                                                                                                                                                                                                                               | URI     |
|          | foreign_currency_value  | Equivalent of `gross_value` in the foreign currency, if explaining a foreign currency invoice                                                                                                                                                                                                                                                                                                                                                                                  | Decimal |
| ✔        | paid_bill               | [Bill](bills.md) that was paid or refunded                                                                                                                                                                                                                                                                                                                                                                                                                                     | URI     |
|          | foreign_currency_value  | Equivalent of `gross_value` in the foreign currency, if explaining a foreign currency bill                                                                                                                                                                                                                                                                                                                                                                                     | Decimal |
| ✔        | paid_user               | [User](users.md) to which money was paid or from which money was received                                                                                                                                                                                                                                                                                                                                                                                                      | URI     |
| ✔        | transfer_bank_account   | [Bank account](bank_accounts.md) transfered to / from                                                                                                                                                                                                                                                                                                                                                                                                                          | URI     |
| ✔        | stock_item              | [Stock item](stock_items.md) purchased or sold                                                                                                                                                                                                                                                                                                                                                                                                                                 | URI     |
| ✔        | stock_altering_quantity | How much stock has been purchased or sold                                                                                                                                                                                                                                                                                                                                                                                                                                      | Integer |
|          | capital_asset           | A link to the [asset](capital_assets.md) purchased with this transaction. Read-only. See [depreciation profiles](depreciation_profiles.md) for more details on what to include in this field for create/update requests.                                                                                                                                                                                                                                                       | URI     |
|          | asset_life_years        | Note! This field is deprecated. Fetch the capital asset using the link in the capital_asset field of the response to view full details of its depreciation profile. Number of years over which the [asset](capital_assets.md) should be depreciated for straight line depreciation, otherwise 0 for backwards compatibility while the field is deprecated. Only relevant for capital asset purchase.                                                                           | String  |
| ?        | disposed_asset          | [Asset](capital_assets.md) which is disposed of. No need to specify category in this case. Required for capital asset disposal                                                                                                                                                                                                                                                                                                                                                 | URI     |
| ?        | property                | The [property](properties.md) linked to the explanation                                                                                                                                                                                                                                                                                                                                                                                                                        | URI     |

## List all bank transaction explanations

Requires the bank account to be specified.

```http
GET https://api.freeagent.com/v2/bank_transaction_explanations?bank_account=https://api.freeagent.com/v2/bank_accounts/:id
```

#### Date Filters

```http
GET https://api.freeagent.com/v2/bank_transaction_explanations?bank_account=https://api.freeagent.com/v2/bank_accounts/:id&from_date=2012-01-01&to_date=2012-03-31
```

```http
GET https://api.freeagent.com/v2/bank_transaction_explanations?bank_account=https://api.freeagent.com/v2/bank_accounts/:id&updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

### Response

```http
Status: 200 OK
```

```json
{ "bank_transaction_explanations": [
  {
    "url": "https://api.freeagent.com/v2/bank_transaction_explanations/20",
    "bank_transaction": "https://api.freeagent.com/v2/bank_transactions/20",
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "category": "https://api.freeagent.com/v2/categories/366",
    "dated_on": "2019-12-01",
    "description": "transform plug-and-play convergence",
    "gross_value": "-90.0",
    "project": "https://api.freeagent.com/v2/projects/1",
    "rebill_type": "markup",
    "rebill_factor": "0.25",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "sales_tax_status": "TAXABLE",
    "sales_tax_rate": "0.0",
    "sales_tax_value": "0.0",
    "is_deletable": true,
    "attachment":
      {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/2/original.pdf?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1316186571&Signature=tA4V5%2BJEE%2Fc3JTg5AiIO494m0cA%3D",
        "content_type":"application/pdf",
        "file_name":"About Stacks.pdf",
        "file_size":466028
      }
  }]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-transaction-explanations type="array">
    <bank-transaction-explanation>
      <url>https://api.freeagent.com/v2/bank_transaction_explanations/20</url>
      <bank-transaction>https://api.freeagent.com/v2/bank_transactions/20</bank-transaction>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <category>https://api.freeagent.com/v2/categories/366</category>
      <dated-on type="date">2019-12-01</dated-on>
      <description>transform plug-and-play convergence</description>
      <gross-value type="decimal">-90.0</gross-value>
      <project>https://api.freeagent.com/v2/projects/1</project>
      <rebill-type>markup</rebill-type>
      <rebill-factor type="decimal">0.25</rebill-factor>
      <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
      <sales-tax-status>TAXABLE</sales-tax-status>
      <sales-tax-rate type="decimal">0.0</sales-tax-rate>
      <sales-tax-value type="decimal">0.0</sales-tax-value>
      <is-deletable type="boolean">true</is-deletable>
      <attachment>
        <url>https://api.freeagent.com/v2/attachments/3</url>
        <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/2/original.pdf?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1316186508&amp;Signature=R0jkClXSS5TYvvmFlOP%2F6UIRuBY%3D</content-src>
        <content-type>application/pdf</content-type>
        <file-name>About Stacks.pdf</file-name>
        <file-size type="integer">466028</file-size>
      </attachment>
    </bank-transaction-explanation>
  </bank-transaction-explanations>
</freeagent>
```
Show as JSON

## Get a single bank transaction explanation

```http
GET https://api.freeagent.com/v2/bank_transaction_explanations/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "bank_transaction_explanation":
  {
    "bank_transaction":"https://api.freeagent.com/v2/bank_transactions/8",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2019-05-01",
    "description":"harness end-to-end e-business",
    "gross_value":"-730.0",
    "project": "https://api.freeagent.com/v2/projects/1",
    "rebill_type": "markup",
    "rebill_factor": "0.25",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "sales_tax_status": "TAXABLE",
    "sales_tax_rate": "20.0",
    "sales_tax_value": "-121.67",
    "is_deletable": true,
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
  <bank-transaction-explanation>
    <bank-transaction>https://api.freeagent.com/v2/bank_transactions/8</bank-transaction>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <dated-on type="date">2019-05-01</dated-on>
    <description>harness end-to-end e-business</description>
    <entry-type>Business Entertaining</entry-type>
    <gross-value type="decimal">-730.0</gross-value>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <rebill-type>markup</rebill-type>
    <rebill-factor type="decimal">0.25</rebill-factor>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <sales-tax-status>TAXABLE</sales-tax-status>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <sales-tax-value type="decimal">-121.67</sales-tax-value>
    <is-deletable type="boolean">true</is-deletable>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
  </bank-transaction-explanation>
</freeagent>
```
Show as JSON

## Create a bank transaction explanation

Payload should have a root `bank_transaction_explanation` element, containing elements
listed under Attributes.

Bank transaction explanations can be created for all of the explanation
types supported in FreeAgent.  Set the `bank_transaction` attribute to
explain an existing bank transaction or set the `bank_account` attribute to
create a matching bank transaction along with the bank transaction
explanation.

In general the explanation type is chosen
by setting the `category` attribute.  However, for paying an invoice, bill or
transferring funds between bank accounts, the category is not required.
To make a Smart User Payment, set the `paid_user` attribute, but
omit the `category` attribute.  To create explanations for other kinds of user payments set
both the `paid_user` attribute and the relevant `category` attribute.

```http
POST https://api.freeagent.com/v2/bank_transaction_explanations
```

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/bank_transaction_explanations/12
```

```json
{ "bank_transaction_explanation":
  {
    "bank_transaction":"https://api.freeagent.com/v2/bank_transactions/8",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2019-05-01",
    "description":"harness end-to-end e-business",
    "category":"https://api.freeagent.com/v2/categories/285",
    "gross_value":"-730.0",
    "project": "https://api.freeagent.com/v2/projects/1",
    "rebill_type": "markup",
    "rebill_factor": "0.25",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "sales_tax_status": "TAXABLE",
    "sales_tax_rate": "20.0",
    "sales_tax_value": "-121.67",
    "is_deletable": true,
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
  <bank-transaction-explanation>
    <bank-transaction>https://api.freeagent.com/v2/bank_transactions/8</bank-transaction>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <dated-on type="date">2019-05-01</dated-on>
    <description>harness end-to-end e-business</description>
    <category>https://api.freeagent.com/v2/categories/285</category>
    <gross-value type="decimal">-730.0</gross-value>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <rebill-type>markup</rebill-type>
    <rebill-factor type="decimal">0.25</rebill-factor>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <sales-tax-status>TAXABLE</sales-tax-status>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <sales-tax-value type="decimal">-121.67</sales-tax-value>
    <is-deletable type="boolean">true</is-deletable>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>
  </bank-transaction-explanation>
</freeagent>
```
Show as JSON

## Transferring money between bank accounts

Explanations which explain money transfers will have a linked explanation
and bank account which explain the other side of the transfer.

### Response

```json
{ "bank_transaction_explanation":
  {
    "bank_transaction":"https://api.freeagent.com/v2/bank_transactions/8",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2019-05-01",
    "description":"Transfer from Bank Account One to Bank Account Two",
    "linked_transfer_explanation":"https://api.freeagent.com/v2/bank_transaction_explanation/125",
    "linked_transfer_account":"https://api.freeagent.com/v2/bank_accounts/2",
    "gross_value":"-170.0",
    "is_deletable": true,
    "updated_at": "2020-02-06T11:08:28.000Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-transaction-explanation>
    <bank-transaction>https://api.freeagent.com/v2/bank_transactions/8</bank-transaction>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <dated-on type="date">2019-05-01</dated-on>
    <description>Transfer from Bank Account One to Bank Account Two</description>
    https://api.freeagent.com/v2/bank_transaction_explanation/125
    https://api.freeagent.com/v2/bank_accounts/2
    <gross-value type="decimal">-730.0</gross-value>
    <is-deletable type="boolean">true</is-deletable>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
  </bank-transaction-explanation>
</freeagent>
```
Show as JSON

## Update a bank transaction explanation

```http
PUT https://api.freeagent.com/v2/bank_transaction_explanations/:id
```

Payload should have a root `bank_transaction_explanation` element, containing elements
listed under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

```json
{ "bank_transaction_explanation":
  {
    "bank_transaction":"https://api.freeagent.com/v2/bank_transactions/8",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2019-05-01",
    "description":"harness end-to-end e-business",
    "category":"https://api.freeagent.com/v2/categories/285",
    "gross_value":"-730.0",
    "project": "https://api.freeagent.com/v2/projects/1",
    "rebill_type": "price",
    "rebill_factor": "800",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "sales_tax_status": "TAXABLE",
    "sales_tax_rate": "20.0",
    "sales_tax_value": "-28.33",
    "is_deletable": true,
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
  <bank-transaction-explanation>
    <bank-transaction>https://api.freeagent.com/v2/bank_transactions/8</bank-transaction>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <dated-on type="date">2019-05-01</dated-on>
    <description>harness end-to-end e-business</description>
    <category>>https://api.freeagent.com/v2/categories/285</category>
    <gross-value type="decimal">-730.0</gross-value>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <rebill-type>price</rebill-type>
    <rebill-factor type="decimal">800</rebill-factor>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <sales-tax-status>TAXABLE</sales-tax-status>
    <sales-tax-rate type="decimal">20.0</sales-tax-rate>
    <sales-tax-value type="decimal">-121.67</sales-tax-value>
    <is-deletable type="boolean">true</is-deletable>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
    </attachment>

  </bank-transaction-explanation>
</freeagent>
```
Show as JSON

## Delete a bank transaction explanation

```http
DELETE https://api.freeagent.com/v2/bank_transaction_explanations/:id
```

### Response

```http
Status: 200 OK
```