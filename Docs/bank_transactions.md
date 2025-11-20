# Bank Transactions

*Minimum access level*: `Banking`, unless stated otherwise.

## Attributes

| Attribute                     | Description                                                                                                                                                                                  | Kind      |
| ----------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| url                           | The unique identifier for the bank transaction                                                                                                                                               | URI       |
| amount                        | Total amount in the company's native [currency](currencies.md)                                                                                                                               | Decimal   |
| bank_account                  | Transaction's [bank account](bank_accounts.md)                                                                                                                                               | URI       |
| dated_on                      | Date of transaction in `YYYY-MM-DD` format                                                                                                                                                   | Date      |
| description                   | Free-text description                                                                                                                                                                        | String    |
| full_description              | A complete description of the transaction, including the amount and type                                                                                                                     | String    |
| uploaded_at                   | When the transaction was uploaded to FreeAgent                                                                                                                                               | Timestamp |
| unexplained_amount            | Amount yet to be explained                                                                                                                                                                   | Decimal   |
| is_manual                     | `true` if transaction was manually added (i.e. not from a statement or bank feed), `false` otherwise                                                                                         | Boolean   |
| transaction_id                | A unique ID of the transaction. It is assumed to be unique within a banking instiution, and can be used as a shared identifier between FreeAgent and the third party. Also known as `fit_id` | String    |
| created_at                    | Creation of the bank transaction (UTC)                                                                                                                                                       | Timestamp |
| updated_at                    | When the bank transaction was last updated (UTC)                                                                                                                                             | Timestamp |
| matching_transactions_count   | The number of other transactions in the bank account that have a matching description                                                                                                        | Integer   |
| bank_transaction_explanations | Entries that fully or partially explain this transaction. See [Bank Transaction Explanations](bank_transaction_explanations.md).                                                             | Array     |

## List all bank transactions under a certain bank account

```http
GET https://api.freeagent.com/v2/bank_transactions?bank_account=:bank_account
```

#### Date Filters

```http
GET https://api.freeagent.com/v2/bank_transactions?bank_account=:bank_account&from_date=2012-01-01&to_date=2012-03-31
```

