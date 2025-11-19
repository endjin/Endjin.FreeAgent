# Trial Balance

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

## Get the trial balance summary

```http
GET https://api.freeagent.com/v2/accounting/trial_balance/summary
```

Date Filters

```json
GET https://api.freeagent.com/v2/accounting/trial_balance/summary?to_date=2014-05-01
GET https://api.freeagent.com/v2/accounting/trial_balance/summary?from_date=2014-01-01&to_date=2014-05-01
```

- `from_date` - Date in YYYY-MM-DD format (optional)
- `to_date` - Date in YYYY-MM-DD format

### Response

If no dates are specified, the summary is for the current date.

If only `to_date` is specified, the summary is for a period containing the
specified date. The trial balance figures are from the beginning of the
accounting period up to the specified date.

If both `from_date` and `to_date` are specified, the summary is for a period
with the specified custom range.

#### Note regarding response attributes

The `display_nominal_code` attribute always returns the full category code as it's displayed on accounting reports
in the FreeAgent web UI, and should be used in most circumstances instead of `nominal_code`. The `nominal_code`
attribute maintains legacy behaviour where, for bank account and user categories (codes 750 and 900-910), the second
part of the code refers to the ID of the related resource (bank account or user) rather than the category sub-code.

```http
Status: 200 OK
```

```json
{
  "trial_balance_summary": [
    {
      "category": "https://api.freeagent.com/v2/categories/101",
      "nominal_code": "101",
      "display_nominal_code": "101",
      "name": "Cost of Sales",
      "total": "275.0"
    },
    {
      "category": "https://api.freeagent.com/v2/categories/750",
      "nominal_code": "750-65345",
      "display_nominal_code": "750-1",
      "name": "Bank Account Default bank account",
      "total": "-12860.0",
      "bank_account": "https://api.freeagent.com/v2/bank_accounts/10"
    },
    {
      "category": "https://api.freeagent.com/v2/categories/902",
      "nominal_code": "902-6465",
      "display_nominal_code:": "902-1",
      "name": "Salary and Bonuses: Bob Bobson",
      "total": "-9062.64",
      "user": "https://api.freeagent.com/v2/users/5"
    },
    {
     "category": "https://api.freeagent.com/v2/categories/606",
     "nominal_code": "606-4",
     "display_nominal_code": "606-4",
     "name": "Other Capital Asset Depreciation",
     "total": "-448.62"
    },
  ]
}

```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <trial-balance-summary type="array">
    <trial-balance-summary>
      <category>https://api.freeagent.com/v2/categories/101</category>
      <nominal_code>101</nominal_code>
      <display_nominal_code>101</nominal_code>
      <name>Cost of Sales</name>
      <total type="decimal">275.0</total>
    </trial-balance-summary>
    <trial-balance-summary>
      <category>https://api.freeagent.com/v2/categories/750</category>
      <nominal_code>750-65345</nominal_code>
      <display_nominal_code>750-1</display_nominal_code>
      <name>Bank Account Default bank account</name>
      <total type="decimal">-12860.0</total>
      <bank_account>https://api.freeagent.com/v2/bank_accounts/10</bank_account>
    </trial-balance-summary>
    <trial-balance-summary>
      <category>https://api.freeagent.com/v2/categories/902</category>
      <nominal_code>902-6465</nominal_code>
      <display_nominal_code>902-1</display_nominal_code>
      <name>Salary and Bonuses: Bob Bobson</name>
      <total type="decimal">-9062.64</total>
      <user>https://api.freeagent.com/v2/users/5</user>
    </trial-balance-summary>
    <trial-balance-summary>
     <category>https://api.freeagent.com/v2/categories/606</category>
     <nominal_code>606-4</nominal_code>
     <display_nominal_code>606-4</nominal_code>
     <name>Other Capital Asset Depreciation</name>
     <total>-448.62</total>
    </trial-balance-summary>
</freeagent>
```
Show as JSON

## Get the opening balances

```http
GET https://api.freeagent.com/v2/accounting/trial_balance/summary/opening_balances
```

Response

```http
Status: 200 OK
```

```json
{
  "trial_balance_summary": [
    {
      "category": "https://api.freeagent.com/v2/categories/001",
      "nominal_code": "001",
      "display_nominal_code": "001",
      "name": "Sales",
      "total": "250.0"
    },
    {
      "category": "https://api.freeagent.com/v2/categories/750",
      "nominal_code": "750-65345",
      "display_nominal_code": "750-1",
      "name": "Bank Account Default bank account",
      "total": "0.0",
      "bank_account": "https://api.freeagent.com/v2/bank_accounts/10"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <trial-balance-summary type="array">
    <trial-balance-summary>
      <category>https://api.freeagent.com/v2/categories/001</category>
      <nominal_code>001</nominal_code>
      <display_nominal_code>001</nominal_code>
      <name>Sales</name>
      <total type="decimal">250.0</total>
    </trial-balance-summary>
    <trial-balance-summary>
      <category>https://api.freeagent.com/v2/categories/750</category>
      <nominal_code>750-65345</nominal_code>
      <display_nominal_code>750-1</display_nominal_code>
      <name>Bank Account Default bank account</name>
      <total type="decimal">0.0</total>
      <bank_account>https://api.freeagent.com/v2/bank_accounts/10</bank_account>
    </trial-balance-summary>
  </trial-balance-summary>
</freeagent>
```
Show as JSON