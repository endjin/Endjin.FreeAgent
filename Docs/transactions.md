# Transactions

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

Displays transactions in each of the different account categories in FreeAgent
for the specified period.

## Attributes

| Attribute             | Description                                                                                                                                                                                                                                                                                           | Kind      |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| url                   | The unique identifier for the transaction                                                                                                                                                                                                                                                             | URI       |
| dated_on              | Date of transaction in `YYYY-MM-DD` format                                                                                                                                                                                                                                                            | Date      |
| created_at            | When the transaction was created                                                                                                                                                                                                                                                                      | Timestamp |
| updated_at            | When the transaction was last updated                                                                                                                                                                                                                                                                 | Timestamp |
| description           | The description of the source of the transaction, for example the reference of a bill                                                                                                                                                                                                                 | String    |
| category              | The unique identifier for the [category](categories.md#get-a-single-category) associated with the transaction                                                                                                                                                                                         | URI       |
| category_name         | The name of the category associated with the transaction                                                                                                                                                                                                                                              | String    |
| nominal_code          | The nominal code of the category                                                                                                                                                                                                                                                                      | String    |
| debit_value           | The debit value for this transaction. Credit values will be negative                                                                                                                                                                                                                                  | Decimal   |
| source_item_url       | The unique identifier for the source of the accounting transaction. Depending on the type of the source item, this may or may not be present                                                                                                                                                          | String    |
| foreign_currency_data | Empty unless the transaction is in a currency other than the native currency. If the transaction is in a foreign currency this field will contain the following: `currency_code` - the ISO code for the currency, `debit_value` - the debit value in the foreign currency. Credit values are negative | Object    |

## List all transactions

```http
GET https://api.freeagent.com/v2/accounting/transactions
```

Specifying no time period will return transactions dated between the start of the current
accounting period and the current date.

#### Date Filters

*Note:* Requested date periods must be equal or less than 12 months or be contained within a single accounting year.

Date params should be in the format `YYYY-MM-DD`.

- `from_date`
- `to_date`

#### View Filters

- `nominal_code`

### Response

```http
Status: 200 OK
```

```json
{
    "transactions": [
        {
            "url": "https://api.freeagent.com/v2/accounting/transactions/1",
            "dated_on": "2023-01-02",
            "created_at": "2023-04-20T10:12:44.000Z",
            "updated_at": "2023-04-20T10:12:44.000Z",
            "description": "Sale",
            "category": "https://api.freeagent.com/v2/categories/750",
            "category_name": "Bank Account",
            "nominal_code": "750-1",
            "debit_value": "30.0",
            "source_item_url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
            "foreign_currency_data": {}
        },
        {
            "url": "https://api.freeagent.com/v2/accounting/transactions/2",
            "dated_on": "2023-06-22",
            "created_at": "2023-04-20T10:12:44.000Z",
            "updated_at": "2023-07-23T15:13:24.000Z",
            "description": "Jane Doe - Bill TEST001",
            "category": "https://api.freeagent.com/v2/categories/359",
            "category_name": "Books and Journals",
            "nominal_code": "359",
            "debit_value": "-30.0",
            "source_item_url": "https://api.freeagent.com/v2/bills/2",
            "foreign_currency_data": {
                "currency_code": "USD",
                "debit_value": "-37.61"
            }
        },
        {
            "url": "https://api.freeagent.com/v2/accounting/transactions/3",
            "dated_on": "2023-04-30",
            "created_at": "2023-04-20T13:04:22.000Z",
            "updated_at": "2023-04-20T13:04:22.000Z",
            "description": "VAT Return 04 23*",
            "category": "https://api.freeagent.com/v2/categories/819",
            "category_name": "VAT Charged",
            "nominal_code": "819",
            "debit_value": "-120.0",
            "foreign_currency_data": {}
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <transactions type="array">
        <transaction>
            <url>https://api.freeagent.com/v2/accounting/transactions/1</url>
            <dated-on type="date">2023-01-02</dated-on>
            <created-at type="dateTime">2023-04-20T10:12:44Z</created-at>
            <updated-at type="dateTime">2023-04-20T10:12:44Z</updated-at>
            <description>Sale</description>
            <category>https://api.freeagent.com/v2/categories/750</category>
            <category-name>Bank Account</category-name>
            <nominal-code>750-1</nominal-code>
            <debit-value type="decimal">30.0</debit-value>
            <source-item-url>https://api.freeagent.com/v2/bank_transaction_explanations/1</source-item-url>
            <foreign-currency-data>
            </foreign-currency-data>
        </transaction>
        <transaction>
            <url>https://api.freeagent.com/v2/accounting/transactions/2</url>
            <dated-on type="date">2023-06-22</dated-on>
            <created-at type="dateTime">2023-04-20T10:12:44Z</created-at>
            <updated-at type="dateTime">2023-07-23T15:13:24Z</updated-at>
            <description>Jane Doe - Bill TEST001</description>
            <category>https://api.freeagent.com/v2/categories/359</category>
            <category-name>Books and Journals</category-name>
            <nominal-code>359</nominal-code>
            <debit-value type="decimal">-30.0</debit-value>
            <source-item-url>https://api.freeagent.com/v2/bills/2</source-item-url>
            <foreign-currency-data>
              <currency-code>USD</currency-code>
              <debit-value>-37.61</debit-value>
            </foreign-currency-data>
        </transaction>
        <transaction>
            <url>https://api.freeagent.com/v2/accounting/transactions/3</url>
            <dated-on type="date">2023-04-30</dated-on>
            <created-at type="dateTime">2023-04-20T13:04:22Z</created-at>
            <updated-at type="dateTime">2023-04-20T13:04:22Z</updated-at>
            <description>VAT Return 04 23*</description>
            <category>https://api.freeagent.com/v2/categories/819</category>
            <category-name>VAT Charged</category-name>
            <nominal-code>819</nominal-code>
            <debit-value type="decimal">-120.0</debit-value>
            <foreign-currency-data>
      </foreign-currency-data>
        </transaction>
    </transactions>
</freeagent>
```
Show as JSON

## Get a single transaction

```http
GET https://api.freeagent.com/v2/accounting/transactions/:id
```

### Response

```http
Status: 200 OK
```

```json
{
    "transaction": {
        "url": "https://api.freeagent.com/v2/accounting/transactions/1",
        "dated_on": "2023-01-02",
        "created_at": "2023-04-20T10:12:44.000Z",
        "updated_at": "2023-04-20T10:12:44.000Z",
        "description": "Sale",
        "category": "https://api.freeagent.com/v2/categories/750",
        "category_name": "Bank Account",
        "nominal_code": "750-1",
        "debit_value": "30.0",
        "source_item_url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
        "foreign_currency_data": {}
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <transaction>
        <url>https://api.freeagent.com/v2/accounting/transactions/1</url>
        <dated-on type="date">2023-01-02</dated-on>
        <created-at type="dateTime">2023-04-20T10:12:44Z</created-at>
        <updated-at type="dateTime">2023-04-20T10:12:44Z</updated-at>
        <description>Sale</description>
        <category>https://api.freeagent.com/v2/categories/750</category>
        <category-name>Bank Account</category-name>
        <nominal-code>750-1</nominal-code>
        <debit-value type="decimal">30.0</debit-value>
        <source-item-url>https://api.freeagent.com/v2/bank_transaction_explanations/1</source-item-url>
        <foreign-currency-data>
        </foreign-currency-data>
    </transaction>
</freeagent>
```
Show as JSON