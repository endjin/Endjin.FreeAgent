# VAT Returns

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

This endpoint allows API applications to retrieve information on a company's VAT Returns, including details on payments due and filing status.

## Attributes

| Attribute        | Description                                                                                 | Kind     |
| ---------------- | ------------------------------------------------------------------------------------------- | -------- |
| url              | Unique identifier for the record                                                            | URI      |
| period_ends_on   | The end date of the period covered by the return                                            | Date     |
| period_starts_on | The start date of the period covered by the return                                          | Date     |
| payments         | An array of [Payments](#payment-attributes)                                                 | Array    |
| filing_due_on    | The date on which submission is due                                                         | Date     |
| filing_status    | One of `unfiled`, `pending`, `rejected`, `filed` or `marked_as_filed`.                      | String   |
| filed_at         | For returns which have been filed online, the date and time at which that happened          | Datetime |
| filed_reference  | For returns which have been filed online, the form bundle number returned by the submission | String   |

## Payment Attributes

| Attribute  | Description                                                                                                                 | Kind    |
| ---------- | --------------------------------------------------------------------------------------------------------------------------- | ------- |
| label      | A human-readable name for the payment                                                                                       | String  |
| due_on     | The date on which a payment is due                                                                                          | Date    |
| amount_due | The amount due on that date. May be negative in the case of a refund.                                                       | Decimal |
| status     | One of `unpaid` or `marked_as_paid`. `paid` is reserved for future use. Key is omitted if `amount_due` is zero or negative. | String  |

## List VAT Returns for a Company

```http
GET https://api.freeagent.com/v2/vat_returns
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_returns": [
        {
            "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
            "period_ends_on": "2023-07-31",
            "period_starts_on": "2023-05-01",
            "payments": [
                {
                    "label": "Payment Due",
                    "amount_due": "5450.91",
                    "due_on": "2023-09-07",
                    "status": "unpaid"
                }
            ],
            "filing_due_on": "2023-09-07",
            "filing_status": "filed",
            "filed_at": "2023-10-10T10:32:49.000+00:00",
            "filed_reference": "00000000001"
        },
        {
            "url": "https://api.freeagent.com/v2/vat_returns/2023-10-31",
            "period_ends_on": "2023-10-31",
            "period_starts_on": "2023-08-01",
            "payments": [
                {
                    "label": "Refund Due",
                    "amount_due": "-62.89",
                    "due_on": "2023-12-07"
                }
            ],
            "filing_due_on": "2023-12-07",
            "filing_status": "unfiled"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-returns type="array">
        <vat-return>
            <url>https://api.freeagent.com/v2/vat_returns/2023-07-31</url>
            <period-ends-on type="date">2023-07-31</period-ends-on>
            <period-starts-on type="date">2023-05-01</period-starts-on>
            <payments type="array">
                <payment>
                    <label>Payment Due</label>
                    <amount-due type="decimal">5450.91</amount-due>
                    <due-on type="date">2023-09-07</due-on>
                    <status>unpaid</status>
                </payment>
            </payments>
            <filing-due-on type="date">2023-09-07</filing-due-on>
            <filing-status>filed</filing-status>
            <filed-at type="dateTime">2023-10-10T10:32:49+00:00</filed-at>
            <filed-reference>00000000001</filed-reference>
        </vat-return>
        <vat-return>
            <url>https://api.freeagent.com/v2/vat_returns/2023-10-31</url>
            <period-ends-on type="date">2023-10-31</period-ends-on>
            <period-starts-on type="date">2023-08-01</period-starts-on>
            <payments type="array">
                <payment>
                    <label>Refund Due</label>
                    <amount-due type="decimal">-62.89</amount-due>
                    <due-on type="date">2023-12-07</due-on>
                </payment>
            </payments>
            <filing-due-on type="date">2023-12-07</filing-due-on>
            <filing-status>unfiled</filing-status>
        </vat-return>
    </vat-returns>
</freeagent>
```
Show as JSON

## Fetch details for a VAT Return

```http
GET https://api.freeagent.com/v2/vat_returns/:period_ends_on
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_return": {
        "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
        "period_ends_on": "2023-07-31",
        "period_starts_on": "2023-05-01",
        "payments": [
            {
                "label": "Payment Due",
                "amount_due": "5450.91",
                "due_on": "2023-09-07",
                "status": "unpaid"
            }
        ],
        "filing_due_on": "2023-09-07",
        "filing_status": "filed",
        "filed_at": "2023-10-10T10:32:49.000+00:00",
        "filed_reference": "00000000001"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-return>
        <url>https://api.freeagent.com/v2/vat_returns/2023-07-31</url>
        <period-ends-on type="date">2023-07-31</period-ends-on>
        <period-starts-on type="date">2023-05-01</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment Due</label>
                <amount-due type="decimal">5450.91</amount-due>
                <due-on type="date">2023-09-07</due-on>
                <status>unpaid</status>
            </payment>
        </payments>
        <filing-due-on type="date">2023-09-07</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-10T10:32:49+00:00</filed-at>
        <filed-reference>00000000001</filed-reference>
    </vat-return>
</freeagent>
```
Show as JSON

## Mark a VAT Return as Filed

*Minimum access level*: `Full Access`

```http
PUT https://api.freeagent.com/v2/vat_returns/:period_ends_on/mark_as_filed
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_return": {
        "url": "https://api.freeagent.com/v2/vat_returns/2023-10-31",
        "period_ends_on": "2023-10-31",
        "period_starts_on": "2023-08-01",
        "payments": [
            {
                "label": "Refund Due",
                "amount_due": "-62.89",
                "due_on": "2023-12-07"
            }
        ],
        "filing_due_on": "2023-12-07",
        "filing_status": "marked_as_filed"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-return>
        <url>https://api.freeagent.com/v2/vat_returns/2023-10-31</url>
        <period-ends-on type="date">2023-10-31</period-ends-on>
        <period-starts-on type="date">2023-08-01</period-starts-on>
        <payments type="array">
            <payment>
                <label>Refund Due</label>
                <amount-due type="decimal">-62.89</amount-due>
                <due-on type="date">2023-12-07</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2023-12-07</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </vat-return>
</freeagent>
```
Show as JSON

## Mark a VAT Return as Unfiled

*Minimum access level*: `Full Access`

```http
PUT https://api.freeagent.com/v2/vat_returns/:period_ends_on/mark_as_unfiled
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_return": {
        "url": "https://api.freeagent.com/v2/vat_returns/2023-10-31",
        "period_ends_on": "2023-10-31",
        "period_starts_on": "2023-08-01",
        "payments": [
            {
                "label": "Refund Due",
                "amount_due": "-62.89",
                "due_on": "2023-12-07"
            }
        ],
        "filing_due_on": "2023-12-07",
        "filing_status": "unfiled"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-return>
        <url>https://api.freeagent.com/v2/vat_returns/2023-10-31</url>
        <period-ends-on type="date">2023-10-31</period-ends-on>
        <period-starts-on type="date">2023-08-01</period-starts-on>
        <payments type="array">
            <payment>
                <label>Refund Due</label>
                <amount-due type="decimal">-62.89</amount-due>
                <due-on type="date">2023-12-07</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2023-12-07</filing-due-on>
        <filing-status>unfiled</filing-status>
    </vat-return>
</freeagent>
```
Show as JSON

## Mark a VAT Return Payment as Paid

```http
PUT https://api.freeagent.com/v2/vat_returns/:period_ends_on/payments/:payment_date/mark_as_paid
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_return": {
        "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
        "period_ends_on": "2023-07-31",
        "period_starts_on": "2023-05-01",
        "payments": [
            {
                "label": "Payment Due",
                "amount_due": "5450.91",
                "due_on": "2023-09-07",
                "status": "marked_as_paid"
            }
        ],
        "filing_due_on": "2023-09-07",
        "filing_status": "filed",
        "filed_at": "2023-10-10T10:32:49.000+00:00",
        "filed_reference": "00000000001"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-return>
        <url>https://api.freeagent.com/v2/vat_returns/2023-07-31</url>
        <period-ends-on type="date">2023-07-31</period-ends-on>
        <period-starts-on type="date">2023-05-01</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment Due</label>
                <amount-due type="decimal">5450.91</amount-due>
                <due-on type="date">2023-09-07</due-on>
                <status>marked_as_paid</status>
            </payment>
        </payments>
        <filing-due-on type="date">2023-09-07</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-10T10:32:49+00:00</filed-at>
        <filed-reference>00000000001</filed-reference>
    </vat-return>
</freeagent>
```
Show as JSON

## Mark a VAT Return Payment as Unpaid

```http
PUT https://api.freeagent.com/v2/vat_returns/:period_ends_on/payments/:payment_date/mark_as_unpaid
```

### Response

```http
Status: 200 OK
```

```json
{
    "vat_return": {
        "url": "https://api.freeagent.com/v2/vat_returns/2023-07-31",
        "period_ends_on": "2023-07-31",
        "period_starts_on": "2023-05-01",
        "payments": [
            {
                "label": "Payment Due",
                "amount_due": "5450.91",
                "due_on": "2023-09-07",
                "status": "unpaid"
            }
        ],
        "filing_due_on": "2023-09-07",
        "filing_status": "filed",
        "filed_at": "2023-10-10T10:32:49.000+00:00",
        "filed_reference": "00000000001"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <vat-return>
        <url>https://api.freeagent.com/v2/vat_returns/2023-07-31</url>
        <period-ends-on type="date">2023-07-31</period-ends-on>
        <period-starts-on type="date">2023-05-01</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment Due</label>
                <amount-due type="decimal">5450.91</amount-due>
                <due-on type="date">2023-09-07</due-on>
                <status>unpaid</status>
            </payment>
        </payments>
        <filing-due-on type="date">2023-09-07</filing-due-on>
        <filing-status>filed</filing-status>
        <filed-at type="dateTime">2023-10-10T10:32:49+00:00</filed-at>
        <filed-reference>00000000001</filed-reference>
    </vat-return>
</freeagent>
```
Show as JSON