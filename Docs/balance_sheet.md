# Balance Sheet

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

## Attributes

| Attribute                    | Description                                                                                                                                                                                                                                                                                                                      | Kind    |
| ---------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| accounting_period_start_date | Start date of the accounting period for which the values are shown Not applicable on opening balances                                                                                                                                                                                                                            | Date    |
| as_at_date                   | Values shown are as at this date, starting from `accounting_period_start_date` Not applicable on opening balances                                                                                                                                                                                                                | Date    |
| currency                     | ISO code representing the company's native [currency](currencies.md)                                                                                                                                                                                                                                                             | String  |
| capital_assets               | Capital assets of the business, containing: `accounts`: Array of capital asset accounts, each containing the `name`, `nominal_code` and `total_debit_value` of the account, rounded to the nearest integer, `net_book_value`: Net book value of all capital asset accounts, rounded to the nearest integer                       | Object  |
| current_assets               | Current assets of the business, containing: `accounts`: Array of current asset accounts, each containing the `name`, `nominal_code` and `total_debit_value` of the account, rounded to the nearest integer                                                                                                                       | Object  |
| current_liabilities          | Current liabilities of the business, containing: `accounts`: Array of current liability accounts, each containing the `name`, `nominal_code` and `total_debit_value` of the account, rounded to the nearest integer A negative total debit value indicates money owed by the business                                            | Object  |
| net_current_assets           | Total value of all current asset and current liability account debit values added together, rounded to the nearest integer                                                                                                                                                                                                       | Integer |
| total_assets                 | Total value of all capital asset, current asset and current liability account debit values added together, rounded to the nearest integer                                                                                                                                                                                        | Integer |
| owners_equity                | Owner's equity, containing: `accounts`: Array of equity accounts, each containing the `name`, `nominal_code` and `total_debit_value` of the account, rounded to the nearest integer, `retained_profit`: Total retained profit, rounded to the nearest integer A negative value indicates money owed by the business to its owner | Object  |
| total_owners_equity          | Total owner's equity, rounded to the nearest integer. This number should always be the inverse of the business's total assets                                                                                                                                                                                                    | Integer |

## Get the balance sheet

```http
GET https://api.freeagent.com/v2/accounting/balance_sheet
```

### Date Filters

```http
GET https://api.freeagent.com/v2/accounting/balance_sheet?as_at_date=2023-09-30
```

- `as_at_date` - Date in YYYY-MM-DD format (optional)

### Response

If no `as_at_date` param is specified, the values returned are as at the current date, from the beginning
of the ongoing annual accounting period.

If `as_at_date` is specified, the values returned are for the annual accounting period containing `as_at_date`,
up to the specified date.

```http
Status: 200 OK
```

