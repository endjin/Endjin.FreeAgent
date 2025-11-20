# Stock Items

*Minimum access level*: `Invoices, Estimates and Files`

Provides read-only access to the list of stock items defined in the FreeAgent account.

## Attributes

| Attribute             | Description                                                                                                                | Kind      |
| --------------------- | -------------------------------------------------------------------------------------------------------------------------- | --------- |
| url                   | The unique identifier for the stock item                                                                                   | URI       |
| description           | Free-text description / code to identify the item when it's added to an [invoice](invoices.md) or [estimate](estimates.md) | String    |
| opening_quantity      | Stock on hand as of FreeAgent Start Date                                                                                   | Integer   |
| opening_balance       | The value of stock on hand as of FreeAgent Start Date                                                                      | Decimal   |
| cost_of_sale_category | The [spending category](categories.md) which will be used to account for sales of this stock item                          | URI       |
| stock_on_hand         | Stock on hand as of today                                                                                                  | Integer   |
| created_at            | Creation of the stock item (UTC)                                                                                           | Timestamp |
| updated_at            | When the stock item was last updated (UTC)                                                                                 | Timestamp |

## List all Stock Items

```http
GET https://api.freeagent.com/v2/stock_items
```

#### Sort Orders

```http
GET https://api.freeagent.com/v2/stock_items?sort=created_at
```

- `created_at`: Sort by the time the stock item was created (default).
- `description`: Sort by the stock item description / code.
- `updated_at`: Sort by the time the stock item was updated.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/stock_items?sort=-created_at
```

### Response

```http
Status: 200 OK
```

```json
{
  "stock_items": [
    {
      "url": "https://api.freeagent.com/v2/stock_items/3",
      "description": "Apple",
      "opening_quantity": "10.0",
      "opening_balance": "1.0",
      "cost_of_sale_category": "https://api.freeagent.com/v2/categories/2",
      "stock_on_hand": "10.0",
      "created_at":"2023-05-22T09:22:45Z",
      "updated_at":"2023-05-25T12:43:36Z"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <stock-items type="array">
    <stock-item>
      <url>https://api.freeagent.com/v2/stock_items/3</url>
      <description>Apple</description>
      <opening-quantity type="decimal">10.0</opening-quantity>
      <opening-balance type="decimal">1.0</opening-balance>
      <cost-of-sale-category>https://api.freeagent.com/v2/categories/2</cost-of-sale-category>
      <stock-on-hand type="decimal">10.0</stock-on-hand>
      <created-at type="datetime">2023-05-22T09:22:45Z</created-at>
      <updated-at type="datetime">2023-05-25T12:43:36Z</updated-at>
    </stock-item>
  </stock-items>
</freeagent>
```
Show as JSON

## Get a single Stock Item

```http
GET https://api.freeagent.com/v2/stock_items/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "stock_item": {
    "url": "https://api.freeagent.com/v2/stock_items/3",
    "description": "Apple",
    "opening_quantity": "10.0",
    "opening_balance": "1.0",
    "cost_of_sale_category": "https://api.freeagent.com/v2/categories/2",
    "stock_on_hand": "10.0",
    "created_at":"2023-05-22T09:22:45Z",
    "updated_at":"2023-05-25T12:43:36Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <stock-item>
    <url>https://api.freeagent.com/v2/stock_items/3</url>
    <description>Apple</description>
    <opening-quantity type="decimal">10.0</opening-quantity>
    <opening-balance type="decimal">1.0</opening-balance>
    <cost-of-sale-category>https://api.freeagent.com/v2/categories/2</cost-of-sale-category>
    <stock-on-hand type="decimal">10.0</stock-on-hand>
    <created-at type="datetime">2023-05-22T09:22:45Z</created-at>
    <updated-at type="datetime">2023-05-25T12:43:36Z</updated-at>
  </stock-item>
</freeagent>
```
Show as JSON