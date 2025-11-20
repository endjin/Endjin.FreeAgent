# Estimates

*Minimum access level*: `Estimates and Invoices`, unless stated otherwise.

## Estimate Attributes

| Required | Attribute                        | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Kind      |
| -------- | -------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                              | The unique identifier for the estimate                                                                                                                                                                                                                                                                                                                                                                                                                                | URI       |
| ✔        | status                           | One of the following: `Draft`, `Sent`, `Open`, `Approved`, `Rejected`, `Invoiced`                                                                                                                                                                                                                                                                                                                                                                                     | String    |
| ✔        | estimate_type                    | One of the following: `Estimate`, `Quote`, `Proposal`                                                                                                                                                                                                                                                                                                                                                                                                                 | String    |
| ✔        | contact                          | The [contact](contacts.md) for whom the estimate is created                                                                                                                                                                                                                                                                                                                                                                                                           | URI       |
|          | project                          | [Project](projects.md) being estimated                                                                                                                                                                                                                                                                                                                                                                                                                                | URI       |
| ✔        | reference                        | Free-text reference                                                                                                                                                                                                                                                                                                                                                                                                                                                   | String    |
| ✔        | dated_on                         | Date of estimate in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                               | Date      |
| ✔        | currency                         | Estimate's [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                                                                                                  | String    |
|          | notes                            | Additional text                                                                                                                                                                                                                                                                                                                                                                                                                                                       | String    |
|          | discount_percent                 | The discount applied across the whole estimate                                                                                                                                                                                                                                                                                                                                                                                                                        | Decimal   |
|          | client_contact_name              | This name will override the default [contact](contacts.md) name on this estimate                                                                                                                                                                                                                                                                                                                                                                                      | String    |
|          | ec_status                        | Estimate's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge`, `EC VAT MOSS` Please note that `EC Goods` and `EC Services` are no longer valid options if the estimate is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the estimate is dated 1/1/2021 or later. | String    |
|          | place_of_supply                  | Place of supply when `ec_status` is [EC VAT MOSS](sales_tax.md#ec-vat-moss)                                                                                                                                                                                                                                                                                                                                                                                           | String    |
|          | estimate_items                   | Items for this estimate. See [Estimate Item Attributes](#estimate-item-attributes).                                                                                                                                                                                                                                                                                                                                                                                   | Array     |
|          | net_value                        | Total value of the estimate calculated from its estimate items                                                                                                                                                                                                                                                                                                                                                                                                        | Decimal   |
|          | include_sales_tax_on_total_value | Include or exclude sales tax from the totals shown on the estimate Defaults to `true`                                                                                                                                                                                                                                                                                                                                                                                 | Boolean   |
|          | created_at                       | Creation of the estimate resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                               | Timestamp |
|          | updated_at                       | When the estimate resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                     | Timestamp |

## Estimate Item Attributes

| Required | Attribute               | Description                                                                                                                                                               | Kind      |
| -------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                     | The unique identifier for the estimate item                                                                                                                               | URI       |
|          | position                | Position on the estimate, starting at 1                                                                                                                                   | Integer   |
| ✔        | item_type               | One of the following: `Hours`, `Days`, `Weeks`, `Months`, `Years`, `-no unit-`, `Products`, `Services`, `Training`, `Expenses`, `Comments`, `Bills`, `Discount`, `Credit` | String    |
|          | quantity                | Quantity                                                                                                                                                                  | Decimal   |
| ✔        | price                   | Price                                                                                                                                                                     | Decimal   |
| ✔        | description             | Free-text description                                                                                                                                                     | String    |
|          | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates                                                                                                                       | Decimal   |
|          | sales_tax_value         | Total amount of sales tax                                                                                                                                                 | Decimal   |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standaard second sales tax rates                                                                                                     | Decimal   |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE` or `EXEMPT` from [sales tax](sales_tax.md)                                                                                        | String    |
|          | second_sales_tax_status | [Universal accounts only] Similar to sales_tax_status, returned only if the relevant [sales tax period](sales_tax_periods.md) defines a second sales tax                  | String    |
|          | second_sales_tax_value  | [Universal accounts only]Total amount of second sales tax                                                                                                                 | Decimal   |
|          | category                | [Accounting category](categories.md) the estimate item falls under                                                                                                        | URI       |
|          | created_at              | Creation of the estimate item                                                                                                                                             | Timestamp |
|          | updated_at              | When the estimate item was last updated                                                                                                                                   | Timestamp |

## List all estimates

```http
GET https://api.freeagent.com/v2/estimates
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/estimates?view=recent
```

- `all`: (default)
- `recent`: Show only recent estimates.
- `draft`: Show only estimates marked as draft.
- `non_draft`: Show only estimates the not marked as draft.
- `sent`: Show only estimates marked as sent.
- `approved`: Show only estimates marked as approved.
- `rejected`: Show only estimates marked as rejected.
- `invoiced`: Show only estimates marked as invoiced.

#### Date Filters

```http
GET https://api.freeagent.com/v2/estimates?from_date=2012-01-01&to_date=2012-03-21
```

```http
GET https://api.freeagent.com/v2/estimates?updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

### Response

```http
Status: 200 OK
```

```json
{ "estimates": [
  {
    "url":"https://api.freeagent.com/v2/estimates/1",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "reference":"001",
    "estimate_type":"Estimate",
    "dated_on":"2011-09-15",
    "status":"Draft",
    "notes":"An example of some additional text.",
    "currency":"GBP",
    "net_value":"25.22",
    "sales_tax_value":"5.04266",
    "sales_tax_status": "TAXABLE",
    "include_sales_tax_on_total_value":true,
    "updated_at":"2011-09-15T10:30:32Z",
    "created_at":"2011-09-15T10:29:51Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <estimates type="array">
    <estimate>
      <url>https://api.freeagent.com/v2/estimates/1</url>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <reference>001</reference>
      <estimate-type>Estimate</estimate-type>
      <dated-on type="date">2011-09-15</dated-on>
      <status>Draft</status>
      <notes>An example of some additional text.</notes>
      <currency>GBP</currency>
      <net-value type="decimal">25.22</net-value>
      <sales-tax-value type="decimal">5.04266</sales-tax-value>
      <include-sales-tax-on-total-value type="boolean">true</include-sales-tax-on-total-value>
      <updated-at type="datetime">2011-09-15T10:30:32Z</updated-at>
      <created-at type="datetime">2011-09-15T10:29:51Z</created-at>
    </estimate>
  </estimates>
</freeagent>
```
Show as JSON

## List all estimates with nested estimate items

You can include estimate items nested into the list of estimates which increases
request size but removes the need to request the estimates separately to see
estimate item information.

```http
GET https://api.freeagent.com/v2/estimates?nested_estimate_items=true
```

### Response

```http
Status: 200 OK
```

```json
{ "estimates": [
  {
    "url":"https://api.freeagent.com/v2/estimates/1",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "reference":"001",
    "estimate_type":"Estimate",
    "dated_on":"2011-09-15",
    "status":"Draft",
    "notes":"An example of some additional text.",
    "currency":"GBP",
    "net_value":"25.22",
    "sales_tax_value":"5.04266",
    "include_sales_tax_on_total_value":true,
    "updated_at":"2011-09-15T10:30:32Z",
    "created_at":"2011-09-15T10:29:51Z",
    "estimate_items": [
      {
        "url":"https://api.freeagent.com/v2/estimate_items/1",
        "position":"1",
        "item_type":"Hours",
        "quantity":"1.03333333",
        "price":"12.2",
        "description":"Development",
        "sales_tax_value":"20.0",
        "sales_tax_status": "TAXABLE",
        "category":"https://api.freeagent.com/v2/categories/001",
        "updated_at":"2011-09-15T10:30:22Z",
        "created_at":"2011-09-15T10:30:22Z"
      }
    ]
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <estimates type="array">
    <estimate>
      <url>https://api.freeagent.com/v2/estimates/1</url>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <reference>001</reference>
      <estimate-type>Estimate</estimate-type>
      <dated-on type="date">2011-09-15</dated-on>
      <status>Draft</status>
      <notes>An example of some additional text.</notes>
      <currency>GBP</currency>
      <net-value type="decimal">25.22</net-value>
      <sales-tax-value type="decimal">5.04266</sales-tax-value>
      <include-sales-tax-on-total-value type="boolean">true</include-sales-tax-on-total-value>
      <updated-at type="datetime">2011-09-15T10:30:32Z</updated-at>
      <created-at type="datetime">2011-09-15T10:29:51Z</created-at>
      <estimate-items type="array">
        <estimate-item>
          <url>https://api.freeagent.com/v2/estimate_items/1</url>
          <position type="integer">1</position>
          <item-type>Hours</item-type>
          <quantity type="decimal">1.03333333</quantity>
          <price type="decimal">12.2</price>
          <description>sada</description>
          <sales-tax-value type="decimal">20.0</sales-tax-value>
          <category>https://api.freeagent.com/v2/categories/001</category>
          <updated-at type="datetime">2011-09-15T10:30:22Z</updated-at>
          <created-at type="datetime">2011-09-15T10:30:22Z</created-at>
        </estimate-item>
      </estimate-items>
    </estimate>
  </estimates>
</freeagent>
```
Show as JSON

## Get a single estimate

```http
GET https://api.freeagent.com/v2/estimates/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "estimate":
  {
    "url":"https://api.freeagent.com/v2/estimates/1",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "reference":"001",
    "estimate_type":"Estimate",
    "dated_on":"2011-09-15",
    "status":"Draft",
    "notes":"An example of some additional text.",
    "currency":"GBP",
    "net_value":"25.22",
    "sales_tax_value":"5.04266",
    "updated_at":"2011-09-15T10:30:32Z",
    "created_at":"2011-09-15T10:29:51Z",
    "estimate_items": [
      {
        "url":"https://api.freeagent.com/v2/estimate_items/1",
        "position":"1",
        "item_type":"Hours",
        "quantity":"1.03333333",
        "price":"12.2",
        "description":"sada",
        "sales_tax_value":"20.0",
        "sales_tax_status": "TAXABLE",
        "category":"https://api.freeagent.com/v2/categories/001",
        "updated_at":"2011-09-15T10:30:22Z",
        "created_at":"2011-09-15T10:30:22Z"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <estimate>
    <url>https://api.freeagent.com/v2/estimates/1</url>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <reference>001</reference>
    <estimate-type>Estimate</estimate-type>
    <dated-on type="date">2011-09-15</dated-on>
    <status>Draft</status>
    <notes>An example of some additional text.</notes>
    <currency>GBP</currency>
    <net-value type="decimal">25.22</net-value>
    <sales-tax-value type="decimal">5.04266</sales-tax-value>
    <updated-at type="datetime">2011-09-15T10:30:32Z</updated-at>
    <created-at type="datetime">2011-09-15T10:29:51Z</created-at>
    <estimate-items type="array">
      <estimate-item>
        <url>https://api.freeagent.com/v2/estimate_items/1</url>
        <position type="integer">1</position>
        <item-type>Hours</item-type>
        <quantity type="decimal">1.03333333</quantity>
        <price type="decimal">12.2</price>
        <description>sada</description>
        <sales-tax-value type="decimal">20.0</sales-tax-value>
        <category>https://api.freeagent.com/v2/categories/001</category>
        <updated-at type="datetime">2011-09-15T10:30:22Z</updated-at>
        <created-at type="datetime">2011-09-15T10:30:22Z</created-at>
      </estimate-item>
    </estimate-items>
  </estimate>
</freeagent>
```
Show as JSON

## Get a single estimate as PDF

```http
GET https://api.freeagent.com/v2/estimates/:id/pdf
```

### Notes

For compatibility purposes, the API returns a base64-encoded representation of the PDF data
inside a JSON or XML payload. After fetching the response, simply look inside the
*pdf.content* node and base64-decode its value to get the PDF.

The encoded data complies with RFC 2045.

### Response

```json
{
  "pdf": {
    "content": "... base64 encoded PDF data ..."
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <pdf>
    <content>... base64 encoded PDF data ...</content>
  </pdf>
</freeagent>
```
Show as JSON

## List all estimates related to a contact

```http
GET https://api.freeagent.com/v2/estimates?contact=https://api.freeagent.com/v2/contacts/2
```

## List all estimates related to a project

```http
GET https://api.freeagent.com/v2/estimates?project=https://api.freeagent.com/v2/projects/2
```

## List all estimates related to an invoice

```http
GET https://api.freeagent.com/v2/estimates?invoice=https://api.freeagent.com/v2/invoices/2
```

## Create an estimate

```http
POST https://api.freeagent.com/v2/estimates
```

Payload should have a root `estimate` element, containing elements listed
under Estimate Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/estimates/9
```

```json
{ "estimate":
  {
    "url":"https://api.freeagent.com/v2/estimates/9",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "reference":"001",
    "estimate_type":"Estimate",
    "dated_on":"2011-09-15",
    "status":"Draft",
    "notes":"An example of some additional text.",
    "currency":"GBP",
    "net_value":"25.22",
    "sales_tax_value":"5.04266",
    "updated_at":"2011-09-15T10:30:32Z",
    "created_at":"2011-09-15T10:29:51Z",
    "estimate_items": [
      {
        "url":"https://api.freeagent.com/v2/estimate_items/1",
        "position":"1",
        "item_type":"Hours",
        "quantity":"1.03333333",
        "price":"12.2",
        "description":"sada",
        "sales_tax_value":"20.0",
        "sales_tax_status": "TAXABLE",
        "category":"https://api.freeagent.com/v2/categories/001",
        "updated_at":"2011-09-15T10:30:22Z",
        "created_at":"2011-09-15T10:30:22Z"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <estimate>
    <url>https://api.freeagent.com/v2/estimates/1</url>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <reference>001</reference>
    <estimate-type>Estimate</estimate-type>
    <dated-on type="date">2011-09-15</dated-on>
    <status>Draft</status>
    <notes>An example of some additional text.</notes>
    <currency>GBP</currency>
    <net-value type="decimal">25.22</net-value>
    <sales-tax-value type="decimal">5.04266</sales-tax-value>
    <updated-at type="datetime">2011-09-15T10:30:32Z</updated-at>
    <created-at type="datetime">2011-09-15T10:29:51Z</created-at>
    <estimate-items type="array">
      <estimate-item>
        <url>https://api.freeagent.com/v2/estimate_items/1</url>
        <position type="integer">1</position>
        <item-type>Hours</item-type>
        <quantity type="decimal">1.03333333</quantity>
        <price type="decimal">12.2</price>
        <description>sada</description>
        <sales-tax-value type="decimal">20.0</sales-tax-value>
        <category>https://api.freeagent.com/v2/categories/001</category>
        <updated-at type="datetime">2011-09-15T10:30:22Z</updated-at>
        <created-at type="datetime">2011-09-15T10:30:22Z</created-at>
      </estimate-item>
    </estimate-items>
  </estimate>
</freeagent>
```
Show as JSON

## Update an estimate

To update the status of an estimate you must use the status transitions to mark an estimate as `Draft`, `Sent`, `Approved` or `Rejected`.

```http
PUT https://api.freeagent.com/v2/estimates/:id
```

Payload should have a root `estimate` element, containing elements listed
under Estimate Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Create an estimate item

```http
POST https://api.freeagent.com/v2/estimate_items
```

Payload should have two root elements:

- `estimate`, URL of the estimate to which the item should be added
- `estimate_item`, containing elements listed under Estimate Item Attributes

### Response

```http
Status: 200 OK
Location: https://api.freeagent.com/v2/estimates_items/2
```

```json
{ "estimate_item":
  {
    "url":"https://api.freeagent.com/v2/estimate_items/2",
    "position":"1",
    "item_type":"Hours",
    "quantity":"1.03333333",
    "price":"12.2",
    "description":"sada",
    "sales_tax_value":"20.0",
    "sales_tax_status": "TAXABLE",
    "category":"https://api.freeagent.com/v2/categories/001",
    "updated_at":"2011-09-15T10:30:22Z",
    "created_at":"2011-09-15T10:30:22Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <estimate-item>
    <url>https://api.freeagent.com/v2/estimate_items/2</url>
    <position type="integer">1</position>
    <item-type>Hours</item-type>
    <quantity type="decimal">1.03333333</quantity>
    <price type="decimal">12.2</price>
    <description>sada</description>
    <sales-tax-value type="decimal">20.0</sales-tax-value>
    <category>https://api.freeagent.com/v2/categories/001</category>
    <updated-at type="datetime">2011-09-15T10:30:22Z</updated-at>
    <created-at type="datetime">2011-09-15T10:30:22Z</created-at>
  </estimate-item>
</freeagent>
```
Show as JSON

## Update an estimate item

```http
PUT https://api.freeagent.com/v2/estimate_items/:id
```

Payload should have a root `estimate_item` element, containing elements listed
under Estimate Item Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Duplicate an estimate

Estimates are always duplicated with a status of Draft, the next reference in the sequence and dated for today's date.

```http
POST https://api.freeagent.com/v2/estimates/:id/duplicate
```

### Response

```http
Status: 200 OK
```

The duplicated estimate will be returned in the body.

## Delete an estimate item

```http
DELETE https://api.freeagent.com/v2/estimate_items/:id
```

### Response

```http
Status: 200 OK
```

## Delete an estimate

```http
DELETE https://api.freeagent.com/v2/estimates/:id
```

### Response

```http
Status: 200 OK
```

## Email an estimate

```http
POST https://api.freeagent.com/v2/estimates/:id/send_email
```

### Input

- `estimate`(Hash)
    - `email`(Hash)
        - `to`
        - `from`- Needs to belong to a registered user and be in one of the following formats:
            - `John Doe <johndoe@example.com>`
            - `johndoe@example.com`
        - `subject`
        - `body`
        - `email_to_sender` (Boolean) - defaults to `true`
        - `attachments`- Array of email attachment hash data structures. Each attachment has a max size limit of 5MB. An email attachment data structure should have the following attributes:
            - `content_type` (String) - MIME-type
            - `data` (String) - Binary data of the file being attached encoded as base64
            - `file_name` (String)

If, instead of defining all e-mail attributes in your request, you wish to use an existing [e-mail template](https://support.freeagent.com/hc/en-gb/articles/115001218070-Set-up-an-automatic-email-for-new-estimates),
all you need to include in the request body is the `use_template` option set to `true`, i.e.

```json
{
  "estimate": {
    "email": {
      "use_template": true
    }
  }
}
```
Show as XML

```json
{
  "estimate": {
    "email": {
      "attachments": [
        {
          "file_name": "testFile.csv",
          "content_type": "text/csv",
          "data": "MTIvMDkvMjAyMiw1LjAwLGJ1cmdlcgo="
        }
      ]
    }
  }
}
```
Show as XML

### Response

```http
Status: 200 OK
```

## Mark estimate as sent

```http
PUT https://api.freeagent.com/v2/estimates/:id/transitions/mark_as_sent
```

### Response

```http
Status: 200 OK
```

## Mark estimate as draft

```http
PUT https://api.freeagent.com/v2/estimates/:id/transitions/mark_as_draft
```

### Response

```http
Status: 200 OK
```

## Mark estimate as approved

```http
PUT https://api.freeagent.com/v2/estimates/:id/transitions/mark_as_approved
```

### Response

```http
Status: 200 OK
```

## Mark estimate as rejected

```http
PUT https://api.freeagent.com/v2/estimates/:id/transitions/mark_as_rejected
```

### Response

```http
Status: 200 OK
```

## Convert estimate to an invoice

```http
PUT https://api.freeagent.com/v2/estimates/:id/transitions/convert_to_invoice
```

### Response

```http
Status: 200 OK
```

The estimate will be returned in the body, updated with `"status": "Invoiced"` and `"invoice":` set to the URL of
the generated Invoice.

## Default additional text

This API allows configuration of the additional text shown on all estimates issued by the company.

### Get default additional text

```http
GET https://api.freeagent.com/v2/estimates/default_additional_text
```

### Response

```http
Status: 200 OK
```

```json
{
  "default_additional_text": "Please respond within 21 working days"
}
```
Show as XML

```xml
<default-additional-text>Please respond within 21 working days</default-additional-text>
```
Show as JSON

### Update default additional text

```http
PUT https://api.freeagent.com/v2/estimates/default_additional_text
```

#### Example Request Body

```json
{
    "default_additional_text": "Respond to this quote within 7 days"
}
```
Show as XML

```xml
<default-additional-text>Respond to this quote within 7 days</default-additional-text>
```
Show as JSON

### Response

```http
Status: 200 OK
```

```json
{
  "default_additional_text": "Respond to this quote within 7 days"
}
```
Show as XML

```xml
<default-additional-text>Respond to this quote within 7 days</default-additional-text>
```
Show as JSON

### Delete default additional text

```http
DELETE https://api.freeagent.com/v2/estimates/default_additional_text
```

### Response

```http
Status: 200 OK
```