```json
{
    "balance_sheet": {
        "accounting_period_start_date": "2023-03-01",
        "as_at_date": "2023-09-30",
        "currency": "GBP",
        "capital_assets": {
            "accounts": [
                {
                    "name": "Other Capital Asset Brought Forward",
                    "nominal_code": "601-4",
                    "total_debit_value": 800
                },
                {
                    "name": "Other Capital Asset Purchase",
                    "nominal_code": "602-4",
                    "total_debit_value": 1649
                },
                {
                    "name": "Other Capital Asset Depreciation Brought Forward",
                    "nominal_code": "605-4",
                    "total_debit_value": -533
                },
                {
                    "name": "Other Capital Asset Depreciation",
                    "nominal_code": "606-4",
                    "total_debit_value": -816
                }
            ],
            "net_book_value": 1099
        },
        "current_assets": {
            "accounts": [
                {
                    "name": "Trade Debtors",
                    "nominal_code": "681",
                    "total_debit_value": 2040
                },
                {
                    "name": "Bank Account: Default bank account",
                    "nominal_code": "750-1",
                    "total_debit_value": 21722
                },
                {
                    "name": "VAT Reclaimed",
                    "nominal_code": "818",
                    "total_debit_value": 78
                }
            ]
        },
        "current_liabilities": {
            "accounts": [
                {
                    "name": "Trade Creditors",
                    "nominal_code": "796",
                    "total_debit_value": -865
                },
                {
                    "name": "VAT",
                    "nominal_code": "817",
                    "total_debit_value": -5083
                },
                {
                    "name": "Corporation Tax",
                    "nominal_code": "820",
                    "total_debit_value": -4932
                },
                {
                    "name": "Expense Account: Development Team",
                    "nominal_code": "905-1",
                    "total_debit_value": -2055
                },
                {
                    "name": "Suspense Account",
                    "nominal_code": "999",
                    "total_debit_value": -100
                }
            ]
        },
        "net_current_assets": 10806,
        "total_assets": 11905,
        "owners_equity": {
            "accounts": [
                {
                    "name": "Share Premium",
                    "nominal_code": "670",
                    "total_debit_value": -30
                }
            ],
            "retained_profit": -11875
        },
        "total_owners_equity": -11905
    }
}

```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <balance-sheet>
        <accounting-period-start-date type="date">2023-03-01</accounting-period-start-date>
        <as-at-date type="date">2023-09-30</as-at-date>
        <currency>GBP</currency>
        <capital-assets>
            <accounts type="array">
                <account>
                    <name>Other Capital Asset Brought Forward</name>
                    <nominal-code>601-4</nominal-code>
                    <total-debit-value type="integer">800</total-debit-value>
                </account>
                <account>
                    <name>Other Capital Asset Purchase</name>
                    <nominal-code>602-4</nominal-code>
                    <total-debit-value type="integer">1649</total-debit-value>
                </account>
                <account>
                    <name>Other Capital Asset Depreciation Brought Forward</name>
                    <nominal-code>605-4</nominal-code>
                    <total-debit-value type="integer">-533</total-debit-value>
                </account>
                <account>
                    <name>Other Capital Asset Depreciation</name>
                    <nominal-code>606-4</nominal-code>
                    <total-debit-value type="integer">-816</total-debit-value>
                </account>
            </accounts>
            <net-book-value type="integer">1099</net-book-value>
        </capital-assets>
        <current-assets>
            <accounts type="array">
                <account>
                    <name>Trade Debtors</name>
                    <nominal-code>681</nominal-code>
                    <total-debit-value type="integer">2040</total-debit-value>
                </account>
                <account>
                    <name>Bank Account: Default bank account</name>
                    <nominal-code>750-1</nominal-code>
                    <total-debit-value type="integer">21722</total-debit-value>
                </account>
                <account>
                    <name>VAT Reclaimed</name>
                    <nominal-code>818</nominal-code>
                    <total-debit-value type="integer">78</total-debit-value>
                </account>
            </accounts>
        </current-assets>
        <current-liabilities>
            <accounts type="array">
                <account>
                    <name>Trade Creditors</name>
                    <nominal-code>796</nominal-code>
                    <total-debit-value type="integer">-865</total-debit-value>
                </account>
                <account>
                    <name>VAT</name>
                    <nominal-code>817</nominal-code>
                    <total-debit-value type="integer">-5083</total-debit-value>
                </account>
                <account>
                    <name>Corporation Tax</name>
                    <nominal-code>820</nominal-code>
                    <total-debit-value type="integer">-4932</total-debit-value>
                </account>
                <account>
                    <name>Expense Account: Development Team</name>
                    <nominal-code>905-1</nominal-code>
                    <total-debit-value type="integer">-2055</total-debit-value>
                </account>
                <account>
                    <name>Suspense Account</name>
                    <nominal-code>999</nominal-code>
                    <total-debit-value type="integer">-100</total-debit-value>
                </account>
            </accounts>
        </current-liabilities>
        <net-current-assets type="integer">10806</net-current-assets>
        <total-assets type="integer">11905</total-assets>
        <owners-equity>
            <accounts type="array">
                <account>
                    <name>Share Premium</name>
                    <nominal-code>670</nominal-code>
                    <total-debit-value type="integer">-30</total-debit-value>
                </account>
            </accounts>
            <retained-profit type="integer">-11875</retained-profit>
        </owners-equity>
        <total-owners-equity type="integer">-11905</total-owners-equity>
    </balance-sheet>
</freeagent>
```
Show as JSON

## Get the opening balances

Note that the opening balances response does not include `accounting_period_start_date` or `as_at_date`, as they are not applicable. No date param is needed, as the values will always only relate to the opening balances period.

```http
GET https://api.freeagent.com/v2/accounting/balance_sheet/opening_balances
```

Response

```http
Status: 200 OK
```

```json
{
    "balance_sheet": {
        "currency": "GBP",
        "capital_assets": {
            "net_book_value": 0
        },
        "current_assets": {
            "accounts": [
                {
                    "name": "Bank Account: Default bank account",
                    "nominal_code": "750-1",
                    "total_debit_value": 100
                }
            ]
        },
        "current_liabilities": {
            "accounts": [
                {
                    "name": "Suspense Account",
                    "nominal_code": "999",
                    "total_debit_value": -100
                }
            ]
        },
        "net_current_assets": 0,
        "total_assets": 0,
        "owners_equity": {
            "retained_profit": 0
        },
        "total_owners_equity": 0
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <balance-sheet>
        <currency>GBP</currency>
        <capital-assets>
            <net-book-value type="integer">0</net-book-value>
        </capital-assets>
        <current-assets>
            <accounts type="array">
                <account>
                    <name>Bank Account: Default bank account</name>
                    <nominal-code>750-1</nominal-code>
                    <total-debit-value type="integer">100</total-debit-value>
                </account>
            </accounts>
        </current-assets>
        <current-liabilities>
            <accounts type="array">
                <account>
                    <name>Suspense Account</name>
                    <nominal-code>999</nominal-code>
                    <total-debit-value type="integer">-100</total-debit-value>
                </account>
            </accounts>
        </current-liabilities>
        <net-current-assets type="integer">0</net-current-assets>
        <total-assets type="integer">0</total-assets>
        <owners-equity>
            <retained-profit type="integer">0</retained-profit>
        </owners-equity>
        <total-owners-equity type="integer">0</total-owners-equity>
    </balance-sheet>
</freeagent>
```
Show as JSON