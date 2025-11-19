# Recurring Invoices

*Minimum access level*: `Estimates and Invoices`, unless stated otherwise.

## Attributes

The attributes below are in addition to the [Invoice Attributes](invoices.md#invoice-attributes).

| Attribute          | Description                                                                                                                              | Kind   |
| ------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- | ------ |
| url                | The unique identifier for the recurring invoice                                                                                          | URI    |
| frequency          | One of the following: `Weekly`, `Two Weekly`, `Four Weekly`, `Monthly`, `Two Monthly`, `Quarterly`, `Biannually`, `Annually`, `2-Yearly` | String |
| recurring_status   | One of the following: `Draft`, `Active`                                                                                                  | String |
| recurring_end_date | When the recurring ends, in `YYYY-MM-DD` format Blank if recurring forever                                                               | Date   |
| next_recurs_on     | When the invoice recurs next, in `YYYY-MM-DD` format                                                                                     | Date   |

## List all recurring invoices

```http
GET https://api.freeagent.com/v2/recurring_invoices
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/recurring_invoices?view=draft
```

- `draft`: Show only draft recurring invoices.
- `active`: Show only active recurring invoices.
- `inactive`: Show only inactive recurring invoices.

### Response

```http
Status: 200 OK
```

```json
{ "recurring_invoices": [
  {
    "url": "https://api.freeagent.com/v2/recurring_invoices/1",
    "contact": "https://api.freeagent.com/v2/contacts/1",
    "contact_name": "Nathan Barley",
    "dated_on": "2012-02-29",
    "frequency": "Weekly",
    "next_recurs_on": "2012-03-07",
    "recurring_end_date": "2012-05-16",
    "recurring_status": "Draft",
    "reference": "002",
    "currency": "GBP",
    "exchange_rate": "1.0",
    "net_value": "2.0",
    "sales_tax_value": "0.4",
    "total_value": "2.4",
    "omit_header": false,
    "always_show_bic_and_iban": false,
    "payment_terms_in_days": 30,
    "payment_methods": {
      "paypal": true,
      "stripe": false
    }
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <recurring-invoices type="array">
    <recurring-invoice>
      <url>https://api.freeagent.com/v2/recurring_invoices/1</url>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <contact_name>Nathan Barley</contact_name>
      <dated-on type="date">2012-02-29</dated-on>
      <frequency>Weekly</frequency>
      <next-recurs-on type="date">2012-03-07</next-recurs-on>
      <recurring-end-date type="date">2012-05-16</recurring-end-date>
      <recurring-status>Draft</recurring-status>
      <reference>001</reference>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <net-value type="decimal">2.0</net-value>
      <sales-tax-value type="decimal">0.4</sales-tax-value>
      <total-value type="decimal">2.4</total-value>
      <omit-header type="boolean">false</omit-header>
      <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
      <payment-terms-in-days type="integer">30</payment-terms-in-days>
      <payment-methods>
        <paypal type="boolean">true</paypal>
        <stripe type="boolean">false</stripe>
      </payment-methods>
    </recurring-invoice>
  </recurring-invoices>
</freeagent>
```
Show as JSON

## Get a single recurring invoice

```http
GET https://api.freeagent.com/v2/recurring_invoices/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "recurring_invoice": {
  "url": "https://api.freeagent.com/v2/recurring_invoices/1",
    "contact": "https://api.freeagent.com/v2/contacts/1",
    "contact_name": "Nathan Barley",
    "dated_on": "2012-02-29",
    "frequency": "Weekly",
    "next_recurs_on": "2012-03-07",
    "recurring_end_date": "2012-05-16",
    "recurring_status": "Draft",
    "reference": "002",
    "currency": "GBP",
    "exchange_rate": "1.0",
    "net_value": "2.0",
    "sales_tax_value": "0.4",
    "total_value": "2.4",
    "omit_header": false,
    "always_show_bic_and_iban": false,
    "payment_terms_in_days": 30,
    "invoice_items": [
    {
      "url": "https://api.freeagent.com/v2/invoice_items/1",
      "position": 1,
      "description": "Item",
      "item_type": "Hours",
      "price": "2.0",
      "quantity": "1.0",
      "sales_tax_rate": "20.0",
      "sales_tax_status": "TAXABLE",
      "category": "https://api.freeagent.com/v2/categories/001"
    }
  ],
  "payment_methods": {
    "paypal": true,
    "stripe": false
  }
}}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <recurring-invoice>
    <url>https://api.freeagent.com/v2/recurring_invoices/1</url>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <contact_name>Nathan Barley</contact_name>
    <dated-on type="date">2012-02-29</dated-on>
    <frequency>Weekly</frequency>
    <next-recurs-on type="date">2012-03-07</next-recurs-on>
    <recurring-end-date type="date">2012-05-16</recurring-end-date>
    <recurring-status>Draft</recurring-status>
    <reference>002</reference>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <net-value type="decimal">2.0</net-value>
    <sales-tax-value type="decimal">0.4</sales-tax-value>
    <total-value type="decimal">2.4</total-value>
    <omit-header type="boolean">false</omit-header>
    <always-show-bic-and-iban type="boolean">false</always-show-bic-and-iban>
    <payment-terms-in-days type="integer">30</payment-terms-in-days>
    <invoice-items type="array">
      <invoice-item>
        <url>https://api.freeagent.com/v2/invoice_items/1</url>
        <position type="integer">1</position>
        <description>Item</description>
        <item-type>Hours</item-type>
        <price type="decimal">2.0</price>
        <quantity type="decimal">1.0</quantity>
        <sales-tax-rate type="decimal">20.0</sales-tax-rate>
        <category>https://api.freeagent.com/v2/categories/001</category>
      </invoice-item>
    </invoice-items>
    <payment-methods>
      <paypal type="boolean">true</paypal>
      <stripe type="boolean">false</stripe>
    </payment-methods>
  </recurring-invoice>
</freeagent>
```
Show as JSON

## List all recurring invoices related to a contact

```http
GET https://api.freeagent.com/v2/recurring_invoices?contact=https://api.freeagent.com/v2/contacts/:id
```