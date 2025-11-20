# Final Accounts Reports

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

This endpoint allows API applications to retrieve information on a company's Final Accounts Reports.

## Attributes

| Attribute        | Description                                                                             | Kind     |
| ---------------- | --------------------------------------------------------------------------------------- | -------- |
| url              | Unique identifier for the record                                                        | URI      |
| period_ends_on   | The end date of the period                                                              | Date     |
| period_starts_on | The start date of the period                                                            | Date     |
| filing_due_on    | The date on which submission is due                                                     | Date     |
| filing_status    | One of `draft`, `unfiled`, `pending`, `rejected`, `filed` or `marked_as_filed`.         | String   |
| filed_at         | For returns which have been filed online, the date and time at which that happened      | Datetime |
| filed_reference  | For returns which have been filed online, the reference number sent with the submission | String   |

## List Final Accounts Reports for a company

```http
GET https://api.freeagent.com/v2/final_accounts_reports
```

### Response

```http
Status: 200 OK
```

```json
{
    "final_accounts_reports": [
        {
            "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
            "period_ends_on": "2022-12-31",
            "period_starts_on": "2022-01-01",
            "filing_due_on": "2023-09-30",
            "filing_status": "pending"
        },
        {
            "url": "https://api.freeagent.com/v2/final_accounts_reports/2023-12-31",
            "period_ends_on": "2023-12-31",
            "period_starts_on": "2023-01-01",
            "filing_due_on": "2024-09-30",
            "filing_status": "draft"
        }
    ]
}
```
Show as XML

&lt;?xml version="1.0" encoding="UTF-8"?&gt;

            https://api.freeagent.com/v2/final_accounts_reports/2022-12-31
            2022-12-31
            2022-01-01
            2023-09-30
            pending

            https://api.freeagent.com/v2/final_accounts_reports/2023-12-31
            2023-12-31
            2023-01-01
            2024-09-30
            draft

````

## Fetch details for a Final Accounts Report

```http
GET https://api.freeagent.com/v2/final_accounts_reports/:period_ends_on
```

### Response

```http
Status: 200 OK
```

```json
{
    "final_accounts_report": {
        "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "filing_due_on": "2023-09-30",
        "filing_status": "pending"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <final-accounts-report>
        <url>https://api.freeagent.com/v2/final_accounts_reports/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <filing-due-on type="date">2023-09-30</filing-due-on>
        <filing-status>pending</filing-status>
    </final-accounts-report>
</freeagent>
```
Show as JSON

## Mark a Final Accounts Report as Filed

*Minimum access level*: `Full Access` for non practice-managed companies, `Account Manager` for practice-managed companies.

```http
PUT https://api.freeagent.com/v2/final_accounts_reports/:period_ends_on/mark_as_filed
```

### Response

```http
Status: 200 OK
```

```json
{
    "final_accounts_report": {
        "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "filing_due_on": "2023-09-30",
        "filing_status": "marked_as_filed"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <final-accounts-report>
        <url>https://api.freeagent.com/v2/final_accounts_reports/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <filing-due-on type="date">2023-09-30</filing-due-on>
        <filing-status>marked_as_filed</filing-status>
    </final-accounts-report>
</freeagent>
```
Show as JSON

## Mark a Final Accounts Report as Unfiled

*Minimum access level*: `Full Access` for non practice-managed companies, `Account Manager` for practice-managed companies.

```http
PUT https://api.freeagent.com/v2/final_accounts_reports/:period_ends_on/mark_as_unfiled
```

### Response

```http
Status: 200 OK
```

```json
{
    "final_accounts_report": {
        "url": "https://api.freeagent.com/v2/final_accounts_reports/2022-12-31",
        "period_ends_on": "2022-12-31",
        "period_starts_on": "2022-01-01",
        "filing_due_on": "2023-09-30",
        "filing_status": "unfiled"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <final-accounts-report>
        <url>https://api.freeagent.com/v2/final_accounts_reports/2022-12-31</url>
        <period-ends-on type="date">2022-12-31</period-ends-on>
        <period-starts-on type="date">2022-01-01</period-starts-on>
        <filing-due-on type="date">2023-09-30</filing-due-on>
        <filing-status>unfiled</filing-status>
    </final-accounts-report>
</freeagent>
```
Show as JSON