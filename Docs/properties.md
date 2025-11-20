# Properties

*Minimum access level*: `Tax, Accounting & Users`

Only applicable for companies of type `UkUnincorporatedLandlord`. Other company types cannot have or create properties.

## Attributes

 Required | Attribute | Description | Kind ||  | url | The unique identifier for the property | URI |
| ✔   | address1 | The first line of the address of the property                                                                  | String   |      |
|     | address2 | The second line of the address of the property (if applicable)                                                 | String \ | null |
|     | address3 | The third line of the address of the property (if applicable)                                                  | String \ | null |
|     | town     | The third line of the address of the property (if applicable)                                                  | String \ | null |
|     | region   | The region of the address of the property                                                                      | String \ | null |
|     | postcode | The postcode of the address of the property                                                                    | String \ | null |
|     | country  | The country of the address of the property. This currently defaults to United Kingdom and cannot be overriden. | String   |      |

## List all properties

```http
GET https://api.freeagent.com/v2/properties
```

### Response

```http
Status: 200 OK
```

```json
{
  "properties": [
    {
      "url": "https://api.freeagent.com/v2/properties/4",
      "address1": "Apartment 1901",
      "address2": "Elliott Bay Towers",
      "town": "Seattle",
      "region": "Washingon",
      "postcode": "98101",
      "country": "United Kingdom"
    },
    {
        "url": "https://api.freeagent.com/v2/properties/5",
        "address1": "Sherlock Holmes Museum",
        "address2": "221B Baker Street",
        "address3": "Marylebone",
        "town": "London",
        "region": "City of London",
        "postcode": "NW1 6XE",
        "country": "United Kingdom"
    },
    {
        "url": "https://api.freeagent.com/v2/properties/3",
        "address1": "Wayne Manor",
        "address2": "Wayne Manor Estate",
        "address3": "1007 Mountain Drive",
        "town": "Gotham City",
        "region": "New Jersey",
        "postcode": "12345",
        "country": "United Kingdom"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <properties type="array">
    <property>
      <url>https://api.freeagent.com/v2/properties/4</url>
      <address1>Apartment 1901</address1>
      <address2>Elliott Bay Towers</address2>
      <town>Seattle</town>
      <region>Washington</region>
      <postcode>98101</postcode>
      <country>United Kingdom</country>
    </property>
    <property>
      <url>https://api.freeagent.com/v2/properties/5</url>
      <address1>Sherlock Holmes Museum</address1>
      <address2>221B Baker Street</address2>
      <address3>Marylebone</address3>
      <town>London</town>
      <region>City of London</region>
      <postcode>NW1 6XE</postcode>
      <country>United Kingdom</country>
    </property>
    <property>
      <url>https://api.freeagent.com/v2/properties/3</url>
      <address1>Wayne Manor</address1>
      <address2>Wayne Manor Estate</address2>
      <address3>1007 Mountain Drive</address3>
      <town>Gotham City</town>
      <region>New Jersey</region>
      <postcode>12345</postcode>
      <country>United Kingdom</country>
    </property>
  </properties>
</freeagent>
```
Show as JSON

## Get a single property

```http
GET https://api.freeagent.com/v2/properties/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "property": {
    "url": "https://api.freeagent.com/v2/properties/3",
    "name": "Wayne Manor",
    "address2": "Wayne Manor Estate",
    "address3": "1007 Mountain Drive",
    "region": "New Jersey",
    "postcode": "12345",
    "country": "United Kingdom"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <property>
        <url>https://api.freeagent.com/v2/properties/3</url>
        <address1>Wayne Manor</address1>
        <address2>Wayne Manor Estate</address2>
        <address3>1007 Mountain Drive</address3>
        <town>Gotham City</town>
        <region>New Jersey</region>
        <postcode>12345</postcode>
        <country>United Kingdom</country>
    </property>
</freeagent>
```
Show as JSON

## Create a property

```http
POST https://api.freeagent.com/v2/properties
```

Payload should have a root `property` element, containing elements listed under Attributes.

### Response

```http
Status: 201 Created
```

```json
{
  "property": {
    "url": "https://api.freeagent.com/v2/properties/6",
    "name": "12334 Maple Boulevard",
    "town": "Middleton",
    "region": "Wisconsin",
    "country": "United Kingdom"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <property>
    <url>https://api.freeagent.com/v2/properties/6</url>
    <address1>12334 Maple Boulevard</address1>
    <town>Middleton</town>
    <region>Wisconsin</region>
    <country>United Kingdom</country>
  </property>
</freeagent>
```
Show as JSON

## Update a property

```http
PUT https://api.freeagent.com/v2/properties/:id
```

Payload should have a root `property` element, containing elements listed under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

```json
{
  "property": {
      "url": "https://api.freeagent.com/v2/properties/6",
      "name": "12334 Maple Boulevard",
      "address2": "Wilkerson Residence",
      "town": "Middleton",
      "region": "Wisconsin",
      "country": "United Kingdom"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <property>
    <url>https://api.freeagent.com/v2/properties/6</url>
    <address1>12334 Maple Boulevard</address1>
    <address2>Wilkerson Residence</address2>
    <town>Middleton</town>
    <region>Wisconsin</region>
    <country>United Kingdom</country>
  </property>
</freeagent>
```
Show as JSON

## Delete a property

```http
DELETE https://api.freeagent.com/v2/properties/:id
```

### Response

```http
Status: 200 OK
```