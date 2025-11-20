# Credit Note Reconciliations

*Minimum access level*: `Estimates and Invoices`, unless stated otherwise.

## Credit Note Reconciliation Attributes

| Required | Attribute     | Description                                                                               | Kind    |
| -------- | ------------- | ----------------------------------------------------------------------------------------- | ------- |
|          | url           | The unique identifier for the credit note reconciliation                                  | URI     |
| ✔        | gross_value   | The amount reconciled between the credit note and invoice                                 | Decimal |
|          | dated_on      | Date the reconciliation takes effect in `YYYY-MM-DD` format                               | Date    |
|          | exchange_rate | Rate at which invoice amount is converted into company's native [currency](currencies.md) | Decimal |
|          | currency      | Credit note reconciliation's [currency](currencies.md)                                    | String  |
| ✔        | invoice       | [Invoice](invoices.md) being reconciled                                                   | URI     |
| ✔        | credit_note   | [Credit note](credit_note.md) being reconciled                                            | URI     |

## List all credit note reconciliations

```http
GET https://api.freeagent.com/v2/credit_note_reconciliations
```

### Input

#### Date Filters

```json
GET https://api.freeagent.com/v2/credit_note_reconciliations?updated_since=2017-05-22T09:00:00.000Z
GET https://api.freeagent.com/v2/credit_note_reconciliations?from_date=2017-05-22
GET https://api.freeagent.com/v2/credit_note_reconciliations?to_date=2017-05-22
```

- `updated_since`
- `from_date`
- `to_date`

### Response

```http
Status: 200 OK
```

```json
{
  "credit_note_reconciliations": [
    {
      "url":"https://api.freeagent.com/v2/credit_note_reconciliations/1",
      "gross_value":"100.0",
      "dated_on":"2020-06-29",
      "currency":"GBP",
      "exchange_rate":"1.0",
      "invoice": "https://api.freeagent.com/v2/invoices/1",
      "credit_note": "https://api.freeagent.com/v2/credit_notes/1",
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
      <url>https://api.freeagent.com/v2/credit_note_reconciliations/1</url>
      <gross-value type="decimal">100.0</gross-value>
      <dated-on type="datetime">2020-06-29</dated-on>
      <currency>GBP</currency>
      <exchange-rate type="decimal">1.0</exchange-rate>
      <invoice>https://api.freeagent.com/v2/invoices/1</bank-account>
      <credit-note>https://api.freeagent.com/v2/credit_notes/1</credit-note>
    </credit-note>
  </credit-notes>
</freeagent>
```
Show as JSON

## Get a single credit note reconciliation

```http
GET https://api.freeagent.com/v2/credit_note_reconciliations/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "credit_note_reconciliations": {
    "url":"https://api.freeagent.com/v2/credit_note_reconciliations/1",
    "gross_value":"100.0",
    "dated_on":"2020-06-29",
    "currency":"GBP",
    "exchange_rate":"1.0",
    "invoice": "https://api.freeagent.com/v2/invoices/1",
    "credit_note": "https://api.freeagent.com/v2/credit_notes/1",
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit-note-reconciliation>
    <url>https://api.freeagent.com/v2/credit_note_reconciliations/1</url>
    <gross-value type="decimal">100.0</gross-value>
    <dated-on type="datetime">2020-06-29</dated-on>
    <currency>GBP</currency>
    <exchange-rate type="decimal">1.0</exchange-rate>
    <invoice>https://api.freeagent.com/v2/invoices/1</bank-account>
    <credit-note>https://api.freeagent.com/v2/credit_notes/1</credit-note>
  </credit-note-reconciliation>
</freeagent>
```
Show as JSON

## Create a credit note reconciliation

```http
POST https://api.freeagent.com/v2/credit_note_reconciliations
```

Payload should have a root `credit_note_reconciliation` element, containing elements listed
under Credit Note Reconciliation Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/credit_note_reconciliations/3
```

```json
{ "credit_note_reconciliation":
  {
    "url": "https://api.freeagent.com/v2/credit_note_reconciliations/2",
    "gross_value": "3.0",
    "dated_on": "2020-08-10",
    "currency": "GBP",
    "exchange_rate":"1.0",
    "invoice":"https://api.freeagent.com/v2/invoices/1",
    "credit_note":"https://api.freeagent.com/v2/credit_notes/2",
    "updated_at":"2020-08-10T15:06:28.225Z",
    "created_at":"2020-08-10T15:06:28.225Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <credit_note_reconciliation>
    <created_at>2020-08-10T15:06:28.225Z</created_at>
    <credit_note>https://api.freeagent.com/v2/credit_notes/2</credit_note>
    <currency>GBP</currency>
    <dated_on>2020-08-10</dated_on>
    <exchange_rate>1.0</exchange_rate>
    <gross_value>3.0</gross_value>
    <invoice>https://api.freeagent.com/v2/invoices/1</invoice>
    <updated_at>2020-08-10T15:06:28.225Z</updated_at>
    <url>https://api.freeagent.com/v2/credit_note_reconciliations/2</url>
  </credit_note_reconciliation>
</freeagent>
```
Show as JSON

## Update a credit note reconciliation

```http
PUT https://api.freeagent.com/v2/credit_note_reconciliations/:id
```

Payload should have a root `credit_note_reconciliation` element, containing elements listed
under Credit Note Reconciliation Attributes.

### Response

```http
Status: 200 OK
```

## Delete a credit note reconciliation

```http
DELETE https://api.freeagent.com/v2/credit_note_reconciliations/:id
```

### Response

```http
Status: 200 OK
```