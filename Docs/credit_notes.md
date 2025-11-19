# Credit Notes

*Minimum access level*: `Estimates and Invoices`, unless stated otherwise.

## Credit Note Attributes

| Required | Attribute              | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Kind      |      |
| -------- | ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
|          | url                    | The unique identifier for the credit note                                                                                                                                                                                                                                                                                                                                                                                                                                      | URI       |      |
|          | status                 | One of the following: `Draft`, `Open`, `Overdue`, `Refunded`, `Written-off`                                                                                                                                                                                                                                                                                                                                                                                                    | String    |      |
|          | long_status            | Credit note status along with the due date as a relative date to 'today' For example: `Open – due in about 1 month`, `Overdue – due about 1 month ago`, `Refunded on – 21 Mar 20`                                                                                                                                                                                                                                                                                              | String    |      |
| ✔        | contact                | The [contact](contacts.md) being credited                                                                                                                                                                                                                                                                                                                                                                                                                                      | URI       |      |
|          | project                | The [project](projects.md) being credited                                                                                                                                                                                                                                                                                                                                                                                                                                      | URI       |      |
| ?        | property               | The [property](properties.md) pertaining to this invoice. Only accepted and required for companies with type `UkUnincorporatedLandlord`.                                                                                                                                                                                                                                                                                                                                       | URI       |      |
|          | reference              | Credit note reference (using global invoice sequencing) If omitted, next invoice reference will be used                                                                                                                                                                                                                                                                                                                                                                        | String    |      |
| ✔        | dated_on               | Date of credit note in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                                     | Date      |      |
|          | due_on                 | When credit note is due, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                                | Date      |      |
| ✔        | payment_terms_in_days  | Set to zero to display 'Due on Receipt' on the credit note                                                                                                                                                                                                                                                                                                                                                                                                                     | Integer   |      |
|          | currency               | Credit note's [currency](currencies.md) Defaults to the company's native currency                                                                                                                                                                                                                                                                                                                                                                                              | String    |      |
|          | cis_rate               | One of the following: `null`, Name of a [Construction Industry Scheme band](cis_bands.md)                                                                                                                                                                                                                                                                                                                                                                                      | String \  | null |
|          | cis_deduction_rate     | Percentage of CIS deduction for the `cis_rate` set                                                                                                                                                                                                                                                                                                                                                                                                                             | Decimal   |      |
|          | cis_deduction          | Total CIS deduction for this credit note, in its currency                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |      |
|          | cis_deduction_suffered | CIS deduction already paid for this credit note, in its currency                                                                                                                                                                                                                                                                                                                                                                                                               | Decimal   |      |
|          | comments               | Additional text added to the bottom of the credit note                                                                                                                                                                                                                                                                                                                                                                                                                         | String    |      |
|          | discount_percent       | The discount applied across the whole credit note                                                                                                                                                                                                                                                                                                                                                                                                                              | Decimal   |      |
|          | client_contact_name    | This name will override the default [contact](contacts.md) name on this credit note                                                                                                                                                                                                                                                                                                                                                                                            | String    |      |
|          | payment_terms          | This will override the normal payment terms and credit note due date                                                                                                                                                                                                                                                                                                                                                                                                           | String    |      |
|          | po_reference           | This PO reference will override any PO set for the [project](projects.md)                                                                                                                                                                                                                                                                                                                                                                                                      | String    |      |
|          | bank_account           | This will be used to display remittance advice on this credit note                                                                                                                                                                                                                                                                                                                                                                                                             | String    |      |
|          | omit_header            | `true` to omit your logo and company address, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                                | Boolean   |      |
|          | show_project_name      | `true` to display the [project](projects.md) name in the Other Information section, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                          | Boolean   |      |
|          | ec_status              | Credit note's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge`, `EC VAT MOSS` Please note that `EC Goods` and `EC Services` are no longer valid options if the credit note is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the credit note is dated 1/1/2021 or later. | String    |      |
|          | place_of_supply        | Place of supply when `ec_status` is [EC VAT MOSS](sales_tax.md#ec-vat-moss)                                                                                                                                                                                                                                                                                                                                                                                                    | String    |      |
|          | net_value              | Net value                                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |      |
|          | exchange_rate          | Rate at which credit note amount is converted into company's native [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                                                  | Decimal   |      |
|          | involves_sales_tax     | `true` if [sales tax](sales_tax.md) applies to the credit note, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                              | Boolean   |      |
|          | sales_tax_value        | Total value of [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                                                                                       | Decimal   |      |
|          | second_sales_tax_value | [Universal accounts only] Total value of [second sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |      |
|          | total_value            | Gross value                                                                                                                                                                                                                                                                                                                                                                                                                                                                    | Decimal   |      |
|          | refunded_value         | Amount refunded so far                                                                                                                                                                                                                                                                                                                                                                                                                                                         | Decimal   |      |
|          | due_value              | Amount yet to be refunded                                                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |      |
|          | is_interim_uk_vat      | `true` if VAT status was `Registration Applied For` at credit note date, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                     | Boolean   |      |
|          | refunded_on            | When the credit note was fully refunded, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                | Date      |      |
|          | written_off_date       | When the credit note was written off, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                   | Date      |      |
|          | credit_note_items      | Array of credit note item data structures. See [Credit Note Item Attributes](#credit-note-item-attributes).                                                                                                                                                                                                                                                                                                                                                                    | Array     |      |
|          | created_at             | Creation of the credit note resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                                     | Timestamp |      |
|          | updated_at             | When the credit note resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                           | Timestamp |      |

## Credit Note Item Attributes

| Required | Attribute               | Description                                                                                                                                                                                  | Kind    |
| -------- | ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
|          | url                     | The unique identifier for the credit note item                                                                                                                                               | URI     |
|          | position                | Position in the credit note, starting at 1                                                                                                                                                   | Decimal |
|          | item_type               | One of the following: `Hours`, `Days`, `Weeks`, `Months`, `Years`, `Products`, `Services`, `Training`, `Expenses`, `Comment`, `Bills`, `Discount`, `Credit`, `VAT` Leave blank for 'No Unit' | String  |
|          | quantity                | Quantity of the `item_type` Defaults to `0`                                                                                                                                                  | Decimal |
| ✔        | description             | Credit note item details                                                                                                                                                                     | String  |
| ?        | price                   | Unit price Required if `invoice_item` is given and `item_type` is non time based                                                                                                             | Decimal |
|          | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates                                                                                                                                          | Decimal |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                                         | Decimal |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE`, `EXEMPT` or `OUT_OF_SCOPE` for [sales tax](sales_tax.md)                                                                                            | String  |
|          | second_sales_tax_status | [Universal accounts only] Similar to sales_tax_status, returned only if the relevant [sales tax period](sales_tax_periods.md) defines a second sales tax                                     | String  |
| ?        | stock_item              | [Stock item](stock_items.md) being credited, if `item_type` is `Stock`                                                                                                                       | URI     |
|          | category                | [Accounting category](categories.md) of the credit note item                                                                                                                                 | URI     |
|          | project                 | The [project](projects.md) being credited                                                                                                                                                    | URI     |
| ✔        | id                      | ID of the credit note item to update                                                                                                                                                         | Integer |
| ✔        | id                      | ID of the credit note item to delete                                                                                                                                                         | Integer |
| ✔        | _destroy                | Should be equal to `1`                                                                                                                                                                       | Integer |

## List all credit notes

```http
GET https://api.freeagent.com/v2/credit_notes
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/credit_notes?view=recent_open_or_overdue
```

- `all`: Show all credit notes (default)
- `recent_open_or_overdue`: Show only recent, open, or overdue invoices.
- `open`: Show only open credit notes.
- `overdue`: Show only overdue credit notes.
- `open_or_overdue`: Show only open or overdue credit notes.
- `draft`: Show only draft credit notes.
- `refunded`: Show only refunded credit notes.
- `last_N_months`: Show only credit notes from the last `N` months.

#### Date Filters

```http
GET https://api.freeagent.com/v2/credit_notes?updated_since=2017-05-22T09:00:00.000Z
```

- `updated_since`

#### Sort Orders

```http
GET https://api.freeagent.com/v2/credit_notes?sort=updated_at
```

- `created_at`: Sort by the time the credit note was created (default).
- `updated_at`: Sort by the time the credit note was last modified.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/credit_notes?sort=-updated_at
```

### Response

```http
Status: 200 OK
```

```json
{
  "credit_notes": [
    {
      "url":"https://api.freeagent.com/v2/credit_notes/1",
      "contact":"https://api.freeagent.com/v2/contacts/2",
      "dated_on":"2011-08-29",
      "due_on":"2011-09-28",
      "reference":"001",
      "currency":"GBP",
      "exchange_rate":"1.0",
      "net_value":"0.0",
      "sales_tax_value":"0.0",
      "involves_sales_tax":true,
      "is_interim_uk_vat":false,
      "total_value": "200.0",
      "refunded_value": "50.0",
      "due_value": "150.0",
      "status":"Open",
      "long_status":"Open – due in about 1 month",
      "comments":"An example credit note comment.",
      "omit_header":false,
      "payment_terms_in_days":30,
      "created_at":"2019-12-17T10:14:07.000Z",
      "updated_at":"2019-12-17T10:14:15.000Z",
      "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
      "contact_name":"Nathan Barley"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit-notes type="array">
    <credit-note>
      <url>https://api.freeagent.com/v2/credit_notes/1</url>
      <contact>https://api.freeagent.com/v2/contacts/2</contact>
      <dated-on type="datetime">2011-08-29</dated-on>
      <due-on type="datetime">2011-09-28</due-on>
      <reference>001</reference>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <net-value type="decimal">0.0</net-value>
      <sales-tax-value type="decimal">0.0</sales-tax-value>
      <involves-sales-tax type="boolean">true</involves-sales-tax>
      <is-interim-uk-vat type="boolean">false</is-interim-uk-vat>
      <total-value type="decimal">200.0</total-value>
      <refunded-value type="decimal">50.0</refunded-value>
      <due-value type="decimal">150.0</due-value>
      <status>Open</status>
      <long-status>Open – due in about 1 month</long-status>
      <comments>An example credit note comment.</comments>
      <omit-header type="boolean">false</omit-header>
      <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
      <payment-terms-in-days type="integer">30</payment-terms-in-days>
      <created-at type="datetime">2011-08-29T00:00:00Z</created-at>
      <updated-at type="datetime">2011-08-29T00:00:00Z</updated-at>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <contact-name>Nathan Barley</contact-name>
    </credit-note>
  </credit-notes>
</freeagent>
```
Show as JSON

## List all credit notes with nested credit note items

You can include credit note items nested into the list of credit notes which increases
request size but removes the need to request the credit notes separately to see
credit note item information.

```http
GET https://api.freeagent.com/v2/credit_notes?nested_credit_note_items=true
```

### Response

```http
Status: 200 OK
```

```json
{
  "credit_notes": [
    {
      "url": "https://api.freeagent.com/v2/credit_notes/1",
      "contact": "https://api.freeagent.com/v2/contacts/2",
      "dated_on": "2020-01-01",
      "due_on": "2020-01-01",
      "reference": "001",
      "currency": "GBP",
      "exchange_rate": "1.0",
      "net_value": "-100.0",
      "sales_tax_value": "-20.0",
      "involves_sales_tax": true,
      "is_interim_uk_vat": false,
      "total_value": "-120.0",
      "refunded_value": "0.0",
      "due_value": "-120.0",
      "status": "Open",
      "long_status":"Open – due today",
      "omit_header": false,
      "payment_terms_in_days": 0,
      "created_at": "2020-01-21T11:42:38.000Z",
      "updated_at": "2020-01-21T11:43:12.000Z",
      "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
      "contact_name": "Nathan Barley",
      "credit_note_items": [
        {
          "url": "https://api.freeagent.com/v2/invoice_items/1",
          "position": 1,
          "description": "Refund",
          "item_type": "Hours",
          "price": "-100.0",
          "quantity": "1.0",
          "sales_tax_rate": "20.0",
          "suffers_cis_deduction": false,
          "sales_tax_status": "TAXABLE",
          "category": "https://api.freeagent.com/v2/categories/001"
        }
      ]
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit-notes type="array">
    <credit-note>
      <url>https://api.freeagent.com/v2/credit_notes/1</url>
      <contact>https://api.freeagent.com/v2/contacts/2</contact>
      <dated-on type="date">2020-01-01</dated-on>
      <due-on type="date">2020-01-01</due-on>
      <reference>001</reference>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <net-value type="decimal">-100.0</net-value>
      <sales-tax-value type="decimal">-20.0</sales-tax-value>
      <involves-sales-tax type="boolean">true</involves-sales-tax>
      <is-interim-uk-vat type="boolean">false</is-interim-uk-vat>
      <total-value type="decimal">-120.0</total-value>
      <refunded-value type="decimal">0.0</refunded-value>
      <due-value type="decimal">-120.0</due-value>
      <status>Open</status>
      <long-status>Open – due today</long-status>
      <omit-header type="boolean">false</omit-header>
      <payment-terms-in-days type="integer">0</payment-terms-in-days>
      <created-at type="dateTime">2020-01-21T11:42:38Z</created-at>
      <updated-at type="dateTime">2020-01-21T11:43:12Z</updated-at>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <contact-name>Nathan Barley</contact-name>
      <credit-note-items type="array">
        <credit-note-item>
          <url>https://api.freeagent.com/v2/invoice_items/1</url>
          <position type="integer">1</position>
          <description>Refund</description>
          <item-type>Hours</item-type>
          <price type="decimal">-100.0</price>
          <quantity type="decimal">1.0</quantity>
          <sales-tax-rate type="decimal">20.0</sales-tax-rate>
          <suffers-cis-deduction type="boolean">false</suffers-cis-deduction>
          <sales-tax-status>TAXABLE</sales-tax-status>
          <category>https://api.freeagent.com/v2/categories/001</category>
        </credit-note-item>
      </credit-note-items>
    </credit-note>
  </credit-notes>
</freeagent>
```
Show as JSON

## Get a single credit note

```http
GET https://api.freeagent.com/v2/credit_notes/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "credit_note": {
    "url": "https://api.freeagent.com/v2/credit_notes/1",
    "contact": "https://api.freeagent.com/v2/contacts/2",
    "dated_on": "2020-01-01",
    "due_on": "2020-01-01",
    "reference": "001",
    "currency": "GBP",
    "exchange_rate": "1.0",
    "net_value": "-100.0",
    "sales_tax_value": "-20.0",
    "involves_sales_tax": true,
    "is_interim_uk_vat": false,
    "total_value": "-120.0",
    "refunded_value": "0.0",
    "due_value": "-120.0",
    "status": "Open",
    "long_status":"Open – due today",
    "omit_header": false,
    "payment_terms_in_days": 0,
    "created_at": "2020-01-21T11:42:38.000Z",
    "updated_at": "2020-01-21T11:43:12.000Z",
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "contact_name": "Nathan Barley",
    "credit_note_items": [
      {
        "url": "https://api.freeagent.com/v2/invoice_items/1",
        "position": 1,
        "description": "Refund",
        "item_type": "Hours",
        "price": "-100.0",
        "quantity": "1.0",
        "sales_tax_rate": "20.0",
        "suffers_cis_deduction": false,
        "sales_tax_status": "TAXABLE",
        "category": "https://api.freeagent.com/v2/categories/001"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit-note>
    <url>https://api.freeagent.com/v2/credit_notes/1</url>
    <contact>https://api.freeagent.com/v2/contacts/2</contact>
    <dated-on type="date">2020-01-01</dated-on>
    <due-on type="date">2020-01-01</due-on>
    <reference>001</reference>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <net-value type="decimal">-100.0</net-value>
    <sales-tax-value type="decimal">-20.0</sales-tax-value>
    <involves-sales-tax type="boolean">true</involves-sales-tax>
    <is-interim-uk-vat type="boolean">false</is-interim-uk-vat>
    <total-value type="decimal">-120.0</total-value>
    <refunded-value type="decimal">0.0</refunded-value>
    <due-value type="decimal">-120.0</due-value>
    <status>Open</status>
    <long-status>Open – due today</long-status>
    <omit-header type="boolean">false</omit-header>
    <payment-terms-in-days type="integer">0</payment-terms-in-days>
    <created-at type="dateTime">2020-01-21T11:42:38Z</created-at>
    <updated-at type="dateTime">2020-01-21T11:43:12Z</updated-at>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <contact-name>Nathan Barley</contact-name>
    <credit-note-items type="array">
      <credit-note-item>
        <url>https://api.freeagent.com/v2/invoice_items/1</url>
        <position type="integer">1</position>
        <description>Refund</description>
        <item-type>Hours</item-type>
        <price type="decimal">-100.0</price>
        <quantity type="decimal">1.0</quantity>
        <sales-tax-rate type="decimal">20.0</sales-tax-rate>
        <suffers-cis-deduction type="boolean">false</suffers-cis-deduction>
        <sales-tax-status>TAXABLE</sales-tax-status>
        <category>https://api.freeagent.com/v2/categories/001</category>
      </credit-note-item>
    </credit-note-items>
  </credit-note>
</freeagent>
```
Show as JSON

## Get a single credit note as PDF

```http
GET https://api.freeagent.com/v2/credit_notes/:id/pdf
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

## List all Credit Notes related to a contact

```http
GET https://api.freeagent.com/v2/credit_notes?contact=https://api.freeagent.com/v2/contacts/2
```

## List all Credit Notes related to a project

```http
GET https://api.freeagent.com/v2/credit_notes?project=https://api.freeagent.com/v2/projects/2
```

## Create a credit note

A credit note is always created with a status of `Draft`. You must use the status transitions to mark a credit note as `Draft`, `Sent` or `Cancelled`.

```http
POST https://api.freeagent.com/v2/credit_notes
```

Payload should have a root `credit_note` element, containing elements listed
under Credit Note Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/credit_notes/3
```

```json
{
  "credit_note": {
    "url": "https://api.freeagent.com/v2/credit_notes/3",
    "contact": "https://api.freeagent.com/v2/contacts/2",
    "dated_on": "2020-01-01",
    "due_on": "2020-01-01",
    "reference": "001",
    "currency": "GBP",
    "exchange_rate": "1.0",
    "net_value": "-100.0",
    "sales_tax_value": "-20.0",
    "involves_sales_tax": true,
    "is_interim_uk_vat": false,
    "total_value": "-120.0",
    "refunded_value": "0.0",
    "due_value": "-120.0",
    "status": "Draft",
    "long_status":"Draft",
    "omit_header": false,
    "payment_terms_in_days": 0,
    "created_at": "2020-01-21T11:42:38.000Z",
    "updated_at": "2020-01-21T11:43:12.000Z",
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "contact_name": "Nathan Barley",
    "credit_note_items": [
      {
        "url": "https://api.freeagent.com/v2/invoice_items/1",
        "position": 1,
        "description": "Refund",
        "item_type": "Hours",
        "price": "-100.0",
        "quantity": "1.0",
        "sales_tax_rate": "20.0",
        "suffers_cis_deduction": false,
        "sales_tax_status": "TAXABLE",
        "category": "https://api.freeagent.com/v2/categories/001"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit-note>
    <url>https://api.freeagent.com/v2/credit_notes/1</url>
    <contact>https://api.freeagent.com/v2/contacts/2</contact>
    <dated-on type="date">2020-01-01</dated-on>
    <due-on type="date">2020-01-01</due-on>
    <reference>001</reference>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <net-value type="decimal">-100.0</net-value>
    <sales-tax-value type="decimal">-20.0</sales-tax-value>
    <involves-sales-tax type="boolean">true</involves-sales-tax>
    <is-interim-uk-vat type="boolean">false</is-interim-uk-vat>
    <total-value type="decimal">-120.0</total-value>
    <refunded-value type="decimal">0.0</refunded-value>
    <due-value type="decimal">-120.0</due-value>
    <status>Draft</status>
    <long-status>Draft</long-status>
    <omit-header type="boolean">false</omit-header>
    <payment-terms-in-days type="integer">0</payment-terms-in-days>
    <created-at type="dateTime">2020-01-21T11:42:38Z</created-at>
    <updated-at type="dateTime">2020-01-21T11:43:12Z</updated-at>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <contact-name>Nathan Barley</contact-name>
    <credit-note-items type="array">
      <credit-note-item>
        <url>https://api.freeagent.com/v2/invoice_items/1</url>
        <position type="integer">1</position>
        <description>Refund</description>
        <item-type>Hours</item-type>
        <price type="decimal">-100.0</price>
        <quantity type="decimal">1.0</quantity>
        <sales-tax-rate type="decimal">20.0</sales-tax-rate>
        <suffers-cis-deduction type="boolean">false</suffers-cis-deduction>
        <sales-tax-status>TAXABLE</sales-tax-status>
        <category>https://api.freeagent.com/v2/categories/001</category>
      </credit-note-item>
    </credit-note-items>
  </credit-note>
</freeagent>
```
Show as JSON

## Update a credit note

To update the status of a credit note you must use the status transitions to mark a credit note as `Draft`, `Sent` or `Cancelled`.

```http
PUT https://api.freeagent.com/v2/credit_notes/:id
```

Payload should have a root `credit_note` element, containing elements listed
under Credit Note Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a credit note

```http
DELETE https://api.freeagent.com/v2/credit_notes/:id
```

### Response

```http
Status: 200 OK
```

## Email a credit note

```http
POST https://api.freeagent.com/v2/credit_notes/:id/send_email
```

### Input

- `credit_note`(Hash)
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

```json
{
  "credit_note": {
    "email": {
      "body": "Test",
      "email_to_sender": false,
      "from": "Development Team <dev@freeagent.com>",
      "subject": "FreeAgent Central Credit Note INV015",
      "to": "raymondcarter@haag-hettinger.net"
    }
  }
}
```
Show as XML

```json
{
  "credit_note": {
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

```xml
<credit_note>
  <email>
    <use_template>true</use_template>
  </email>
</credit_note>
```
Show as JSON

### Response

```http
Status: 200 OK
```

## Mark credit note as sent

```http
PUT https://api.freeagent.com/v2/credit_notes/:id/transitions/mark_as_sent
```

Can also be used to re-open a credit note after it has been cancelled.

### Response

```http
Status: 200 OK
```

## Mark credit note as draft

```http
PUT https://api.freeagent.com/v2/credit_notes/:id/transitions/mark_as_draft
```

### Response

```http
Status: 200 OK
```