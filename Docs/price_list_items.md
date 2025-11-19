# Price List Items

*Minimum access level*: `Invoices, Estimates and Files`

## Attributes

| Required | Attribute             | Description                                                                                                                                                                 | Kind      |
| -------- | --------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                   | The unique identifier for the price list item                                                                                                                               | URI       |
| ✔        | code                  | A unique name / code used to identify the item when adding it to an [invoice](invoices.md) or [estimate](estimates.md)                                                      | String    |
| ✔        | quantity              | Item quantity                                                                                                                                                               | Decimal   |
| ✔        | item_type             | One of the following: `Hours`, `Days`, `Weeks`, `Months`, `Years`, `Products`, `Services`, `Training`, `Expenses`, `Comment`, `Bills`, `Discount`, `Credit`, `VAT`, `Stock` | String    |
| ✔        | description           | Free-text description of the item                                                                                                                                           | String    |
| ✔        | price                 | The unit price of one item                                                                                                                                                  | Decimal   |
|          | vat_status            | [UK accounts only] One of the following: `out_of_scope` (default), `reduced`, `standard`, `zero`                                                                            | String    |
|          | sales_tax_rate        | [Universal and US accounts only] One of the standard [sales tax](sales_tax.md) rates                                                                                        | Decimal   |
|          | second_sales_tax_rate | [Universal accounts only] One of the standard [second sales tax](sales_tax.md) rates                                                                                        | Decimal   |
|          | category              | Income [accounting category](categories.md) of the item                                                                                                                     | URI       |
|          | stock_item            | [Stock item](stock_items.md), if `item_type` is stock                                                                                                                       | URI       |
|          | created_at            | Creation of the price list item (UTC)                                                                                                                                       | Timestamp |
|          | updated_at            | When the price list item was last updated (UTC)                                                                                                                             | Timestamp |

## List all price list items

```http
GET https://api.freeagent.com/v2/price_list_items
```

#### Sort Orders

```http
GET https://api.freeagent.com/v2/price_list_items?sort=created_at
```

- `created_at`: Sort by the time the price list item was created (default).
- `code`: Sort by the price list item code.
- `updated_at`: Sort by the time the price list item was updated.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/price_list_items?sort=-created_at
```

### Response

```json
{
  "price_list_items": [
    {
      "url": "https://api.freeagent.com/v2/price_list_items/1",
      "code": "A001",
      "item_type": "Products",
      "quantity": "1.0",
      "price": "10.99",
      "description": "Apple",
      "sales_tax_rate": "0.0",
      "second_sales_tax_rate": "0.0",
      "vat_status": "standard",
      "category": "https://api.freeagent.com/v2/categories/2",
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
  <price-list-items type="array">
    <price-list-item>
      <url>https://api.freeagent.com/v2/price_list_items/1</url>
      <code>A001</code>
      <item-type>Products</item-type>
      <quantity>1.0</quantity>
      <price>10.99</price>
      <description>Apple</description>
      <sales-tax-rate>0.0</sales-tax-rate>
      <second-sales-tax-rate>0.0</second-sales-tax-rate>
      <vat-status>standard</vat-status>
      <category>https://api.freeagent.com/v2/categories/2</category>
      <created-at type="datetime">2023-05-22T09:22:45Z</created-at>
      <updated-at type="datetime">2023-05-25T12:43:36Z</updated-at>
    </price-list-item>
  </price-list-items>
</freeagent>
```
Show as JSON

## Get a single price list item

```http
GET https://api.freeagent.com/v2/price_list_items/:id
```

### Response

```http
Status: 200 OK
```

```json
{
  "price_list_item": {
    "url": "https://api.freeagent.com/v2/price_list_items/1",
    "code": "A001",
    "item_type": "Products",
    "quantity": "1.0",
    "price": "10.99",
    "description": "Apple",
    "sales_tax_rate": "0.0",
    "second_sales_tax_rate": "0.0",
    "vat_status": "standard",
    "category": "https://api.freeagent.com/v2/categories/2",
    "created_at":"2023-05-22T09:22:45Z",
    "updated_at":"2023-05-25T12:43:36Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <price-list-item>
    <url>https://api.freeagent.com/v2/price_list_items/1</url>
    <code>A001</code>
    <item-type>Products</item-type>
    <quantity>1.0</quantity>
    <price>10.99</price>
    <description>Apple</description>
    <sales-tax-rate>0.0</sales-tax-rate>
    <second-sales-tax-rate>0.0</second-sales-tax-rate>
    <vat-status>standard</vat-status>
    <category>https://api.freeagent.com/v2/categories/2</category>
    <created-at type="datetime">2023-05-22T09:22:45Z</created-at>
    <updated-at type="datetime">2023-05-25T12:43:36Z</updated-at>
  </price-list-item>
</freeagent>
```
Show as JSON

## Create a price list item

```http
POST https://api.freeagent.com/v2/price_list_items
```

Payload should have a root `price_list_item` element, containing elements listed
under Attributes.

### Example Request Body

```json
{
  "price_list_item": {
    "code": "A001",
    "description": "Apple",
    "vat_status": "standard",
    "price": "1.99",
    "item_type": "Products",
    "quantity": "1"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<price-list-item>
  <code>A001</code>
  <description>Apple</description>
  <vat-status>standard</vat-status>
  <price type="decimal">1.99</price>
  <item-type>Products</item-type>
  <quantity type="decimal">1</quantity>
</price-list-item>
```
Show as JSON

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/price_list_items/17
```

```json
{
  "price_list_item": {
    "category": "https://api.freeagent.com/v2/categories/001",
    "code": "A001",
    "vat_status": "standard",
    "description": "Apple",
    "url": "https://api.freeagent.com/v2/price_list_items/17",
    "sales_tax_rate": "0.0",
    "price": "1.99",
    "item_type": "Products",
    "second_sales_tax_rate": "0.0",
    "quantity": "1.0",
    "created_at":"2023-05-25T12:43:36Z",
    "updated_at":"2023-05-25T12:43:36Z"
  }
}

```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <price-list-item>
    <url>https://api.freeagent.com/v2/price_list_items/17</url>
    <code>A001</code>
    <item-type>Products</item-type>
    <quantity type="decimal">1.0</quantity>
    <price type="decimal">1.99</price>
    <description>Apple</description>
    <sales-tax-rate type="decimal">0.0</sales-tax-rate>
    <second-sales-tax-rate type="decimal">0.0</second-sales-tax-rate>
    <vat-status>standard</vat-status>
    <category>https://api.freeagent.com/v2/categories/001</category>
    <created-at type="datetime">2023-05-25T12:43:36Z</created-at>
    <updated-at type="datetime">2023-05-25T12:43:36Z</updated-at>
  </price-list-item>
</freeagent>
```
Show as JSON

## Update a price list item

```http
PUT https://api.freeagent.com/v2/price_list_items/:id
```

Payload must have a root `price_list_item` element, containing elements listed
under Attributes that should be updated.

### Example Request Body

```json
{
  "price_list_item": {
    "description": "Pear",
    "price": "3.99"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<price-list-item>
  <description>Pear</description>
  <price type="decimal">3.99</price>
</price-list-item>
```
Show as JSON

### Response

```http
Status: 200 OK
```

## Delete a price list item

```http
DELETE https://api.freeagent.com/v2/price_list_item/:id
```

### Response

```http
Status: 200 OK
```