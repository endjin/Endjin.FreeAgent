# Bank Feeds

*Minimum access level*: `Banking`, unless stated otherwise.

## Attributes

| Required | Attribute         | Description                                                                                                                                                                                                                | Kind      |
| -------- | ----------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url               | The unique identifier for the Bank Feed                                                                                                                                                                                    | URI       |
| ✔        | bank_account      | The URL of the bank account which the feed will import transactions into                                                                                                                                                   | URI       |
|          | state             | The current status of the feed                                                                                                                                                                                             | String    |
|          | created_at        | Creation of the feed resource (UTC)                                                                                                                                                                                        | Timestamp |
|          | updated_at        | When the feed resource was last updated (UTC)                                                                                                                                                                              | Timestamp |
|          | feed_type         | The type of the feed. Can be "api" or "open_banking".                                                                                                                                                                      | String    |
|          | bank_service_name | The display string for the name of the bank service this feed is connected to.                                                                                                                                             | String    |
|          | sca_expires_at    | The Date and Time at which the SCA (Strong Customer Authentication) will expire on a API Bank Feed, formated in ISO 8601. Only provided for `api` feed types. This time may be in the past if the SCA has already expired. | DateTime  |

## Reading an API Bank Feed

```http
GET https://api.freeagent.com/v2/bank_feeds/:id
```

### Response

```http
Status: 200 OK
```

```json
{
    "bank_feed": {
        "url": "http://api.freeagent.com/v2/bank_feeds/7",
        "bank_account": "https://api.freeagent.com/v2/bank_accounts/22",
        "state": "enabled",
        "created_at": "2020-11-17T09:50:02.000Z",
        "updated_at": "2020-11-17T09:50:02.000Z",
        "feed_type": "api",
        "bank_service_name": "Mettle",
        "sca_expires_at": "2021-01-01T01:01:01.000Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <bank-feed>
        <url>http://api.freeagent.com/v2/bank_feeds/7</url>
        <bank-account>https://api.freeagent.com/v2/bank_accounts/22</bank-account>
        <state>enabled</state>
        <created-at type="dateTime">2020-11-17T09:51:03Z</created-at>
        <updated-at type="dateTime">2020-11-17T09:51:03Z</updated-at>
        <feed-type>api</feed-type>
        <bank-service-name>Mettle</bank-service-name>
        <sca-expires-at type="dateTime">2021-01-01T01:01:000Z</sca-expires-at>
    </bank-feed>
</freeagent>
```
Show as JSON

```http
Status: 404 Not Found
```

The requested bank feed cannot be found in this FreeAgent account.

## Reading the list of bank feeds for a company

```http
GET https://api.freeagent.com/v2/bank_feeds
```

### Response

```http
Status: 200 OK
```

```json
{
    "bank_feeds": [
      {
        "url": "http://api.freeagent.com/v2/bank_feeds/7",
        "bank_account": "https://api.freeagent.com/v2/bank_accounts/22",
        "state": "enabled",
        "created_at": "2020-11-17T09:50:02.000Z",
        "updated_at": "2020-11-17T09:50:02.000Z",
        "feed_type": "api",
        "bank_service_name": "Mettle",
        "sca_expires_at": "2021-01-01T01:01:01.000Z"
      },
      {
        "url": "http://api.freeagent.com/v2/bank_feeds/8",
        "bank_account": "https://api.freeagent.com/v2/bank_accounts/23",
        "state": "enabled",
        "created_at": "2020-11-17T09:50:02.000Z",
        "updated_at": "2020-11-17T09:50:02.000Z",
        "feed_type": "open_banking",
        "bank_service_name": "HSBC"
      }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <bank-feeds type="array">
      <bank-feed>
        <url>http://api.freeagent.com/v2/bank_feeds/7</url>
        <bank-account>https://api.freeagent.com/v2/bank_accounts/22</bank-account>
        <state>enabled</state>
        <created-at type="dateTime">2020-11-17T09:51:03Z</created-at>
        <updated-at type="dateTime">2020-11-17T09:51:03Z</updated-at>
        <feed-type>api</feed-type>
        <bank-service-name>Mettle</bank-service-name>
        <sca-expires-at type="dateTime">2021-01-01T01:01:000Z</sca-expires-at>
      </bank-feed>
      <bank-feed>
        <url>http://api.freeagent.com/v2/bank_feeds/8</url>
        <bank-account>https://api.freeagent.com/v2/bank_accounts/23</bank-account>
        <state>enabled</state>
        <created-at type="dateTime">2020-11-17T09:51:03Z</created-at>
        <updated-at type="dateTime">2020-11-17T09:51:03Z</updated-at>
        <feed-type>open_banking</feed-type>
        <bank-service-name>HSBC</bank-service-name>
      </bank-feed>
    </bank-feeds>
</freeagent>
```
Show as JSON