# Corporation Tax Returns

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

This endpoint allows API applications to retrieve information on a company's Corporation Tax Returns, including amount due, deadlines and filing/payment status.

## Attributes

| Attribute        | Description                                                                                                                                                    | Kind     |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- |
| url              | Unique identifier for the record                                                                                                                               | URI      |
| period_ends_on   | The end date of the period covered by the return                                                                                                               | Date     |
| period_starts_on | The start date of the period covered by the return                                                                                                             | Date     |
| amount_due       | Amount to be paid by `payment_due_on`                                                                                                                          | Decimal  |
| payment_due_on   | The date on which payment is due                                                                                                                               | Date     |
| payment_status   | For returns which require payment, one of `unpaid` or `marked_as_paid`. `paid` is reserved for future use. Key is omitted if there are no payments to be made. | String   |
| filing_due_on    | The date on which submission is due                                                                                                                            | Date     |
| filing_status    | One of `draft`, `unfiled`, `pending`, `rejected`, `filed` or `marked_as_filed`.                                                                                | String   |
| filed_at         | For returns which have been filed online, the date and time at which that happened                                                                             | Datetime |
| filed_reference  | For returns which have been filed online, the IRMark returned by the submission                                                                                | String   |

## List Corporation Tax Returns for a company

```http
GET https://api.freeagent.com/v2/corporation_tax_returns
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_returns": [
        {
            "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
            "period_ends_on": "2022-12-31",
            "period_starts_on": "2022-01-01",
            "amount_due": "0.0",
            "payment_due_on": "2023-10-01",
            "filing_due_on": "2023-12-31",
            "filing_status": "filed",
            "filed_at": "2023-10-09T17:13:34.119+00:00",
            "filed_reference": "4CQXAEAVNA656F6XM3DHYBF6Y37I3S43"
        },
        {
            "url": "https://api.freeagent.com/v2/corporation_tax_returns/2023-12-31",
            "period_ends_on": "2023-12-31",
            "period_starts_on": "2023-01-01",
            "amount_due": "2724.03",
            "payment_due_on": "2024-10-01",
            "payment_status": "unpaid",
            "filing_due_on": "2024-12-31",
            "filing_status": "draft"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-returns type="array">
        <corporation-tax-return>
            <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
            <period-ends-on type="date">2022-12-31</period-ends-on>
            <period-starts-on type="date">2022-01-01</period-starts-on>
            <amount-due type="decimal">0.0</amount-due>
            <payment-due-on type="date">2023-10-01</payment-due-on>
            <filing-due-on type="date">2023-12-31</filing-due-on>
            <filing-status>filed</filing-status>
            <filed-at type="dateTime">2023-10-09T17:13:34+00:00</filed-at>
            <filed-reference>4CQXAEAVNA656F6XM3DHYBF6Y37I3S43</filed-reference>
        </corporation-tax-return>
        <corporation-tax-return>
            <url>https://api.freeagent.com/v2/corporation_tax_returns/2023-12-31</url>
            <period-ends-on type="date">2023-12-31</period-ends-on>
            <period-starts-on type="date">2023-01-01</period-starts-on>
            <amount-due type="decimal">2724.03</amount-due>
            <payment-due-on type="date">2024-10-01</payment-due-on>
            <payment-status>unpaid</payment-status>
            <filing-due-on type="date">2024-12-31</filing-due-on>
            <filing-status>draft</filing-status>
        </corporation-tax-return>
    </corporation-tax-returns>
</freeagent>
```
Show as JSON

## Fetch details for a Corporation Tax Return

