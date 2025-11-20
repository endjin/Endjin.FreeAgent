# Self Assessment Returns

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

This endpoint allows API applications to retrieve information on a user's Self Assessment Returns, including details on payments due and filing status.

## Attributes

| Attribute        | Description                                                                                   | Kind     |
| ---------------- | --------------------------------------------------------------------------------------------- | -------- |
| url              | Unique identifier for the record                                                              | URI      |
| period_ends_on   | The end date of the period covered by the return                                              | Date     |
| period_starts_on | The start date of the period covered by the return                                            | Date     |
| payments         | An array of [Payments](#payment-attributes)                                                   | Array    |
| filing_due_on    | The date on which submission is due                                                           | Date     |
| filing_status    | One of `unfiled`, `pending`, `rejected`, `provisionally_filed`, `filed` or `marked_as_filed`. | String   |
| filed_at         | For returns which have been filed online, the date and time at which that happened            | Datetime |
| filed_reference  | For returns which have been filed online, the IRMark returned by the submission               | String   |

## Payment Attributes

| Attribute  | Description                                                                                                                 | Kind    |
| ---------- | --------------------------------------------------------------------------------------------------------------------------- | ------- |
| label      | A human-readable name for the payment                                                                                       | String  |
| due_on     | The date on which a payment is due                                                                                          | Date    |
| amount_due | The amount due on that date. May be negative in the case of a balancing payment.                                            | Decimal |
| status     | One of `unpaid` or `marked_as_paid`. `paid` is reserved for future use. Key is omitted if `amount_due` is zero or negative. | String  |

## List Self Assessment Returns for a User

```http
GET https://api.freeagent.com/v2/users/:user_id/self_assessment_returns
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_returns": [
        {
            "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05",
            "period_ends_on": "2022-04-05",
            "period_starts_on": "2021-04-06",
            "payments": [
                {
                    "label": "Balancing Payment",
                    "amount_due": "-2250.0",
                    "due_on": "2023-01-31"
                }
            ],
            "filing_due_on": "2023-01-31",
            "filing_status": "filed",
            "filed_at": "2023-10-10T10:32:49.000+00:00",
            "filed_reference": "YFM7CAZY2AQXUCRCQJOVS45UZUS4JWQN"
        },
        {
            "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2023-04-05",
            "period_ends_on": "2023-04-05",
            "period_starts_on": "2022-04-06",
            "payments": [
                {
                    "label": "Balancing Payment",
                    "amount_due": "6181.82",
                    "due_on": "2024-01-31",
                    "status": "unpaid"
                }
            ],
            "filing_due_on": "2024-01-31",
            "filing_status": "unfiled"
        },
        {
            "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
            "period_ends_on": "2024-04-05",
            "period_starts_on": "2023-04-06",
            "payments": [
                {
                    "label": "Payment on Account",
                    "amount_due": "3009.01",
                    "due_on": "2024-01-31",
                    "status": "unpaid"
                },
                {
                    "label": "Payment on Account",
                    "amount_due": "3009.01",
                    "due_on": "2024-07-31",
                    "status": "unpaid"
                },
                {
                    "label": "Balancing Payment",
                    "amount_due": "-6018.02",
                    "due_on": "2025-01-31"
                }
            ],
            "filing_due_on": "2025-01-31",
            "filing_status": "unfiled"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-returns type="array">
        <self-assessment-return>
            <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2022-04-05</url>
            <period-ends-on type="date">2022-04-05</period-ends-on>
            <period-starts-on type="date">2021-04-06</period-starts-on>
            <payments type="array">
                <payment>
                    <label>Balancing Payment</label>
                    <amount-due type="decimal">-2250.0</amount-due>
                    <due-on type="date">2023-01-31</due-on>
                </payment>
            </payments>
            <filing-due-on type="date">2023-01-31</filing-due-on>
            <filing-status>filed</filing-status>
            <filed-at type="dateTime">2023-10-10T10:32:49+00:00</filed-at>
            <filed-reference>YFM7CAZY2AQXUCRCQJOVS45UZUS4JWQN</filed-reference>
        </self-assessment-return>
        <self-assessment-return>
            <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2023-04-05</url>
            <period-ends-on type="date">2023-04-05</period-ends-on>
            <period-starts-on type="date">2022-04-06</period-starts-on>
            <payments type="array">
                <payment>
                    <label>Balancing Payment</label>
                    <amount-due type="decimal">6181.82</amount-due>
                    <due-on type="date">2024-01-31</due-on>
                    <status>unpaid</status>
                </payment>
            </payments>
            <filing-due-on type="date">2024-01-31</filing-due-on>
            <filing-status>unfiled</filing-status>
        </self-assessment-return>
        <self-assessment-return>
            <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
            <period-ends-on type="date">2024-04-05</period-ends-on>
            <period-starts-on type="date">2023-04-06</period-starts-on>
            <payments type="array">
                <payment>
                    <label>Payment on Account</label>
                    <amount-due type="decimal">3009.01</amount-due>
                    <due-on type="date">2024-01-31</due-on>
                    <status>unpaid</status>
                </payment>
                <payment>
                    <label>Payment on Account</label>
                    <amount-due type="decimal">3009.01</amount-due>
                    <due-on type="date">2024-07-31</due-on>
                    <status>unpaid</status>
                </payment>
                <payment>
                    <label>Balancing Payment</label>
                    <amount-due type="decimal">-6018.02</amount-due>
                    <due-on type="date">2025-01-31</due-on>
                </payment>
            </payments>
            <filing-due-on type="date">2025-01-31</filing-due-on>
            <filing-status>unfiled</filing-status>
        </self-assessment-return>
    </self-assessment-returns>
</freeagent>
```
Show as JSON

## Fetch details for a Self Assessment Return

```http
GET https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_return": {
        "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
        "period_ends_on": "2024-04-05",
        "period_starts_on": "2023-04-06",
        "payments": [
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-01-31",
                "status": "unpaid"
            },
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-07-31",
                "status": "unpaid"
            },
            {
                "label": "Balancing Payment",
                "amount_due": "-6018.02",
                "due_on": "2025-01-31"
            }
        ],
        "filing_due_on": "2025-01-31",
        "filing_status": "unfiled"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-return>
        <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
        <period-ends-on type="date">2024-04-05</period-ends-on>
        <period-starts-on type="date">2023-04-06</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-01-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-07-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Balancing Payment</label>
                <amount-due type="decimal">-6018.02</amount-due>
                <due-on type="date">2025-01-31</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2025-01-31</filing-due-on>
        <filing-status>unfiled</filing-status>
    </self-assessment-return>
</freeagent>
```
Show as JSON

## Mark a Self Assessment Return as Filed

*Minimum access level*: `Full Access`

```http
PUT https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on/mark_as_filed
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_return": {
        "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
        "period_ends_on": "2024-04-05",
        "period_starts_on": "2023-04-06",
        "payments": [
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-01-31",
                "status": "unpaid"
            },
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-07-31",
                "status": "unpaid"
            },
            {
                "label": "Balancing Payment",
                "amount_due": "-6018.02",
                "due_on": "2025-01-31"
            }
        ],
        "filing_due_on": "2025-01-31",
        "filing_status": "marked_as_filed"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-return>
        <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
        <period-ends-on type="date">2024-04-05</period-ends-on>
        <period-starts-on type="date">2023-04-06</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-01-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-07-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Balancing Payment</label>
                <amount-due type="decimal">-6018.02</amount-due>
                <due-on type="date">2025-01-31</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2025-01-31</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </self-assessment-return>
</freeagent>
```
Show as JSON

## Mark a Self Assessment Return as Unfiled

*Minimum access level*: `Full Access`

```http
PUT https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on/mark_as_unfiled
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_return": {
        "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
        "period_ends_on": "2024-04-05",
        "period_starts_on": "2023-04-06",
        "payments": [
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-01-31",
                "status": "unpaid"
            },
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-07-31",
                "status": "unpaid"
            },
            {
                "label": "Balancing Payment",
                "amount_due": "-6018.02",
                "due_on": "2025-01-31"
            }
        ],
        "filing_due_on": "2025-01-31",
        "filing_status": "unfiled"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-return>
        <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
        <period-ends-on type="date">2024-04-05</period-ends-on>
        <period-starts-on type="date">2023-04-06</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-01-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-07-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Balancing Payment</label>
                <amount-due type="decimal">-6018.02</amount-due>
                <due-on type="date">2025-01-31</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2025-01-31</filing-due-on>
        <filing-status>unfiled</filing-status>
    </self-assessment-return>
</freeagent>
```
Show as JSON

## Mark a Self Assessment Return Payment as Paid

```http
PUT https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on/payments/:payment_date/mark_as_paid
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_return": {
        "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
        "period_ends_on": "2024-04-05",
        "period_starts_on": "2023-04-06",
        "payments": [
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-01-31",
                "status": "marked_as_paid"
            },
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-07-31",
                "status": "unpaid"
            },
            {
                "label": "Balancing Payment",
                "amount_due": "-6018.02",
                "due_on": "2025-01-31"
            }
        ],
        "filing_due_on": "2025-01-31",
        "filing_status": "marked_as_filed"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-return>
        <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
        <period-ends-on type="date">2024-04-05</period-ends-on>
        <period-starts-on type="date">2023-04-06</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-01-31</due-on>
                <status>marked_as_paid</status>
            </payment>
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-07-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Balancing Payment</label>
                <amount-due type="decimal">-6018.02</amount-due>
                <due-on type="date">2025-01-31</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2025-01-31</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </self-assessment-return>
</freeagent>
```
Show as JSON

## Mark a Self Assessment Return Payment as Unpaid

```http
PUT https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on/payments/:payment_date/mark_as_unpaid
```

### Response

```http
Status: 200 OK
```

```json
{
    "self_assessment_return": {
        "url": "https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05",
        "period_ends_on": "2024-04-05",
        "period_starts_on": "2023-04-06",
        "payments": [
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-01-31",
                "status": "unpaid"
            },
            {
                "label": "Payment on Account",
                "amount_due": "3009.01",
                "due_on": "2024-07-31",
                "status": "unpaid"
            },
            {
                "label": "Balancing Payment",
                "amount_due": "-6018.02",
                "due_on": "2025-01-31"
            }
        ],
        "filing_due_on": "2025-01-31",
        "filing_status": "marked_as_filed"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <self-assessment-return>
        <url>https://api.freeagent.com/v2/users/119/self_assessment_returns/2024-04-05</url>
        <period-ends-on type="date">2024-04-05</period-ends-on>
        <period-starts-on type="date">2023-04-06</period-starts-on>
        <payments type="array">
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-01-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Payment on Account</label>
                <amount-due type="decimal">3009.01</amount-due>
                <due-on type="date">2024-07-31</due-on>
                <status>unpaid</status>
            </payment>
            <payment>
                <label>Balancing Payment</label>
                <amount-due type="decimal">-6018.02</amount-due>
                <due-on type="date">2025-01-31</due-on>
            </payment>
        </payments>
        <filing-due-on type="date">2025-01-31</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </self-assessment-return>
</freeagent>
```
Show as JSON