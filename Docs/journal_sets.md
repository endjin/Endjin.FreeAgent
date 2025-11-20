# Journal Sets

*Minimum access level*: `Tax, Accounting and Users`, unless stated otherwise.

FreeAgent models journalled corrections to accounts in 'sets' - collections of journal entries, all on the same day, which must balance to be valid.

## Journal Set Attributes

| Required | Attribute       | Description                                                                                                                                                                                                                       | Kind   |
| -------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------ |
|          | url             | The unique identifier for the journal set                                                                                                                                                                                         | URI    |
| ?        | dated_on        | The date on which the journal entries are entered, in `YYYY-MM-DD` format Not applicable on opening balances                                                                                                                      | Date   |
| ✔        | description     | Free-text description                                                                                                                                                                                                             | String |
| ✔        | journal_entries | Array of journal entry data structures. See [Journal Entry Attributes](#journal-entry-attributes).                                                                                                                                | Array  |
|          | tag             | Free-text tag that can be used to identify journal sets created by your application, or to filter journal sets when searching Tagged journal sets will not be editable by users in the app - updates can only be made via the API | String |
|          | bank_accounts   | Read-only array of [bank account](bank_accounts.md) opening balances                                                                                                                                                              | Array  |
|          | stock_items     | Read-only array of [stock item](stock_items.md) opening balances                                                                                                                                                                  | Array  |

## Journal Entry Attributes

| Required | Attribute               | Description                                                                                                                                                  | Kind    |
| -------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ | ------- |
|          | url                     | The unique identifier for the journal entry                                                                                                                  | URI     |
| ✔        | category                | The [accounting category](categories.md) of the journal entry                                                                                                | URI     |
| ✔        | debit_value             | The debit value of the journal entry                                                                                                                         | Decimal |
| ✔        | capital_asset_type      | The [capital asset type](capital_asset_types.md) for the journal                                                                                             | URI     |
| ✔        | user                    | The [user](users.md) for the journal                                                                                                                         | URI     |
| ✔        | stock_item              | The [stock item](stock_items.md) for the journal                                                                                                             | URI     |
| ✔        | stock_altering_quantity | The quantity change for the specified stock item                                                                                                             | Integer |
|          | bank_account            | The [bank account](bank_accounts.md) for the journal It is no longer possible to journal to/from bank accounts, so this is provided for historical data only | URI     |
| ?        | property                | The [property](properties.md) linked to the journal entry                                                                                                    | URI     |

## List all journal sets

```http
GET https://api.freeagent.com/v2/journal_sets
```

### Filters

```http
GET https://api.freeagent.com/v2/journal_sets?from_date=2012-01-01&to_date=2012-03-31&tag=MYAPPTAG
```

```http
GET https://api.freeagent.com/v2/journal_sets?updated_since=2018-05-24T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`
- `tag`

### Response

```http
Status: 200 OK
```

```json
{
  "journal_sets":[
    {
      "url":"https://api.freeagent.com/v2/journal_sets/1",
      "dated_on":"2011-07-28",
      "description":"An example journal set",
      "updated_at": "2020-02-06T11:08:28.000Z",
      "tag":"MYAPPTAG",
      "journal_entries":[
        {
          "url":"https://api.freeagent.com/v2/journal_entries/1",
          "category":"https://api.freeagent.com/v2/categories/001",
          "description":"A Sales Correction",
          "debit_value":"-123.45"
        },
        {
          "url":"https://api.freeagent.com/v2/journal_entries/2",
          "category":"https://api.freeagent.com/v2/categories/901",
          "user":"https://api.freeagent.com/v2/users/1",
          "description":"Director's Capital Introduced",
          "debit_value":"123.45"
        }
      ]
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-sets type="array">
    <journal-set>
      <url>https://api.freeagent.com/v2/journal_sets/1</url>
      <dated-on type="date">2011-07-28</dated-on>
      <description>An example journal set</description>
      <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
      <tag>MYAPPTAG</tag>
      <journal-entries type="array">
        <journal-entry>
          <url>https://api.freeagent.com/v2/journal_entries/1</url>
          <category>https://api.freeagent.com/v2/categories/001</category>
          <description>A Sales Correction</description>
          <debit-value type="decimal">-123.45</debit-value>
        </journal-entry>
        <journal-entry>
          <url>https://api.freeagent.com/v2/journal_entries/2</url>
          <category>https://api.freeagent.com/v2/categories/901</category>
          <user>https://api.freeagent.com/v2/users/1</user>
          <description>Director's Capital Introduced</description>
          <debit-value type="decimal">123.45</debit-value>
        </journa-entry>
      </journal-entries>
    </journal-set>
  </journal-sets>
</freeagent>
```
Show as JSON

## Get a single journal set

```http
GET https://api.freeagent.com/v2/journal_sets/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "journal_set":{
    "url":"https://api.freeagent.com/v2/journal_sets/1",
    "dated_on":"2011-07-28",
    "description":"An example journal set",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "tag":"MYAPPTAG",
    "journal_entries":[
      {
        "url":"https://api.freeagent.com/v2/journal_entries/1",
        "category":"https://api.freeagent.com/v2/categories/001",
        "description":"A Sales Correction",
        "debit_value":"-123.45"
      },
      {
        "url":"https://api.freeagent.com/v2/journal_entries/2",
        "category":"https://api.freeagent.com/v2/categories/901",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Director's Capital Introduced",
        "debit_value":"123.45"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-set>
    <url>https://api.freeagent.com/v2/journal_sets/1</url>
    <dated-on type="date">2011-07-28</dated-on>
    <description>An example journal set</description>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <tag>MYAPPTAG</tag>
    <journal-entries type="array">
      <journal-entry>
        <url>https://api.freeagent.com/v2/journal_entries/1</url>
        <category>https://api.freeagent.com/v2/categories/001</category>
        <description>A Sales Correction</description>
        <debit-value type="decimal">-123.45</debit-value>
      </journal-entry>
      <journal-entry>
        <url>https://api.freeagent.com/v2/journal_entries/2</url>
        <category>https://api.freeagent.com/v2/categories/901</category>
        <user>https://api.freeagent.com/v2/users/1</user>
        <description>Director's Capital Introduced</description>
        <debit-value type="decimal">123.45</debit-value>
      </journal-entry>
    </journal-entries>
  </journal-set>
</freeagent>
```
Show as JSON

## Get the Opening Balances

```http
GET https://api.freeagent.com/v2/journal_sets/opening_balances
```

### Response

```http
Status: 200 OK
```

```json
{
  "journal_set": {
    "url": "https://api.freeagent.com/v2/journal_sets/1",
    "description": "Opening Balances Journal Set",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "journal_entries": [
      {
          "url": "/v2/journal_sets/1/journal_entries/3",
          "category": "https://api.freeagent.com/v2/categories/001",
          "debit_value": "10.0"
      }
    ],
    "bank_accounts": [
      {
          "url": "https://api.freeagent.com/v2/bank_accounts/1",
          "description": "Default bank account",
          "debit_value": "-1000.0"
      }
    ],
    "stock_items": [
      {
          "url": "https://api.freeagent.com/v2/stock_items/1",
          "description": "Opening Balance for Stock Item: Banana",
          "debit_value": "-1.5"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-set>
    <url>https://api.freeagent.com/v2/journal_sets/1</url>
    <description>Opening Balances Journal Set</description>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <journal-entries type="array">
      <journal-entry>
        <url>/v2/journal_sets/1/journal_entries/3</url>
        <category>https://api.freeagent.com/v2/categories/001</category>
        <debit-value type="decimal">10.0</debit-value>
      </journal-entry>
    </journal-entries>
    <bank-accounts type="array">
      <bank-account>
        <url>https://api.freeagent.com/v2/bank_accounts/1</url>
        <description>Default bank account</description>
        <debit-value type="decimal">-1000.0</debit-value>
      </bank-account>
    </bank-accounts>
    <stock-items type="array">
      <stock-item>
        <url>https://api.freeagent.com/v2/stock_items/1</url>
        <description>Opening Balance for Stock Item: Banana</description>
        <debit-value type="decimal">-1.5</debit-value>
      </stock-item>
    </stock-items>
  </journal-set>
</freeagent>
```
Show as JSON

## Create a journal set

```http
POST https://api.freeagent.com/v2/journal_sets
```

Payload should have a root `journal_set` element, containing elements listed
under Journal Set Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/journal_sets/12
```

```json
{
  "journal_set":{
    "url":"https://api.freeagent.com/v2/journal_sets/1",
    "dated_on":"2011-07-28",
    "description":"An example journal set",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "tag":"MYAPPTAG",
    "journal_entries":[
      {
        "url":"https://api.freeagent.com/v2/journal_entries/1",
        "category":"https://api.freeagent.com/v2/categories/001",
        "description":"A Sales Correction",
        "debit_value":"-123.45"
      },
      {
        "url":"https://api.freeagent.com/v2/journal_entries/2",
        "category":"https://api.freeagent.com/v2/categories/901",
        "user":"https://api.freeagent.com/v2/users/1",
        "description":"Director's Capital Introduced",
        "debit_value":"123.45"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <journal-set>
    <url>https://api.freeagent.com/v2/journal_sets/1</url>
    <dated-on type="date">2011-07-28</dated-on>
    <description>An example journal set</description>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <tag>MYAPPTAG</tag>
    <journal-entries type="array">
      <journal-entry>
        <url>https://api.freeagent.com/v2/journal_entries/1</url>
        <category>https://api.freeagent.com/v2/categories/001</category>
        <description>A Sales Correction</description>
        <debit-value type="decimal">-123.45</debit-value>
      </journal-entry>
      <journal-entry>
        <url>https://api.freeagent.com/v2/journal_entries/2</url>
        <category>https://api.freeagent.com/v2/categories/901</category>
        <description>Director's Capital Introduced</description>
        <debit-value type="decimal">123.45</debit-value>
      </journal-entry>
    </journal-entries>
  </journal-set>
</freeagent>
```
Show as JSON

## Update a journal set

```http
PUT https://api.freeagent.com/v2/journal_sets/:id
```

Payload should have a root `journal_set` element, containing elements listed
under Journal Set Attributes that should be updated.

### Response

```http
Status: 200 OK
```

```json
{
  "journal_set": {
    "url": "https://api.freeagent.com/v2/journal_sets/37",
    "dated_on": "2014-01-18",
    "description": "Journal Set for 2014-01-18",
    "updated_at": "2020-02-06T11:08:28.000Z",
    "journal_entries": [
      {
        // Remove this journal entry
        "url": "https://api.freeagent.com/v2/journal_sets/37/journal_entries/17",
        "_destroy": true
      },
      {
        // Change the debit value on this journal entry
        "url": "https://api.freeagent.com/v2/journal_sets/37/journal_entries/18",
        "debit_value": "-20.0"
      },
      {
        // Add this journal entry to the set
        "category": "https://api.freeagent.com/v2/categories/001",
        "debit_value": "-20.0"
      }
    ]
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
  <journal-set>
    <url>https://api.freeagent.com/v2/journal_sets/37</url>
    <dated-on type="date">2014-01-18</dated-on>
    <description>Journal Set for 2014-01-18</description>
    <updated-at type="datetime">2020-02-06T11:08:28.000Z</updated-at>
    <journal-entries type="array">
      <journal-entry>
        <!-- Remove this journal entry -->
        <url>https://api.freeagent.com/v2/journal_sets/37/journal_entries/17</url>
        <_destroy>true</_destroy>
      </journal-entry>
      <journal-entry>
        <!-- Change the debit value on this journal entry -->
        <url>https://api.freeagent.com/v2/journal_sets/37/journal_entries/18</url>
        <debit-value type="decimal">-20.0</debit-value>
      </journal-entry>
      <journal-entry>
        <!-- Add this journal entry to the set -->
        <category>https://api.freeagent.com/v2/categories/001</category>
        <debit-value type="decimal">-20.0</debit-value>
      </journal-entry>
    </journal-entries>
  </journal-set>
```
Show as JSON

### Response

```http
Status: 200 OK
```

## Delete a journal set

```http
DELETE https://api.freeagent.com/v2/journal_sets/:id
```

### Response

```http
Status: 200 OK
```