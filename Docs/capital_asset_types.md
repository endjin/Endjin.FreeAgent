# Capital Asset Types

*Minimum access level*: `Full Access`.

## Attributes

| Required | Attribute      | Description                                                                                                                                                                          | Kind      |
| -------- | -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- |
|          | url            | The unique identifier for the capital asset type                                                                                                                                     | URI       |
| ✔        | name           | Name of a user-created custom asset type or one of the system values from the following list: `Computer Equipment`, `Fixtures and Fittings`, `Motor Vehicles`, `Other Capital Asset` | String    |
|          | system_default | `true` for default capital asset types, `false` if it was created by a user                                                                                                          | Boolean   |
|          | created_at     | Creation of the capital asset type (UTC)                                                                                                                                             | Timestamp |
|          | updated_at     | When the capital asset type was last updated (UTC)                                                                                                                                   | Timestamp |

## List all capital assets

```http
GET https://api.freeagent.com/v2/capital_asset_types
```

### Response

```http
Status: 200 OK
```

```json
{
    "capital_asset_types": [
        {
            "url": "https://api.freeagent.com/v2/capital_asset_types/397",
            "name": "Computer Equipment",
            "system_default": true,
            "created_at": "2020-04-22T11:32:24.000Z",
            "updated_at": "2020-04-22T11:32:24.000Z"
        },
        {
            "url": "https://api.freeagent.com/v2/capital_asset_types/398",
            "name": "Fixtures and Fittings",
            "system_default": true,
            "created_at": "2020-04-22T11:32:24.000Z",
            "updated_at": "2020-04-22T11:32:24.000Z"
        },
        {
            "url": "https://api.freeagent.com/v2/capital_asset_types/399",
            "name": "Motor Vehicles",
            "system_default": true,
            "created_at": "2020-04-22T11:32:24.000Z",
            "updated_at": "2020-04-22T11:32:24.000Z"
        },
        {
            "url": "https://api.freeagent.com/v2/capital_asset_types/400",
            "name": "Other Capital Asset",
            "system_default": true,
            "created_at": "2020-04-22T11:32:24.000Z",
            "updated_at": "2020-04-22T11:32:24.000Z"
        },
        {
            "url": "https://api.freeagent.com/v2/capital_asset_types/445",
            "name": "User-Created Type",
            "system_default": false,
            "created_at": "2020-05-20T20:48:05.000Z",
            "updated_at": "2020-05-20T20:48:05.000Z"
        }
    ]
}
```
Show as XML

```xml
<freeagent>
    <capital-asset-types type="array">
        <capital-asset-type>
            <url>https://api.freeagent.com/v2/capital_asset_types/397</url>
            <name>Computer Equipment</name>
            <system-default type="boolean">true</system-default>
            <created-at type="dateTime">2020-04-22T11:32:24Z</created-at>
            <updated-at type="dateTime">2020-04-22T11:32:24Z</updated-at>
        </capital-asset-type>
        <capital-asset-type>
            <url>https://api.freeagent.com/v2/capital_asset_types/398</url>
            <name>Fixtures and Fittings</name>
            <system-default type="boolean">true</system-default>
            <created-at type="dateTime">2020-04-22T11:32:24Z</created-at>
            <updated-at type="dateTime">2020-04-22T11:32:24Z</updated-at>
        </capital-asset-type>
        <capital-asset-type>
            <url>https://api.freeagent.com/v2/capital_asset_types/399</url>
            <name>Motor Vehicles</name>
            <system-default type="boolean">true</system-default>
            <created-at type="dateTime">2020-04-22T11:32:24Z</created-at>
            <updated-at type="dateTime">2020-04-22T11:32:24Z</updated-at>
        </capital-asset-type>
        <capital-asset-type>
            <url>https://api.freeagent.com/v2/capital_asset_types/400</url>
            <name>Other Capital Asset</name>
            <system-default type="boolean">true</system-default>
            <created-at type="dateTime">2020-04-22T11:32:24Z</created-at>
            <updated-at type="dateTime">2020-04-22T11:32:24Z</updated-at>
        </capital-asset-type>
        <capital-asset-type>
            <url>https://api.freeagent.com/v2/capital_asset_types/445</url>
            <name>User-Created Type</name>
            <system-default type="boolean">false</system-default>
            <created-at type="dateTime">2020-05-20T20:48:05Z</created-at>
            <updated-at type="dateTime">2020-05-20T20:48:05Z</updated-at>
        </capital-asset-type>
    </capital-asset-types>
</freeagent>
```
Show as JSON

