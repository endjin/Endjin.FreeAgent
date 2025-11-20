# Capital Assets

*Minimum access level*: `Banking`, unless stated otherwise.

## Attributes

| Attribute             | Description                                                                                                                                                                         | Kind      |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
| url                   | The unique identifier for the capital asset                                                                                                                                         | URI       |
| description           | Free-text description                                                                                                                                                               | String    |
| asset_life_years      | Note! This field is deprecated. See the depreciation_profile field instead. This field will contain the asset life in years for straight line depreciation, otherwise it will be 0. | Integer   |
| depreciation_profile  | Details of the method used for calculating depreciation, along with its frequency. See [depreciation profiles](depreciation_profiles.md) for more details.                          | Object    |
| asset_type            | A user created custom asset type or one of the system values from the following list: `Computer Equipment`, `Fixtures and Fittings`, `Motor Vehicles`, `Other Capital Asset`        | String    |
| purchased_on          | When the asset was purchased, in `YYYY-MM-DD` format                                                                                                                                | Date      |
| disposed_on           | When the asset was disposed of, in `YYYY-MM-DD` format                                                                                                                              | Date      |
| created_at            | Creation of the capital asset resource (UTC)                                                                                                                                        | Timestamp |
| updated_at            | When the capital asset resource was last updated (UTC)                                                                                                                              | Timestamp |
| capital_asset_history | Array of events in the capital asset life cycle. See [Capital Asset History attributes](#capital-asset-history-attributes)                                                          | Array     |

## Capital Asset History attributes

Returned if `include_history=true` is added
to the request URL when listing all capital assets or getting a single capital asset.

| Attribute   | Description                                                                               | Kind    |
| ----------- | ----------------------------------------------------------------------------------------- | ------- |
| type        | Type of event, e.g. `purchase`, `depreciation`, `annual_investment_allowance`, `disposal` | String  |
| description | Description of the event                                                                  | String  |
| date        | When the event occurred                                                                   | Date    |
| value       | Value of the underlying transaction                                                       | Decimal |
| tax_value   | Value of tax included in the underlying transaction                                       | Decimal |
| link        | URI reference to a related Bill, Bank Transaction Explanation or Expense, if available    | URI     |

## List all capital assets

```http
GET https://api.freeagent.com/v2/capital_assets
```

#### View filters

```http
GET https://api.freeagent.com/v2/capital_assets?view=all
```

- `all`
- `disposed`
- `disposable`

### Response

```http
Status: 200 OK
```

```json
{
  "capital_assets": [
    {
      "url": "https://api.freeagent.com/v2/capital_assets/1",
      "description": "Computer",
      "asset_life_years": 2,
      "asset_type": "Computer Equipment Purchase",
      "purchased_on": "2015-10-07",
      "created_at": "2015-12-07T10:19:27.000Z",
      "updated_at": "2015-12-07T10:39:45.000Z"
    },
    {
      "url": "https://api.freeagent.com/v2/capital_assets/2",
      "description": "Productivity suite subscription",
      "asset_life_years": 4,
      "asset_type": "Other Capital Asset Purchase",
      "purchased_on": "2015-10-07",
      "created_at": "2015-12-07T10:20:57.000Z",
      "updated_at": "2015-12-07T10:39:55.000Z"
    },
    {
      "url": "https://api.freeagent.com/v2/capital_assets/3",
      "description": "Workstation",
      "asset_life_years": 2,
      "asset_type": "Computer Equipment Purchase",
      "purchased_on": "2014-10-07",
      "disposed_on": "2015-12-07",
      "created_at": "2014-10-07T10:41:21.000Z",
      "updated_at": "2014-10-07T10:41:43.000Z"
    }
  ]
}
```
Show as XML

```xml
<freeagent>
  <capital-assets type="array">
    <capital-asset>
      <url>https://api.freeagent.com/v2/capital_assets/1</url>
      <description>Computer</description>
      <asset-life-years type="integer">2</asset-life-years>
      <asset-type>Computer Equipment Purchase</asset-type>
      <purchased-on type="date">2015-10-07</purchased-on>
      <created-at type="dateTime">2015-12-07T10:19:27Z</created-at>
      <updated-at type="dateTime">2015-12-07T10:39:45Z</updated-at>
    </capital-asset>
    <capital-asset>
      <url>https://api.freeagent.com/v2/capital_assets/2</url>
      <description>Productivity suite subscription</description>
      <asset-life-years type="integer">4</asset-life-years>
      <asset-type>Other Capital Asset Purchase</asset-type>
      <purchased-on type="date">2015-10-07</purchased-on>
      <created-at type="dateTime">2015-12-07T10:20:57Z</created-at>
      <updated-at type="dateTime">2015-12-07T10:39:55Z</updated-at>
    </capital-asset>
    <capital-asset>
      <url>https://api.freeagent.com/v2/capital_assets/3</url>
      <description>Workstation</description>
      <asset-life-years type="integer">2</asset-life-years>
      <asset-type>Computer Equipment Purchase</asset-type>
      <purchased-on type="date">2014-10-07</purchased-on>
      <disposed-on type="date">2015-12-07</disposed-on>
      <created-at type="dateTime">2014-10-07T10:41:21Z</created-at>
      <updated-at type="dateTime">2014-10-07T10:41:43Z</updated-at>
    </capital-asset>
  </capital-assets>
</freeagent>
```
Show as JSON

### Include capital asset history

```http
GET https://api.freeagent.com/v2/capital_assets?include_history=true
```

The `include_history` parameter can be added to the request URL to retrieve information about events in the capital
asset life cycle (such as purchase, depreciation, and disposal). If relevant, capital asset history will also
include capital allowance calculations such as annual investment allowance, first year allowances, and allocation
to the main rate or special rate pool for writing down allowances (please bear in mind that
[FreeAgent’s comprehensive capital allowances calculation](https://support.freeagent.com/hc/en-gb/articles/360015356699-How-to-use-FreeAgent-s-comprehensive-capital-allowances-calculation) is only available for assets purchased within an
accounting year ending on or after 23rd July 2020; any assets purchased within an accounting year ending before
23rd July 2020 will continue to be treated by FreeAgent as 100% allowable for tax).

If `include_history=true` is passed into the request URL, the response will include a `capital_asset_history`
array and will look as follows:

```json
{
    "capital_assets": [
        {
            "url": "https://api.freeagent.com/v2/capital_assets/2",
            "description": "New computer monitor",
            "asset_life_years": 2,
            "asset_type": "Computer Equipment Purchase",
            "purchased_on": "2019-08-19",
            "disposed_on": "2020-07-03",
            "created_at": "2020-07-08T16:55:15.000Z",
            "updated_at": "2020-07-10T16:57:09.000Z",
            "capital_asset_history": [
                {
                    "type": "purchase",
                    "description": "Computer Equipment Purchase",
                    "date": "2019-08-19",
                    "value": "1403.0",
                    "link": "https://api.freeagent.com/v2/expenses/30"
                },
                {
                    "type": "annual_investment_allowance",
                    "description": "Annual Investment Allowance",
                    "date": "2019-08-19",
                    "tax_value": "-1403.0"
                },
                {
                    "type": "depreciation",
                    "description": "Depreciation of 50%",
                    "date": "2019-08-19",
                    "value": "-701.5"
                },
                {
                    "type": "disposal",
                    "description": "Disposal",
                    "date": "2020-07-03",
                    "value": "-701.5",
                    "tax_value": "-741.67",
                    "link": "https://api.freeagent.com/v2/bank_transaction_explanations/444"
                }
            ]
        },
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <capital-assets type="array">
    <capital-asset>
      <url>https://api.freeagent.com/v2/capital_assets/3</url>
      <description>New computer monitor</description>
      <asset-life-years type="integer">3</asset-life-years>
      <asset-type>Computer Equipment Purchase</asset-type>
      <purchased-on type="date">2019-09-15</purchased-on>
      <created-at type="dateTime">2020-07-08T16:55:17Z</created-at>
      <updated-at type="dateTime">2020-07-08T16:55:17Z</updated-at>
      <capital-asset-history type="array">
        <capital-asset-history>
          <type>purchase</type>
          <description>Computer Equipment Purchase</description>
          <date type="date">2019-09-15</date>
          <value type="decimal">1044.0</value>
          https://api.freeagent.com/v2/expenses/48
        </capital-asset-history>
        <capital-asset-history>
          <type>annual_investment_allowance</type>
          <description>Annual Investment Allowance</description>
          <date type="date">2019-08-19</date>
          <tax-value type="decimal">-1403.0</tax-value>
        </capital-asset-history>
        <capital-asset-history>
            <type>depreciation</type>
            <description>Depreciation of 50%</description>
            <date type="date">2019-08-19</date>
            <value type="decimal">-701.5</value>
        </capital-asset-history>
        <capital-asset-history>
          <type>disposal</type>
          <description>Disposal</description>
          <date type="date">2020-07-03</date>
          <value type="decimal">-701.5</value>
          <tax-value type="decimal">-741.67</tax-value>
          https://api.freeagent.com/v2/bank_transaction_explanations/444
        </capital-asset-history>
      </capital-asset-history>
    </capital-asset>
  </capital-assets>
</freeagent>
```
Show as JSON

## Get a single capital asset

```http
GET https://api.freeagent.com/v2/capital_assets/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "capital_asset": {
    "url": "https://api.freeagent.com/v2/capital_assets/3",
    "description": "Workstation",
    "asset_life_years": 2,
    "asset_type": "Computer Equipment Purchase",
    "purchased_on": "2014-10-07",
    "disposed_on": "2015-12-07",
    "created_at": "2014-10-07T10:41:21.000Z",
    "updated_at": "2014-10-07T10:41:43.000Z"
  }
}
```
Show as XML

```xml
<freeagent>
  <capital-asset>
    <url>https://api.freeagent.com/v2/capital_assets/3</url>
    <description>Workstation</description>
    <asset-life-years type="integer">2</asset-life-years>
    <asset-type>Computer Equipment Purchase</asset-type>
    <purchased-on type="date">2014-10-07</purchased-on>
    <disposed-on type="date">2015-12-07</disposed-on>
    <created-at type="dateTime">2014-10-07T10:41:21Z</created-at>
    <updated-at type="dateTime">2014-10-07T10:41:43Z</updated-at>
  </capital-asset>
</freeagent>
```
Show as JSON

### Input

The `include_history` parameter can also be used for this endpoint to retrieve capital asset history events for a single item.