```http
GET https://api.freeagent.com/v2/bank_transactions?bank_account=:bank_account&updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

#### View Filters

```http
GET https://api.freeagent.com/v2/bank_transactions?bank_account=:bank_account&view=unexplained
```

- `all` (default)
- `unexplained`
- `explained`
- `manual`
- `imported`
- `marked_for_review`

#### Filtering by latest statement upload

Returns only transactions that are part of the last statement that has been uploaded.
Note that this may have been uploaded via the API or by a user via the web interface.

```http
GET https://api.freeagent.com/v2/bank_transactions?bank_account=:bank_account&last_uploaded=true
```

### Response

```http
Status: 200 OK
```

```json
{ "bank_transactions":[
  {
    "url":"https://api.freeagent.com/v2/bank_transactions/8",
    "amount":"-730.0",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2020-07-06",
    "description":".Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609",
    "full_description": ".Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609/OTHER/£730.0",
    "uploaded_at": "2020-07-06T15:26:21.000Z",
    "unexplained_amount":"0.0",
    "is_manual":false,
    "transaction_id":"049b807d-83ea-4d98-854c-e84b18775d31",
    "created_at": "2020-07-06T15:26:21.000Z",
    "updated_at": "2020-07-06T15:26:21.000Z",
    "matching_transactions_count": 0,
    "bank_transaction_explanations": []
  },
  {
    "url":"https://api.freeagent.com/v2/bank_transactions/15",
    "amount":"-350.0",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2020-07-06",
    "description":".Ledner Inc/.mesh enterprise platforms (246)942-9558",
    "full_description": ".Ledner Inc/.mesh enterprise platforms (246)942-9558/OTHER/£350.0",
    "uploaded_at": "2020-07-06T15:26:21.000Z",
    "unexplained_amount":"0.0",
    "is_manual":false,
    "transaction_id":"049b807d-83ea-4d98-854c-e84b18775d32",
    "created_at": "2020-07-06T15:26:21.000Z",
    "updated_at": "2020-07-06T15:26:21.000Z",
    "matching_transactions_count": 0,
    "bank_transaction_explanations": []
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-transactions type="array">
    <bank-transaction>
      <url>https://api.freeagent.com/v2/bank_transactions/8</url>
      <amount type="decimal">-730.0</amount>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <dated-on type="date">2020-07-06</dated-on>
      <description>.Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609</description>
      <full-description>.Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609/OTHER/£730.0</full-description>
      <uploaded-at type="dateTime">2020-07-06T15:26:21Z</uploaded-at>
      <unexplained-amount type="decimal">0.0</unexplained-amount>
      <is-manual type="boolean">false</is-manual>
      <transaction-id>049b807d-83ea-4d98-854c-e84b18775d31</transaction-id>
      <created-at type="dateTime">2020-07-06T15:26:21Z</created-at>
      <updated-at type="dateTime">2020-07-06T15:26:21Z</updated-at>
      <matching-transactions-count type="integer">0</matching-transactions-count>
      <bank-transaction-explanations type="array"/>
    </bank-transaction>
    <bank-transaction>
      <url>https://api.freeagent.com/v2/bank_transactions/15</url>
      <amount type="decimal">-350.0</amount>
      <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
      <dated-on type="date">2020-07-06</dated-on>
      <description>.Ledner Inc/.mesh enterprise platforms (246)942-9558</description>
      <full-description>.Ledner Inc/.mesh enterprise platforms (246)942-9558/OTHER/£730.0</full-description>
      <uploaded-at type="dateTime">2020-07-06T15:26:21Z</uploaded-at>
      <unexplained-amount type="decimal">0.0</unexplained-amount>
      <is-manual type="boolean">false</is-manual>
      <transaction-id>049b807d-83ea-4d98-854c-e84b18775d32</transaction-id>
      <created-at type="dateTime">2020-07-06T15:26:21Z</created-at>
      <updated-at type="dateTime">2020-07-06T15:26:21Z</updated-at>
      <matching-transactions-count type="integer">0</matching-transactions-count>
      <bank-transaction-explanations type="array"/>
    </bank-transaction>
  </bank-transactions>
</freeagent>
```
Show as JSON

## Get a single bank transaction

```http
GET https://api.freeagent.com/v2/bank_transactions/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "bank_transaction":
  {
    "url":"https://api.freeagent.com/v2/bank_transactions/15",
    "amount":"-730.0",
    "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
    "dated_on":"2020-07-06",
    "description":".Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609",
    "full_description": ".Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609/OTHER/£730.0",
    "uploaded_at": "2020-07-06T15:26:21.000Z",
    "unexplained_amount":"0.0",
    "is_manual":false,
    "transaction_id":"049b807d-83ea-4d98-854c-e84b18775d31",
    "created_at": "2020-07-06T15:26:21.000Z",
    "updated_at": "2020-07-06T15:26:21.000Z",
    "matching_transactions_count": 0,
    "bank_transaction_explanations":[
      {
        "url":"https://api.freeagent.com/v2/bank_transaction_explanations/8",
        "bank_transaction":"https://api.freeagent.com/v2/bank_transactions/8",
        "bank_account":"https://api.freeagent.com/v2/bank_accounts/1",
        "dated_on":"2020-07-06",
        "description":"harness end-to-end e-business",
        "entry_type":"Business Entertaining",
        "gross_value":"-730.0",
        "is_deletable":true
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-transaction>
    <url>https://api.freeagent.com/v2/bank_transactions/15</url>
    <amount type="decimal">-730.0</amount>
    <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
    <dated-on type="date">2020-07-06</dated-on>
    <description>.Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609</description>
    <full-description>.Reichert, Kautzer and Schultz/.harness end-to-end e-business 665-454-0057 x55609/OTHER/£730.0</full-description>
    <uploaded-at type="dateTime">2020-07-06T15:26:21Z</uploaded-at>
    <unexplained-amount type="decimal">0.0</unexplained-amount>
    <is-manual type="boolean">false</is-manual>
    <transaction-id>049b807d-83ea-4d98-854c-e84b18775d31</transaction-id>
    <created-at type="dateTime">2020-07-06T15:26:21Z</created-at>
    <updated-at type="dateTime">2020-07-06T15:26:21Z</updated-at>
    <matching-transactions-count type="integer">0</matching-transactions-count>
    <bank-transaction-explanations type="array">
      <bank-transaction-explanation>
        <url>https://api.freeagent.com/v2/bank_transaction_explanations/8</url>
        <bank-transaction>https://api.freeagent.com/v2/bank_transactions/8</bank-transaction>
        <bank-account>https://api.freeagent.com/v2/bank_accounts/1</bank-account>
        <dated-on type="date">2020-07-06</dated-on>
        <description>harness end-to-end e-business</description>
        <entry-type>Business Entertaining</entry-type>
        <gross-value type="decimal">-730.0</gross-value>
        <is-deletable type="boolean">true</is-deletable>
      </bank-transaction-explanation>
    </bank-transaction-explanations>
  </bank-transaction>
</freeagent>
```
Show as JSON

## Delete a bank transaction explanation

```http
DELETE https://api.freeagent.com/v2/bank_transaction/:id
```

Bank transactions can only be deleted if they are fully unexplained or if there are no explanations for the transaction.

### Response

```http
Status: 200 OK
```

## Upload a bank statement

```http
POST https://api.freeagent.com/v2/bank_transactions/statement?bank_account=:bank_account
```

Bank statements can be uploaded as either an array of transactions or as a file.

**Note:** For all statement imports, transactions are deduplicated against any existing transactions in the bank account on that date with the same amount and description. To avoid any incorrect deduplication, best practice is to include *all* of a day's transactions in a single statement upload.

### Upload a bank statement as an array of transactions

#### Attributes

- `statement`(Required) An array of bank transaction objects, each containing:
    - `dated_on` (Required) Date of transaction in `YYYY-MM-DD` format.
    - `description` Free-text description. *Defaults to an empty string*
    - `amount` Total amount in the company's native [currency](currencies.md). *Defaults to 0*
    - `fitid` Unique transaction ID. *Defaults to null.*
    - `transaction_type` Transaction type. *Defaults to OTHER*

The `transaction_type` if passed, can be any of the following values:

| Type        | Description                      | Notes                                                                 |
| ----------- | -------------------------------- | --------------------------------------------------------------------- |
| CREDIT      | Generic credit                   | Will be treated as positive value regardless of signage of the amount |
| DEBIT       | Generic debit                    | Will be treated as negative value regardless of signage of the amount |
| INT         | Interest earned or paid          | Treated as credit or debit depending on the signage of the amount     |
| DIV         | Dividend                         | Will be treated as positive value regardless of signage of the amount |
| FEE         | FI fee                           | Will be treated as negative value regardless of signage of the amount |
| SRVCHG      | Service charge                   | Will be treated as negative value regardless of signage of the amount |
| DEP         | Deposit                          | Will be treated as positive value regardless of signage of the amount |
| ATM         | ATM debit or credit              | Treated as credit or debit depending on the signage of the amount     |
| POS         | Point of sale debit or credit    | Treated as credit or debit depending on the signage of the amount     |
| XFER        | Transfer                         | Will be treated as negative value regardless of signage of the amount |
| CHECK       | Cheque (check)                   | Will be treated as negative value regardless of signage of the amount |
| PAYMENT     | Electronic payment               | Will be treated as negative value regardless of signage of the amount |
| CASH        | Cash withdrawal                  | Will be treated as negative value regardless of signage of the amount |
| DIRECTDEP   | Direct deposit                   | Will be treated as positive value regardless of signage of the amount |
| DIRECTDEBIT | Merchant initiated debit         | Will be treated as negative value regardless of signage of the amount |
| REPEATPMT   | Repeating payment/standing order | Will be treated as negative value regardless of signage of the amount |
| OTHER       | Other debit or credit            | Treated as credit or debit depending on the signage of the amount     |

#### Example cURL Request

```json
curl https://api.freeagent.com/v2/bank_transactions/statement?bank_account=:bank_account \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -H "Accept: */*" \
  -X POST \
  --data
  '{ "statement" :
    [
      {
        "dated_on" : "2019-07-01",
        "amount" : -100,
        "description" : "Local Council",
        "fitid" : "049b807d-83ea-4d98-854c-e84b18775d31"
      },
      {
        "dated_on" : "2019-07-05",
        "amount" : 3560,
        "description" : "Sales",
        "fitid" : "8956efc9-549a-45e4-b3e9-fadb8f070ec6"
      }
    ]
  }'
```
Show as XML

```xml
curl https://api.freeagent.com/v2/bank_transactions/statement?bank_account=:bank_account \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/xml" \
  -H "Accept: */*" \
  -X POST \
  --data '<?xml version="1.0" encoding="UTF-8"?>
  <statement type="array">
    <transaction>
      <dated_on>2017-10-01</dated_on>
      <amount>-100</amount>
      <description>Local Council</description>
      <fitid>049b807d-83ea-4d98-854c-e84b18775d31</fitid>
    </transaction>
    <transaction>
      <dated_on>2017-10-05</dated_on>
      <amount>3560</amount>
      <description>Sales</description>
      <fitid>8956efc9-549a-45e4-b3e9-fadb8f070ec6</fitid>
    </transaction>
  </statement>'
```
Show as JSON

### Upload a bank statement as a file

#### Attributes

- `statement` (Required) A file containing the statement. We strongly recommend using OFX, sometimes referred to as QBO or Quickbooks or MS Money 2005. We also support QIF and some CSV formats. [Learn more about which bank statements FreeAgent supports](https://support.freeagent.com/hc/en-gb/articles/115001217690-Which-bank-statement-formats-does-FreeAgent-support-).

#### Example cURL Request

```json
curl https://api.freeagent.com/v2/bank_transactions/statement?bank_account=:bank_account \
  -H "Authorization: Bearer TOKEN" \
  -H "Accept: */*" \
  -X POST \
  --form "statement=@your_file.ofx"
```

### Responses

```http
Status: 200 OK
```

Your data has been uploaded successfully. However, this does not indicate whether or not your statement has been *imported* correctly - please make another request to list your currently imported transactions, or check in the FreeAgent application itself to see if your import has succeeded.

```http
Status: 400 Bad Request
```

Your request was poorly formatted and could not be parsed successfully. Please check you have not omitted a required attribute and have formatted your request correctly.

```http
Status: 404 Not Found
```

You have either made a request to the wrong URL or you have failed to provide a valid bank account to import to.

```http
Status: 406 Not Acceptable
```

You have omitted the statement value or the statement you have submitted does not contain any data.