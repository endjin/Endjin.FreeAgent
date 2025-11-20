# Hire purchases

*Only available to UK companies*

*Minimum access level*: `Bills`

Hire purchases can be created or removed by passing the relevant attribute to the [Bills API](bills.md)

## Read-only Attributes

| Required | Attribute                           | Description                                                                         | Kind |
| -------- | ----------------------------------- | ----------------------------------------------------------------------------------- | ---- |
|          | url                                 | The unique identifier for the hire purchase                                         | URI  |
|          | description                         | The free-text description of the hire purchase, taken from the bill                 | URI  |
|          | bill                                | The unique identifier for the bill for this hire purchase                           | URI  |
|          | url                                 | The unique identifier for the hire purchase                                         | URI  |
|          | liabilities_over_one_year_category  | The unique identifier for the hire purchase's account for liabilities over one year | URI  |
|          | liabilities_under_one_year_category | The unique identifier for the hire purchase's account for liabilities within a year | URI  |

## List all hire purchases

Returns a list of hire purchases

```http
GET https://api.freeagent.com/v2/hire_purchases
```

### Response

```http
Status: 200 OK
```

```json
{ "hire_purchases": [
  {
    "url":"https://api.freeagent.com/v2/hire_purchases/1",
    "description":"My hire purchase bill",
    "bill":"https://api.freeagent.com/v2/bills/1",
    "liabilities_over_one_year_category":"https://api.freeagent.com/v2/categories/793-1",
    "liabilities_under_one_year_category":"https://api.freeagent.com/v2/categories/792-1"
  }
] }
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <hire-purchases type="array">
    <hire-purchase>
      <url>https://api.freeagent.com/v2/hire_purchases/1</url>
      <description>My hire purchase bill</description>
      <bill>https://api.freeagent.com/v2/bills/1</bill>
      https://api.freeagent.com/v2/categories/793-1
      https://api.freeagent.com/v2/categories/792-1
    </hire-purchase>
  </hire-purchases>
<freeagent>
```
Show as JSON

## Get a single hire purchase

```http
GET https://api.freeagent.com/v2/hire_purchases/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "hire_purchase":
  {
    "url":"https://api.freeagent.com/v2/hire_purchases/1",
    "description":"My hire purchase bill",
    "bill":"https://api.freeagent.com/v2/bills/1",
    "liabilities_over_one_year_category":"https://api.freeagent.com/v2/categories/793-1",
    "liabilities_under_one_year_category":"https://api.freeagent.com/v2/categories/792-1"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <hire-purchase>
    <url>https://api.freeagent.com/v2/hire_purchases/1</url>
    <description>My hire purchase bill</description>
    <bill>https://api.freeagent.com/v2/bills/1</bill>
    https://api.freeagent.com/v2/categories/793-1
    https://api.freeagent.com/v2/categories/792-1
  </hire-purchase>
</freeagent>
```
Show as JSON