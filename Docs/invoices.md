# Invoices

*Minimum access level*: `Estimates and Invoices`, unless stated otherwise.

## Invoice Attributes

| Required | Attribute                | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Kind      |      |
| -------- | ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
|          | url                      | The unique identifier for the invoice                                                                                                                                                                                                                                                                                                                                                                                                                              | URI       |      |
|          | status                   | One of the following: `Draft`, `Scheduled To Email`, `Open`, `Zero Value`, `Overdue`, `Paid`, `Overpaid`, `Refunded`, `Written-off`, `Part written-off`                                                                                                                                                                                                                                                                                                            | String    |      |
|          | long_status              | Invoice status along with the due date as a relative date to 'today' For example: `Open – due in 1 day`, `Overdue – due 15 days ago`, `Paid on – 21 Mar 20`                                                                                                                                                                                                                                                                                                        | String    |      |
| ✔        | contact                  | The [contact](contacts.md) being invoiced                                                                                                                                                                                                                                                                                                                                                                                                                          | URI       |      |
|          | project                  | The [project](projects.md) being invoiced                                                                                                                                                                                                                                                                                                                                                                                                                          | URI       |      |
| ?        | property                 | The [property](properties.md) pertaining to this invoice. Only accepted and required for companies with type `UkUnincorporatedLandlord`.                                                                                                                                                                                                                                                                                                                           | URI       |      |
|          | include_timeslips        | One of the following: `null`, `billed_grouped_by_single_timeslip` (one line), `billed_grouped_by_timeslip` (separate lines), `billed_grouped_by_timeslip_task`, `billed_grouped_by_timeslip_date`                                                                                                                                                                                                                                                                  | String    |      |
|          | include_expenses         | One of the following: `null`, `billed_grouped_by_single_expense` (one line), `billed_grouped_by_expense` (separate lines) Includes expenses, bills and bank account entries                                                                                                                                                                                                                                                                                        | String    |      |
|          | include_estimates        | One of the following: `null`, `billed_grouped_by_single_estimate` (one line), `billed_grouped_by_estimate` (separate lines) Includes only open or approved estimates                                                                                                                                                                                                                                                                                               | String    |      |
|          | reference                | Invoice reference (using global invoice sequencing) If omitted, next invoice reference will be used                                                                                                                                                                                                                                                                                                                                                                | String    |      |
| ✔        | dated_on                 | Date of invoice in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                             | Date      |      |
|          | due_on                   | When invoice is due, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                                        | Date      |      |
| ✔        | payment_terms_in_days    | Set to zero to display 'Due on Receipt' on the invoice                                                                                                                                                                                                                                                                                                                                                                                                             | Integer   |      |
|          | currency                 | Invoice's [currency](currencies.md) Defaults to the company's native currency                                                                                                                                                                                                                                                                                                                                                                                      | String    |      |
|          | cis_rate                 | One of the following: `null`, Name of a [Construction Industry Scheme band](cis_bands.md)                                                                                                                                                                                                                                                                                                                                                                          | String \  | null |
|          | cis_deduction_rate       | Percentage of CIS deduction for the `cis_rate` set                                                                                                                                                                                                                                                                                                                                                                                                                 | Decimal   |      |
|          | cis_deduction            | Total CIS deduction for this invoice, in its currency                                                                                                                                                                                                                                                                                                                                                                                                              | Decimal   |      |
|          | cis_deduction_suffered   | CIS deduction already paid for this invoice, in its currency                                                                                                                                                                                                                                                                                                                                                                                                       | Decimal   |      |
|          | comments                 | Additional text added to the bottom of the invoice                                                                                                                                                                                                                                                                                                                                                                                                                 | String    |      |
|          | send_new_invoice_emails  | `true` to email this invoice automatically using your default template, `false` otherwise Can only set to `true` if templates exist                                                                                                                                                                                                                                                                                                                                | Boolean   |      |
|          | send_reminder_emails     | `true` to email payment reminders if the invoice goes unpaid, `false` otherwise Can only set to `true` if templates exist                                                                                                                                                                                                                                                                                                                                          | Boolean   |      |
|          | send_thank_you_emails    | `true` to email a Thank You once this invoice has been paid, `false` otherwise Can only set to `true` if templates exist                                                                                                                                                                                                                                                                                                                                           | Boolean   |      |
|          | discount_percent         | The discount applied across the whole invoice                                                                                                                                                                                                                                                                                                                                                                                                                      | Decimal   |      |
|          | contact_name             | The display name of the [contact](contacts.md) for this Invoice. This will always be the display name on the [contact](contacts.md) associated with the invoice, even if overriden by the `client_contact_name`.                                                                                                                                                                                                                                                   | String    |      |
|          | client_contact_name      | This name will override the default [contact](contacts.md) name (and `contact_name` parameter) on this invoice.                                                                                                                                                                                                                                                                                                                                                    | String    |      |
|          | payment_terms            | This will override the normal payment terms and invoice due date                                                                                                                                                                                                                                                                                                                                                                                                   | String    |      |
|          | po_reference             | This PO reference will override any PO set for the [project](projects.md)                                                                                                                                                                                                                                                                                                                                                                                          | String    |      |
|          | bank_account             | This [bank account](bank_accounts.md) will be used to display remittance advice on this invoice                                                                                                                                                                                                                                                                                                                                                                    | URI       |      |
|          | omit_header              | `true` to omit your logo and company address, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                                    | Boolean   |      |
|          | show_project_name        | `true` to display the [project](projects.md) name in the Other Information section, `false` otherwise                                                                                                                                                                                                                                                                                                                                                              | Boolean   |      |
|          | always_show_bic_and_iban | `true` to always display the BIC and IBAN numbers, if defined, on the invoice, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                   | Boolean   |      |
|          | ec_status                | Invoice's VAT status for reporting purposes. One of the following: `UK/Non-EC`, `EC Goods`, `EC Services`, `Reverse Charge`, `EC VAT MOSS` Please note that `EC Goods` and `EC Services` are no longer valid options if the invoice is dated 1/1/2021 or later and the company is based in Great Britain (but not Northern Ireland), following the UK's withdrawal from the EU. `Reverse Charge` is only a valid status if the invoice is dated 1/1/2021 or later. | String    |      |
|          | place_of_supply          | Place of supply when `ec_status` is [EC VAT MOSS](sales_tax.md#ec-vat-moss)                                                                                                                                                                                                                                                                                                                                                                                        | String    |      |
|          | net_value                | Net value                                                                                                                                                                                                                                                                                                                                                                                                                                                          | Decimal   |      |
|          | exchange_rate            | Rate at which invoice amount is converted into company's native [currency](currencies.md)                                                                                                                                                                                                                                                                                                                                                                          | Decimal   |      |
|          | involves_sales_tax       | `true` if [sales tax](sales_tax.md) applies to the invoice, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                                      | Boolean   |      |
|          | sales_tax_value          | Total value of [sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                                                                           | Decimal   |      |
|          | second_sales_tax_value   | [Universal accounts only] Total value of [second sales tax](sales_tax.md)                                                                                                                                                                                                                                                                                                                                                                                          | Decimal   |      |
|          | total_value              | Gross value                                                                                                                                                                                                                                                                                                                                                                                                                                                        | Decimal   |      |
|          | paid_value               | Amount paid off so far                                                                                                                                                                                                                                                                                                                                                                                                                                             | Decimal   |      |
|          | due_value                | Amount yet to be paid                                                                                                                                                                                                                                                                                                                                                                                                                                              | Decimal   |      |
|          | is_interim_uk_vat        | `true` if VAT status was `Registration Applied For` at invoice date, `false` otherwise                                                                                                                                                                                                                                                                                                                                                                             | Boolean   |      |
|          | paid_on                  | When the invoice was fully paid off, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                        | Date      |      |
|          | written_off_date         | When the invoice was written off, in `YYYY-MM-DD` format                                                                                                                                                                                                                                                                                                                                                                                                           | Date      |      |
|          | recurring_invoice        | The [recurring invoice](recurring_invoices.md) from which the invoice was generated                                                                                                                                                                                                                                                                                                                                                                                | URI       |      |
|          | payment_url              | Online payment URL if PayPal, GoCardless, Stripe or Tyl are enabled                                                                                                                                                                                                                                                                                                                                                                                                | URI       |      |
|          | payment_methods          | Flags to read/set online payment methods for the invoice. See [Payment Methods Notes](#payment-methods-notes).                                                                                                                                                                                                                                                                                                                                                     | Hash      |      |
|          | invoice_items            | Array of invoice item data structures. See [Invoice Item Attributes](#invoice-item-attributes).                                                                                                                                                                                                                                                                                                                                                                    | Array     |      |
|          | created_at               | Creation of the invoice resource (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                             | Timestamp |      |
|          | updated_at               | When the invoice resource was last updated (UTC)                                                                                                                                                                                                                                                                                                                                                                                                                   | Timestamp |      |

## Invoice Item Attributes

| Required | Attribute               | Description                                                                                                                                                                                           | Kind    |
| -------- | ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
|          | url                     | The unique identifier for the invoice item                                                                                                                                                            | URI     |
|          | position                | Position in the invoice, starting at 1                                                                                                                                                                | Decimal |
|          | item_type               | One of the following: `Hours`, `Days`, `Weeks`, `Months`, `Years`, `Products`, `Services`, `Training`, `Expenses`, `Comment`, `Bills`, `Discount`, `Credit`, `VAT`, `Stock` Leave blank for 'No Unit' | String  |
|          | quantity                | Quantity of the `item_type` Defaults to `0`                                                                                                                                                           | Decimal |
| ✔        | description             | Invoice item details                                                                                                                                                                                  | String  |
| ?        | price                   | Unit price Required if `invoice_item` is given and `item_type` is non time based                                                                                                                      | Decimal |
|          | sales_tax_rate          | One of the standard [sales tax](sales_tax.md) rates                                                                                                                                                   | Decimal |
|          | second_sales_tax_rate   | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                                                  | Decimal |
|          | sales_tax_status        | Indicates whether the item is `TAXABLE` or `EXEMPT` from [sales tax](sales_tax.md)                                                                                                                    | String  |
|          | second_sales_tax_status | [sales tax period](sales_tax_periods.md)defines a second sales tax                                                                                                                                    | String  |
| ?        | stock_item              | [Stock item](stock_items.md) being invoiced, if `item_type` is `Stock`                                                                                                                                | URI     |
|          | category                | [Accounting category](categories.md) of the invoice item                                                                                                                                              | URI     |
|          | project                 | The [project](projects.md) being invoiced                                                                                                                                                             | URI     |
| ✔        | id                      | ID of the invoice item to update                                                                                                                                                                      | Integer |
| ✔        | id                      | ID of the invoice item to delete                                                                                                                                                                      | Integer |
| ✔        | _destroy                | Should be equal to `1`                                                                                                                                                                                | Integer |

## Payment Methods Attributes

| Required | Attribute                   | Description                                                                                                                                                                                                                                                         | Kind    |
| -------- | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
|          | paypal                      | Payable online using [PayPal](https://www.freeagent.com/support/kb/integrations/invoice-payments-with-paypal/)                                                                                                                                                      | Boolean |
|          | gocardless_preauth          | Payable using a previously authorised [GoCardless Direct Debit Mandate](https://www.freeagent.com/support/kb/integrations/gocardless-taking-payments-with-a-direct-debit-mandate/). See also [taking payment](#take-payment-using-gocardless-direct-debit-mandate). | Boolean |
|          | gocardless_instant_bank_pay | Payable online using [GoCardless](https://support.freeagent.com/hc/en-gb/articles/26413965633810-Take-instant-bank-payments-using-GoCardless)                                                                                                                       | Boolean |
|          | stripe                      | Payable online using [Stripe](https://www.freeagent.com/support/kb/integrations/invoice-payments-with-stripe/)                                                                                                                                                      | Boolean |
|          | tyl                         | Payable online using [Tyl](https://support.freeagent.com/hc/en-gb/articles/5850863683602-Take-invoice-payments-with-Tyl-by-NatWest)                                                                                                                                 | Boolean |

## List all invoices

```http
GET https://api.freeagent.com/v2/invoices
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/invoices?view=recent_open_or_overdue
```

- `all`: Show all invoices (default)
- `recent_open_or_overdue`: Show only recent, open, or overdue invoices.
- `open`: Show only open invoices.
- `overdue`: Show only overdue invoices.
- `open_or_overdue`: Show only open or overdue invoices.
- `draft`: Show only draft invoices.
- `paid`: Show only paid invoices.
- `scheduled_to_email`: Show only invoices scheduled to email.
- `thank_you_emails`: Show only invoices with active thank you emails.
- `reminder_emails`: Show only invoices with active reminders.
- `last_N_months`: Show only invoices from the last `N` months.

#### Date Filters

```http
GET https://api.freeagent.com/v2/invoices?updated_since=2017-05-22T09:00:00.000Z
```

- `updated_since`

#### Sort Orders

```http
GET https://api.freeagent.com/v2/invoices?sort=updated_at
```

- `created_at`: Sort by the time the invoice was created (default).
- `updated_at`: Sort by the time the invoice was last modified.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/invoices?sort=-updated_at
```

### Response

```http
Status: 200 OK
```

```json
{ "invoices": [
  {
    "url":"https://api.freeagent.com/v2/invoices/1",
    "contact":"https://api.freeagent.com/v2/contacts/2",
    "dated_on":"2011-08-29",
    "due_on":"2011-09-28",
    "reference":"001",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "net_value":"0.0",
    "sales_tax_value":"0.0",
    "total_value": "200.0",
    "paid_value": "50.0",
    "due_value": "150.0",
    "status":"Open",
    "long_status":"Open – due in about 1 month",
    "comments":"An example invoice comment.",
    "omit_header":false,
    "send_thank_you_emails":false,
    "send_reminder_emails":false,
    "send_new_invoice_emails": false,
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "always_show_bic_and_iban": false,
    "payment_terms_in_days":30,
    "ec_status":"EC Goods",
    "payment_methods": {
      "paypal": true,
      "stripe": false
    },
    "created_at":"2011-08-29T00:00:00Z",
    "updated_at":"2011-08-29T00:00:00Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <invoices type="array">
    <invoice>
      <url>https://api.freeagent.com/v2/invoices/1</url>
      <contact>https://api.freeagent.com/v2/contacts/2</contact>
      <dated-on type="datetime">2011-08-29</dated-on>
      <due-on type="datetime">2011-09-28</due-on>
      <reference>001</reference>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <net-value type="decimal">0.0</net-value>
      <sales-tax-value type="decimal">0.0</sales-tax-value>
      <total-value type="decimal">200.0</total-value>
      <paid-value type="decimal">50.0</paid-value>
      <due-value type="decimal">150.0</due-value>
      <status>Open</status>
      <long-status>Open – due in about 1 month</long-status>
      <comments>An example invoice comment.</comments>
      <omit-header type="boolean">false</omit-header>
      <send-thank-you-emails type="boolean">false</send-thank-you-emails>
      <send-reminder-emails type="boolean">false</send-reminder-emails>
      <send-new-invoice-emails type="boolean">false</send-new-invoice-emails>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
      <payment-terms-in-days type="integer">30</payment-terms-in-days>
      <ec-status>EC Goods</ec-status>
      <payment-methods>
        <paypal type="boolean">true</paypal>
        <stripe type="boolean">false</stripe>
      </payment-methods>
      <created-at type="datetime">2011-08-29T00:00:00Z</created-at>
      <updated-at type="datetime">2011-08-29T00:00:00Z</updated-at>
    </invoice>
  </invoices>
</freeagent>
```
Show as JSON

### Optional attributes

- `payment_terms` string with custom payment terms only returned if set
- `client_contact_name` string returned if custom client name has been set
- `po_reference` shown if set
- `discount_percent` shown if set

## List all invoices with nested invoice items

You can include invoice items nested into the list of invoices which increases
request size but removes the need to request the invoices separately to see
invoice item information.

```http
GET https://api.freeagent.com/v2/invoices?nested_invoice_items=true
```

### Response

```http
Status: 200 OK
```

```json
{ "invoices": [
  {
    "url":"https://api.freeagent.com/v2/invoices/1",
    "contact":"https://api.freeagent.com/v2/contacts/2",
    "dated_on":"2011-08-29",
    "due_on":"2011-09-28",
    "reference":"001",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "net_value":"0.0",
    "sales_tax_value":"0.0",
    "total_value": "200.0",
    "paid_value": "50.0",
    "due_value": "150.0",
    "status":"Open",
    "long_status":"Open – due in about 1 month",
    "comments":"An example invoice comment.",
    "omit_header":false,
    "always_show_bic_and_iban": false,
    "send_thank_you_emails":false,
    "send_reminder_emails":false,
    "send_new_invoice_emails": false,
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "payment_terms_in_days":30,
    "ec_status":"EC Goods",
    "payment_methods": {
      "paypal": true,
      "stripe": false
    },
    "created_at":"2011-08-29T00:00:00Z",
    "updated_at":"2011-08-29T00:00:00Z",
    "invoice_items":[
      {
        "description":"Test InvoiceItem",
        "item_type":"Hours",
        "price":"0.0",
        "quantity":"0.0"
      }
    ]
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <invoices type="array">
    <invoice>
      <url>https://api.freeagent.com/v2/invoices/1</url>
      <contact>https://api.freeagent.com/v2/contacts/2</contact>
      <dated-on type="datetime">2011-08-29</dated-on>
      <due-on type="datetime">2011-09-28</due-on>
      <reference>001</reference>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <net-value type="decimal">0.0</net-value>
      <sales-tax-value type="decimal">0.0</sales-tax-value>
      <total-value type="decimal">200.0</total-value>
      <paid-value type="decimal">50.0</paid-value>
      <due-value type="decimal">150.0</due-value>
      <status>Open</status>
      <long-status>Open – due in about 1 month</long-status>
      <comments>An example invoice comment.</comments>
      <omit-header type="boolean">false</omit-header>
      <send-thank-you-emails type="boolean">false</send-thank-you-emails>
      <send-reminder-emails type="boolean">false</send-reminder-emails>
      <send-new-invoice-emails type="boolean">false</send-new-invoice-emails>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
      <payment-terms-in-days type="integer">30</payment-terms-in-days>
      <ec-status>EC Goods</ec-status>
      <payment-methods>
        <paypal type="boolean">true</paypal>
        <stripe type="boolean">false</stripe>
      </payment-methods>
      <created-at type="datetime">2011-08-29T00:00:00Z</created-at>
      <updated-at type="datetime">2011-08-29T00:00:00Z</updated-at>
      <invoice-items type="array">
        <invoice-item>
          <description>Test InvoiceItem</description>
          <item-type>Hours</item-type>
          <price type="decimal">0.0</price>
          <quantity type="decimal">0.0</quantity>
        </invoice-item>
      </invoice-items>
   </invoice>
  </invoices>
</freeagent>
```
Show as JSON

## Get a single invoice

```http
GET https://api.freeagent.com/v2/invoices/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "invoice":
  {
    "contact":"https://api.freeagent.com/v2/contacts/2",
    "project":"https://api.freeagent.com/v2/projects/3",
    "dated_on":"2011-08-29",
    "due_on":"2011-09-02",
    "reference":"003",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "net_value":"0.0",
    "total_value": "200.0",
    "paid_value": "50.0",
    "due_value": "150.0",
    "status":"Open",
    "long_status":"Open – due in 4 days",
    "comments":"An example invoice comment.",
    "omit_header":false,
    "always_show_bic_and_iban": false,
    "send_thank_you_emails":false,
    "send_reminder_emails":false,
    "send_new_invoice_emails": false,
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "payment_terms_in_days":5,
    "ec_status":"EC Goods",
    "payment_methods": {
      "paypal": true,
      "stripe": false
    },
    "created_at":"2011-08-29T00:00:00Z",
    "updated_at":"2011-08-29T00:00:00Z",
    "invoice_items":[
      {
        "description":"Test InvoiceItem",
        "item_type":"Hours",
        "price":"0.0",
        "quantity":"0.0"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <invoice>
    <contact>https://api.freeagent.com/v2/contacts/2</contact>
    <project>https://api.freeagent.com/v2/projects/3</project>
    <dated-on type="datetime">2011-08-29</dated-on>
    <due-on type="datetime">2011-09-02</due-on>
    <reference>003</reference>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <net-value type="decimal">0.0</net-value>
    <total-value type="decimal">200.0</total-value>
    <paid-value type="decimal">50.0</paid-value>
    <due-value type="decimal">150.0</due-value>
    <status>Open</status>
    <long-status>Open – due in 4 days</long-status>
    <comments>An example invoice comment.</comments>
    <omit-header type="boolean">false</omit-header>
    <send-thank-you-emails type="boolean">false</send-thank-you-emails>
    <send-reminder-emails type="boolean">false</send-reminder-emails>
    <send-new-invoice-emails type="boolean">false</send-new-invoice-emails>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
    <payment-terms-in-days type="integer">5</payment-terms-in-days>
    <ec-status>EC Goods</ec-status>
    <payment-methods>
      <paypal type="boolean">true</paypal>
      <stripe type="boolean">false</stripe>
    </payment-methods>
    <created-at type="datetime">2011-08-29T00:00:00Z</created-at>
    <updated-at type="datetime">2011-08-29T00:00:00Z</updated-at>
    <invoice-items type="array">
      <invoice-item>
        <description>Test InvoiceItem</description>
        <item-type>Hours</item-type>
        <price type="decimal">0.0</price>
        <quantity type="decimal">0.0</quantity>
      </invoice-item>
    </invoice-items>
  </invoice>
</freeagent>
```
Show as JSON

## Get a single invoice as PDF

```http
GET https://api.freeagent.com/v2/invoices/:id/pdf
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

## List all invoices related to a contact

```http
GET https://api.freeagent.com/v2/invoices?contact=https://api.freeagent.com/v2/contacts/2
```

## List all invoices related to a project

```http
GET https://api.freeagent.com/v2/invoices?project=https://api.freeagent.com/v2/projects/2
```

## Create an invoice

An invoice is always created with a status of `Draft`. You must use the status transitions to mark an invoice as `Draft`, `Sent`, `Scheduled` or `Cancelled`.

```http
POST https://api.freeagent.com/v2/invoices
```

Payload should have a root `invoice` element, containing elements listed
under Invoice Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/invoices/3
```

```json
{ "invoice":
  {
    "contact":"https://api.freeagent.com/v2/contacts/2",
    "dated_on":"2011-08-29",
    "due_on":"2011-09-02",
    "reference":"003",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "net_value":"0.0",
    "total_value": "200.0",
    "paid_value": "0.0",
    "due_value": "200.0",
    "status":"Draft",
    "long_status":"Draft",
    "omit_header":false,
    "always_show_bic_and_iban": false,
    "send_thank_you_emails":false,
    "send_reminder_emails":false,
    "send_new_invoice_emails": false,
    "bank_account": "https://api.freeagent.com/v2/bank_accounts/1",
    "payment_terms_in_days":5,
    "payment_methods": {
      "paypal": false,
      "stripe": false
    },
    "created_at":"2011-08-29T00:00:00Z",
    "updated_at":"2011-08-29T00:00:00Z",
    "invoice_items":[
      {
        "description":"Test InvoiceItem",
        "item_type":"Hours",
        "price":"0.0",
        "quantity":"0.0"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <invoice>
    <contact>https://api.freeagent.com/v2/contacts/2</contact>
    <dated-on type="datetime">2011-08-29</dated-on>
    <due-on type="datetime">2001-09-02</due-on>
    <reference>003</reference>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <net-value type="decimal">0.0</net-value>
    <total-value type="decimal">200.0</total-value>
    <paid-value type="decimal">0.0</paid-value>
    <due-value type="decimal">200.0</due-value>
    <status>Draft</status>
    <long-status>Draft</long-status>
    <omit-header type="boolean">false</omit-header>
    <send-thank-you-emails type="boolean">false</send-thank-you-emails>
    <send-reminder-emails type="boolean">false</send-reminder-emails>
    <send-new-invoice-emails type="boolean">false</send-new-invoice-emails>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <payment-terms-in-days type="integer">5</payment-terms-in-days>
    <payment-methods>
      <paypal type="boolean">false</paypal>
      <stripe type="boolean">false</stripe>
    </payment-methods>
    <created-at type="datetime">2011-08-29T00:00:00Z</created-at>
    <updated-at type="datetime">2011-08-29T00:00:00Z</updated-at>
    <invoice-items type="array">
      <invoice-item>
        <description>Test InvoiceItem</description>
        <item-type>Hours</item-type>
        <price type="decimal">0.0</price>
        <quantity type="decimal">0.0</quantity>
      </invoice-item>
    </invoice-items>
  </invoice>
</freeagent>
```
Show as JSON

## Duplicate an invoice

Invoices are always duplicated with a status of Draft, the next reference in the global invoice sequence and dated for today's date.

```http
POST https://api.freeagent.com/v2/invoices/:id/duplicate
```

### Response

```http
Status: 200 OK
```

The duplicated invoice will be returned in the body.

## Update an invoice

To update the status of an invoice you must use the status transitions to mark an invoice as `Draft`, `Sent`, `Scheduled` or `Cancelled`.

```http
PUT https://api.freeagent.com/v2/invoices/:id
```

Payload should have a root `invoice` element, containing elements listed
under Invoice Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete an invoice

```http
DELETE https://api.freeagent.com/v2/invoices/:id
```

### Response

```http
Status: 200 OK
```

## Email an invoice

```http
POST https://api.freeagent.com/v2/invoices/:id/send_email
```

### Input

- `invoice`(Hash)
    - `email`(Hash)
        - `to`
        - `from`- Needs to belong to a registered user and be in one of the following formats:
            - `John Doe <johndoe@example.com>`
            - `johndoe@example.com`
        - `subject`
        - `body`
        - `email_to_sender` (Boolean) - defaults to `true`
        - `attach_expense_receipts` (Boolean) - when rebilling expenses, setting this attribute to `true` will include expense attachments in the email
        - `attachments`- Array of email attachment hash data structures. Each attachment has a max size limit of 5MB. An email attachment data structure should have the following attributes:
            - `content_type` (String) - MIME-type
            - `data` (String) - Binary data of the file being attached encoded as base64
            - `file_name` (String)

If, instead of defining all e-mail attributes in your request, you wish to use an existing [e-mail template](https://support.freeagent.com/hc/en-gb/articles/115001219010-How-to-set-up-and-send-automatic-emails-for-new-invoices),
all you need to include in the request body is the `use_template` option set to `true`, i.e.

```json
{
  "invoice": {
    "email": {
      "use_template": true
    }
  }
}
```
Show as XML

```json
{
  "invoice": {
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
<invoice>
  <email>
    <use_template>true</use_template>
  </email>
</invoice>
```
Show as JSON

### Response

```http
Status: 200 OK
```

## Mark invoice as sent

```http
PUT https://api.freeagent.com/v2/invoices/:id/transitions/mark_as_sent
```

Can also be used to re-open an invoice after it has been cancelled.

### Response

```http
Status: 200 OK
```

## Mark invoice as scheduled

```http
PUT https://api.freeagent.com/v2/invoices/:id/transitions/mark_as_scheduled
```

### Response

```http
Status: 200 OK
```

## Mark invoice as draft

```http
PUT https://api.freeagent.com/v2/invoices/:id/transitions/mark_as_draft
```

### Response

```http
Status: 200 OK
```

## Mark invoice as cancelled

```http
PUT https://api.freeagent.com/v2/invoices/:id/transitions/mark_as_cancelled
```

Write-off an invoice as unpaid. Invoice must be sent and have a due date in the past.

### Response

```http
Status: 200 OK
```

## Take Payment using GoCardless Direct Debit Mandate

```http
POST https://api.freeagent.com/v2/invoices/:id/direct_debit
```

Only available for invoices which fit the following criteria:

- The invoice must be sent
- The invoice must have gocardless_preauth available and enabled as a payment method
- The invoice must not have already had a payment taken for it

### Response

```http
Status: 200 OK
```

## Get invoice timeline

```http
GET https://api.freeagent.com/v2/invoices/timeline
```

### Response

```http
Status: 200 OK
```

```json
{ "invoice_timeline_items":[
  {
    "reference":"007",
    "summary":"Payment: 007: \u00a314.40 received",
    "description":"resras",
    "dated_on":"2011-09-02",
    "amount":"14.4"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <invoice-timeline-items type="array">
    <invoice-timeline-item>
      <reference>007</reference>
      <summary>Payment: 007: &#163;14.40 received</summary>
      <description>New Invoice</description>
      <dated-on type="date">2011-09-02</dated-on>
      <amount type="decimal">14.4</amount>
    </invoice-timeline-item>
  </invoice-timeline-items>
</freeagent>
```
Show as JSON

## Payment Methods Notes

The hash of payment methods returned will contain different keys depending on which payment options are valid for the invoice.

A summary of the logic around this is:

- `paypal` - the company must have a PayPal bank account
- `gocardless_preauth` - the company must have enabled GoCardless and the contact must have an active Direct Debit agreement (currency must be GBP)
- `gocardless_instant_bank_pay` - the company must have the GoCardless integration enabled and the invoice currency must be GBP
- `stripe` - the company must have the Stripe integration enabled and the invoice currency must be supported by Stripe
- `tyl` - the company must have the Tyl integration enabled and the invoice currency must be GBP

When creating or updating an invoice, *any* option may be set, however if that option is not valid for the invoice then it will be ignored and not returned in the response.

### Example API Responses

`bool` in each case may be true/false to signify whether that payment option is enabled for the specific invoice.

For a company with PayPal enabled:

```json
{
  "payment_methods": {
    "paypal": bool
  }
}
```

For a company with GoCardless enabled, on a GBP invoice to a contact *with* a preauth agreement:

```json
{
  "payment_methods": {
    "gocardless_preauth": bool
  }
}
```

For a company with GoCardless enabled, on a USD invoice to a contact *with* a preauth agreement:

```json
{
  "payment_methods": {}
}
```

## Convert to credit note

Convert a draft negative invoice to a credit note.

```http
PUT https://api.freeagent.com/v2/invoices/:id/transitions/convert_to_credit_note
```

### Response

```http
Status: 200 OK
```

The converted credit note will be returned in the body.

## Default additional text

This API allows configuration of the additional text shown on all invoices issued by the company.

### Get default additional text

```http
GET https://api.freeagent.com/v2/invoices/default_additional_text
```

### Response

```http
Status: 200 OK
```

```json
{
  "default_additional_text": "Please pay within 21 working days"
}
```
Show as XML

```xml
<default-additional-text>Please pay within 21 working days</default-additional-text>
```
Show as JSON

### Update default additional text

```http
PUT https://api.freeagent.com/v2/invoices/default_additional_text
```

#### Example Request Body

```json
{
    "default_additional_text": "Pay within 7 days"
}
```
Show as XML

```xml
<default-additional-text>Pay within 7 days</default-additional-text>
```
Show as JSON

### Response

```http
Status: 200 OK
```

```json
{
  "default_additional_text": "Pay within 7 days"
}
```
Show as XML

```xml
<default-additional-text>Pay within 7 days</default-additional-text>
```
Show as JSON

### Delete default additional text

```http
DELETE https://api.freeagent.com/v2/invoices/default_additional_text
```

### Response

```http
Status: 200 OK
```