```http
GET https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_return": {
        "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "amount_due": "0.0",
        "payment_due_on": "2023-10-01",
        "filing_due_on": "2023-12-31",
        "filing_status": "filed",
        "filed_at": "2023-10-09T17:13:34.119+00:00",
        "filed_reference": "4CQXAEAVNA656F6XM3DHYBF6Y37I3S43"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-return>
        <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <amount-due type="decimal">0.0</amount-due>
        <payment-due-on type="date">2023-10-01</payment-due-on>
        <filing-due-on type="date">2023-12-31</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-09T17:13:34+00:00</filed-at>
        <filed-reference>4CQXAEAVNA656F6XM3DHYBF6Y37I3S43</filed-reference>
    </corporation-tax-return>
</freeagent>
```
Show as JSON

## Mark a Corporation Tax Return as Filed

*Minimum access level*: `Full Access` for non practice-managed companies, `Account Manager` for practice-managed companies.

```http
PUT https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on/mark_as_filed
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_return": {
        "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "amount_due": "0.0",
        "payment_due_on": "2023-10-01",
        "filing_due_on": "2023-12-31",
        "filing_status": "marked_as_filed",
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-return>
        <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <amount-due type="decimal">0.0</amount-due>
        <payment-due-on type="date">2023-10-01</payment-due-on>
        <filing-due-on type="date">2023-12-31</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </corporation-tax-return>
</freeagent>
```
Show as JSON

## Mark a Corporation Tax Return as Unfiled

*Minimum access level*: `Full Access` for non practice-managed companies, `Account Manager` for practice-managed companies.

```http
PUT https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on/mark_as_unfiled
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_return": {
        "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "amount_due": "0.0",
        "payment_due_on": "2023-10-01",
        "filing_due_on": "2023-12-31",
        "filing_status": "unfiled",
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-return>
        <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <amount-due type="decimal">0.0</amount-due>
        <payment-due-on type="date">2023-10-01</payment-due-on>
        <filing-due-on type="date">2023-12-31</filing-due-on>
        <filing-status>unfiled</filing-status>
    </corporation-tax-return>
</freeagent>
```
Show as JSON

## Mark a Corporation Tax Return as Paid

```http
PUT https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on/mark_as_paid
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_return": {
        "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "amount_due": "5000.0",
        "payment_due_on": "2023-10-01",
        "payment_status": "marked_as_paid",
        "filing_due_on": "2023-12-31",
        "filing_status": "filed",
        "filed_at": "2023-10-09T17:13:34.119+00:00",
        "filed_reference": "4CQXAEAVNA656F6XM3DHYBF6Y37I3S43"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-return>
        <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <amount-due type="decimal">5000.0</amount-due>
        <payment-due-on type="date">2023-10-01</payment-due-on>
        <payment-status>marked_as_paid</payment-status>
        <filing-due-on type="date">2023-12-31</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-09T17:13:34+00:00</filed-at>
        <filed-reference>4CQXAEAVNA656F6XM3DHYBF6Y37I3S43</filed-reference>
    </corporation-tax-return>
</freeagent>
```
Show as JSON

## Mark a Corporation Tax Return as Unpaid

```http
PUT https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on/mark_as_unpaid
```

### Response

```http
Status: 200 OK
```

```json
{
    "corporation_tax_return": {
        "url": "https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "amount_due": "5000.0",
        "payment_due_on": "2023-10-01",
        "payment_status": "unpaid",
        "filing_due_on": "2023-12-31",
        "filing_status": "filed",
        "filed_at": "2023-10-09T17:13:34.119+00:00",
        "filed_reference": "4CQXAEAVNA656F6XM3DHYBF6Y37I3S43"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <corporation-tax-return>
        <url>https://api.freeagent.com/v2/corporation_tax_returns/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <amount-due type="decimal">5000.0</amount-due>
        <payment-due-on type="date">2023-10-01</payment-due-on>
        <payment-status>unpaid</payment-status>
        <filing-due-on type="date">2023-12-31</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-09T17:13:34+00:00</filed-at>
        <filed-reference>4CQXAEAVNA656F6XM3DHYBF6Y37I3S43</filed-reference>
    </corporation-tax-return>
</freeagent>
```
Show as JSON