## Get a single capital asset type

```http
GET https://api.freeagent.com/v2/capital_asset_types/:id
```

### Response

```http
Status: 200 OK
```

```json
{
    "capital_asset_type": {
        "url": "https://api.freeagent.com/v2/capital_asset_types/397",
        "name": "Computer Equipment",
        "system_default": true,
        "created_at": "2020-04-22T11:32:24.000Z",
        "updated_at": "2020-04-22T11:32:24.000Z"
    }
}
```
Show as XML

```xml
<freeagent>
    <capital-asset-type>
        <url>https://api.freeagent.com/v2/capital_asset_types/397</url>
        <name>Computer Equipment</name>
        <system-default type="boolean">true</system-default>
        <created-at type="dateTime">2020-04-22T11:32:24Z</created-at>
        <updated-at type="dateTime">2020-04-22T11:32:24Z</updated-at>
    </capital-asset-type>
</freeagent>
```
Show as JSON

## Create a capital asset type

```http
POST https://api.freeagent.com/v2/capital_asset_types
```

Payload should have a root `capital_asset_type` element containing the `name` attribute.

### Example Request Body

```json
{
    "capital_asset_type": {
        "name": "Spaceships"
    }
}
```
Show as XML

```xml
<capital_asset_type>
  <name>Spaceships</name>
</capital_asset_type>
```
Show as JSON

### Response

```http
Status: 201 Created
```

```json
{
    "capital_asset_type": {
        "url": "https://api.freeagent.com/v2/capital_asset_types/401",
        "name": "Spaceships",
        "system_default": false,
        "created_at": "2020-12-09T16:47:17.000Z",
        "updated_at": "2020-12-09T16:47:17.000Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <capital-asset-type>
        <url>https://api.freeagent.com/v2/capital_asset_types/401</url>
        <name>Spaceships</name>
        <system-default type="boolean">false</system-default>
        <created-at type="dateTime">2020-12-09T16:50:49Z</created-at>
        <updated-at type="dateTime">2020-12-09T16:50:49Z</updated-at>
    </capital-asset-type>
</freeagent>
```
Show as JSON

## Update a capital asset type

Only user created capital asset types that do not contain any items can be updated.

```http
PUT https://api.freeagent.com/v2/capital_asset_types/:id
```

Payload should have a root `capital_asset_type` element containing the `name` attribute.

### Example Request Body

```json
{
    "capital_asset_type": {
        "name": "Spacetrains"
    }
}
```
Show as XML

```xml
<capital_asset_type>
  <name>Spacetrains</name>
</capital_asset_type>
```
Show as JSON

### Response

```http
Status: 200 OK
```

```json
{
    "capital_asset_type": {
        "url": "https://api.freeagent.com/v2/capital_asset_types/401",
        "name": "Spacetrains",
        "system_default": false,
        "created_at": "2020-12-09T17:15:54.000Z",
        "updated_at": "2020-12-09T17:15:54.000Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <capital-asset-type>
        <url>https://api.freeagent.com/v2/capital_asset_types/401</url>
        <name>Spacetrains</name>
        <system-default type="boolean">false</system-default>
        <created-at type="dateTime">2020-12-09T17:20:49Z</created-at>
        <updated-at type="dateTime">2020-12-09T17:20:49Z</updated-at>
    </capital-asset-type>
</freeagent>
```
Show as JSON

## Delete a capital asset type

Only user created capital asset types that do not contain any items can be deleted.

```http
DELETE https://api.freeagent.com/v2/capital_asset_types/:id
```

### Response

```http
Status: 200 OK
```

```json
{
    "capital_asset_type": {
        "url": "https://api.freeagent.com/v2/capital_asset_types/401",
        "name": "Spacetrains",
        "system_default": false,
        "created_at": "2020-12-09T17:15:54.000Z",
        "updated_at": "2020-12-09T17:15:54.000Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <capital-asset-type>
        <url>https://api.freeagent.com/v2/capital_asset_types/401</url>
        <name>Spacetrains</name>
        <system-default type="boolean">false</system-default>
        <created-at type="dateTime">2020-12-09T17:20:49Z</created-at>
        <updated-at type="dateTime">2020-12-09T17:20:49Z</updated-at>
    </capital-asset-type>
</freeagent>
```
Show as JSON