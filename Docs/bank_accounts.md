# Bank Accounts

*Minimum access level*: `Banking`, unless stated otherwise.

## Attributes

| Required | Attribute            | Description                                                                                                                                                                   | Kind      |
| -------- | -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                  | The unique identifier for the bank account                                                                                                                                    | URI       |
|          | type                 | One of the following: `StandardBankAccount` (the default if no type is given), `PaypalAccount`, `CreditCardAccount`                                                           | String    |
| ✔        | name                 | Account name                                                                                                                                                                  | String    |
|          | currency             | The [accounting currency](currencies.md) for the account, e.g. `GBP`, `USD`, `EUR` Defaults to the native currency Cannot be changed once a bank account has any transactions | String    |
|          | is_personal          | `true` for a personal account, `false` for a business account                                                                                                                 | Boolean   |
|          | is_primary           | `true` if it is the primary bank account of the [company](company.md), `false` otherwise                                                                                      | Boolean   |
|          | status               | Constant that represents if a bank account is `active` or `hidden`                                                                                                            | String    |
|          | bank_name            | Bank name                                                                                                                                                                     | String    |
| ✔        | opening_balance      | The account balance at the start of the FreeAgent Start Date For accounts opened after this date, enter zero                                                                  | Decimal   |
|          | bank_code            | Constant that represents the bank, based on the sort code. For example: `generic`, `barclays`, `natwest`, `rbs`                                                               | String    |
|          | current_balance      | Latest balance                                                                                                                                                                | Decimal   |
|          | latest_activity_date | Date of latest transaction or the latest bank account entry, in `YYYY-MM-DD` format                                                                                           | Date      |
|          | created_at           | Creation of the bank account resource (UTC)                                                                                                                                   | Timestamp |
|          | updated_at           | When the bank account resource was last updated (UTC)                                                                                                                         | Timestamp |
|          | bank_guess_enabled   | `true` if guess is enabled on the bank account, `false` if disabled                                                                                                           | Boolean   |
|          | account_number       | Bank account number                                                                                                                                                           | String    |
|          | sort_code            | Bank account sort code / routing number                                                                                                                                       | String    |
|          | secondary_sort_code  | Secondary bank account sort code / routing number Use this only if the bank account has two Bank/Sort Codes                                                                   | String    |
|          | iban                 | International Bank Account Number for the account                                                                                                                             | String    |
|          | bic                  | Bank Identifier Code for the account (also known as the Swift Code)                                                                                                           | String    |
| ✔        | account_number       | Bank account number For security, only the last four digits are required                                                                                                      | String    |
| ✔        | email                | The email address used to log into PayPal                                                                                                                                     | String    |

## List bank accounts

```http
GET https://api.freeagent.com/v2/bank_accounts
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/bank_accounts?view=standard_bank_accounts
```

- `standard_bank_accounts`: Show only standard bank accounts.
- `credit_card_accounts`: Show only credit card accounts.
- `paypal_accounts`: Show only paypal accounts.

### Response

```http
Status: 200 OK
```

```json
{ "bank_accounts":[
  {
    "url":"https://api.freeagent.com/v2/bank_accounts/1",
    "opening_balance":"0.0",
    "type":"StandardBankAccount",
    "name":"Default bank account",
    "is_personal":false,
    "status":"active",
    "currency": "GBP",
    "current_balance": "0.0",
    "updated_at":"2011-07-28T11:25:20Z",
    "created_at":"2011-07-28T11:25:11Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-accounts type="array">
    <bank-account>
      <url>https://api.freeagent.com/v2/bank_accounts/1</url>
      <opening-balance type="decimal">0.0</opening-balance>
      <type>StandardBankAccount</type>
      <name>Default bank account</name>
      <currency>GBP</currency>
      <current-balance type="decimal">0.0</current-balance>
      <is-personal type="boolean">false</is-personal>
      <status>active</status>
      <updated-at type="datetime">2011-07-28T11:25:20Z</updated-at>
      <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
    </bank-account>
  </bank-accounts>
</freeagent>
```
Show as JSON

## Get a single bank account

```http
GET https://api.freeagent.com/v2/bank_accounts/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "bank_account":
  {
    "opening_balance":"0.0",
    "type":"StandardBankAccount",
    "name":"Default bank account",
    "is_personal":false,
    "status":"active",
    "currency": "GBP",
    "current_balance": "0.0",
    "updated_at":"2011-07-28T11:25:20Z",
    "created_at":"2011-07-28T11:25:11Z",
    "bank_guess_enabled":false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-account>
    <opening-balance type="decimal">0.0</opening-balance>
    <type>StandardBankAccount</type>
    <name>Default bank account</name>
    <is-personal type="boolean">false</is-personal>
    <status>active</status>
    <currency>GBP</currency>
    <current-balance type="decimal">0.0</current-balance>
    <updated-at type="datetime">2011-07-28T11:25:20Z</updated-at>
    <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
    <bank_guess_enabled type="boolean">false</bank_guess_enabled>
  </bank-account>
</freeagent>
```
Show as JSON

## Create a bank account

```http
POST https://api.freeagent.com/v2/bank_accounts
```

Payload should have a root `bank_account` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/bank_accounts/61
```

```json
{ "bank_account":
  {
    "opening_balance":"0.0",
    "type":"StandardBankAccount",
    "name":"Default bank account",
    "is_personal":false,
    "status":"active",
    "updated_at":"2011-07-28T11:25:20Z",
    "created_at":"2011-07-28T11:25:11Z",
    "bank_guess_enabled":false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <bank-account>
    <opening-balance type="decimal">0.0</opening-balance>
    <type>StandardBankAccount</type>
    <name>Default bank account</name>
    <is-personal type="boolean">false</is-personal>
    <status>active</status>
    <updated-at type="datetime">2011-07-28T11:25:20Z</updated-at>
    <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
    <bank_guess_enabled type="boolean">false</bank_guess_enabled>
  </bank-account>
</freeagent>
```
Show as JSON

## Update a bank account

```http
PUT https://api.freeagent.com/v2/bank_accounts/:id
```

Payload should have a root `bank_account` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a bank account

```http
DELETE https://api.freeagent.com/v2/bank_accounts/:id
```

### Response

```http
Status: 200 OK
```