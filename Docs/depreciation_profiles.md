# Depreciation Profiles

A depreciation profile specifies how the depreciation of a [capital asset](capital_assets.md)  should
be calculated. The depreciation profile should be included in the request when creating or updating the entity
which relates to the purchase of the asset.
The depreciation profile will be included in the response when fetching a capital asset.

The entities which can relate to the purchase of a capital asset are:

- [bank transaction explanations](bank_transaction_explanations.md)
- [bill items](bills.md#bill-item-attributes)
- [expenses](expenses.md)

The depreciation profile should be nested inside a capital asset namespace and have the following structure:

```json
"capital_asset": {
  "depreciation_profile": {
    "method": "straight_line",
    "asset_life_years": 12,
    "frequency": "annually"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <capital_asset>
    <depreciation_profile>
      <method>straight_line</method>
      <asset_life_years>12</asset_life_years>
      <frequency>annually</frequency>
    </depreciation_profile>
  </capital_asset>
</freeagent>
```
Show as JSON

The following example shows the request to create an expense which relates to the purchase of a capital asset which should
depreciate using the straight line method, with an asset life of 10 years.

```json
{
    "expense": {
        "user": "https://api.freeagent.com/v2/users/1",
        "category": "https://api.freeagent.com/v2/categories/602-1",
        "dated_on": "2024-02-01",
        "currency": "GBP",
        "gross_value": "-20000.0",
        "description": "An example capital asset expense",
        "capital_asset": {
            "depreciation_profile": {
                "method": "straight_line",
                "asset_life_years": 10,
                "frequency": "annually"
            }
        }
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <expense>
    <user>https://api.freeagent.com/v2/users/116</user>
    <category>https://api.freeagent.com/v2/categories/602-1</category>
    <dated_on>2024-02-01</dated_on>
    <currency>GBP</currency>
    <gross_value>-20000.0</gross_value>
    <description>An example capital asset expense</description>
    <capital_asset>
      <depreciation_profile>
        <method>straight_line</method>
        <asset_life_years>10</asset_life_years>
        <frequency>annually</frequency>
      </depreciation_profile>
    </capital_asset>
  </expense>
</freeagent>
```
Show as JSON

## Valid Depreciation Methods

FreeAgent supports the following depreciation methods in the following format:

- straight_line
- reducing_balance
- no_depreciation

## Valid Depreciation Frequencies

The frequency refers to how often depreciation ledgers are posted. The following frequencies are valid:

- monthly
- annually

If no frequency is provided, the depreciation will default to monthly posting.

## Depreciation Method Specific Parameters

Each depreciation method has different parameters required to specify how depreciation
should be calculated. In each case, all of the parameters are required in addition to the
method parameter, with the exception of frequency.

### Straight Line Depreciation

| Attribute        | Description                                                                                                                                          | Kind    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| asset_life_years | The number of years before the value of the asset should be considered 0, starting from the date of purchase. This should be between 2 and 25 years. | Integer |
| frequency        | How often depreciation ledgers should be posted. [See Valid Depreciation Frequencies.](depreciation_profiles.md#valid-depreciation-frequencies)      | String  |

### Reducing Balance Depreciation

| Attribute                      | Description                                                                                                                                     | Kind    |
| ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| annual_depreciation_percentage | The percentage by which the the previous year's asset value should be reduced. This should be between 1 and 99.                                 | Integer |
| frequency                      | How often depreciation ledgers should be posted. [See Valid Depreciation Frequencies.](depreciation_profiles.md#valid-depreciation-frequencies) | String  |

### No Depreciation

No additional parameters are required for no depreciation.

## Update Requests

Entities relating to assets created using the deprecated depreciation schedule field
will be able to be updated using either `deprecation_schedule` or a `depreciation_profile`.

Assets created using a `depreciation_profile` will only be updatable using a `depreciation_profile`.

Attempting to update an asset with a depreciation schedule when the asset is using a depreciation profile
will result in the following error response.

```http
Status: 422 Unprocessable Entity
```

```json
{
    "errors": {
        "error": {
            "message": "`depreciation_schedule` is deprecated, use `depreciation_profile` instead"
        }
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <errors>
    <error>
      <message>
        `depreciation_schedule` is deprecated, use `depreciation_profile` instead
      </message>
    </error>
  </errors>
</freeagent>
```
Show as JSON