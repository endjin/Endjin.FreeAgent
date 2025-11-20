# Categories

*Minimum access level*: `My Money`, unless stated otherwise.

Categories are used to explain the type of income or cost for estimate items, invoice items, expenses, bills and bank transaction explanations.

## Attributes

| Attribute           | Description                                                                                                 | Kind    |
| ------------------- | ----------------------------------------------------------------------------------------------------------- | ------- |
| url                 | The unique identifier for the category                                                                      | URI     |
| description         | Category name                                                                                               | String  |
| nominal_code        | Category code                                                                                               | String  |
| group_description   | Name of the group to which the category belongs                                                             | String  |
| auto_sales_tax_rate | One of the following: `Outside of the scope of VAT`, `Zero rate`, `Reduced rate`, `Standard rate`, `Exempt` | String  |
| group_description   | Name of the group to which the category belongs                                                             | String  |
| allowable_for_tax   | `true` if cost can be deducted from income when working out your tax bill, `false` otherwise                | Boolean |
| tax_reporting_name  | Where the category is reported in the Statutory Accounts                                                    | String  |
| auto_sales_tax_rate | One of the following: `Outside of the scope of VAT`, `Zero rate`, `Reduced rate`, `Standard rate`           | String  |
| bank_account        | The bank account represented by the sub account. Will be present on 750-x categories                        | URI     |
| capital_asset_type  | The capital asset type represented by the sub account. Will be present on 601-x to 607-x categories         | URI     |
| stock_item          | The stock item represented by the sub account. Will be present on 609-x categories                          | URI     |
| hire_purchase       | The hire purchase represented by the sub account. Will be present on 792-x to 793-x categories              | URI     |
| user                | The user represented by the sub account. Will be present on 900-x to 910-x categories                       | URI     |

## List all categories

Returns the list of all categories for the current company in four sets:
Admin Expenses, Cost of Sales, Income and General.  The list of
categories varies between companies as users can create custom
categories.

```http
GET https://api.freeagent.com/v2/categories
```

### Input

#### List all categories including sub accounts

Returns the list of all categories as above except it includes sub accounts
in the list instead of the associated top level accounts where they exist.
Sub accounts can be identified by inspecting the nominal code which will
include a - sign and a sub code - for example `602-1`.

```http
GET https://api.freeagent.com/v2/categories?sub_accounts=true
```

### Response

```http
Status: 200 OK
```

For clarity, only one category per set is shown below

```json
{
  "admin_expenses_categories": [
    {
        "url": "https://api.freeagent.com/v2/categories/285",
        "description": "Accommodation and Meals",
        "nominal_code": "285",
        "allowable_for_tax": true,
        "tax_reporting_name": "Travel and subsistence expenses",
        "auto_sales_tax_rate": "Standard rate"
    },
  ],
  "cost_of_sales_categories": [
    {
        "url": "https://api.freeagent.com/v2/categories/102",
        "description": "Commission Paid",
        "nominal_code": "102",
        "allowable_for_tax": true,
        "tax_reporting_name": "Commissions Payable",
        "auto_sales_tax_rate": "Standard rate"
    }
  ],
  "income_categories": [
    {
        "url": "https://api.freeagent.com/v2/categories/001",
        "description": "Sales",
        "nominal_code": "001",
        "auto_sales_tax_rate": "Standard rate"
    }
  ],
  "general_categories": [
    {
        "url": "https://api.freeagent.com/v2/categories/051",
        "description": "Interest Received",
        "nominal_code": "051"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <admin-expenses-categories type="array">
    <admin-expenses-category>
      <url>https://api.freeagent.com/v2/categories/285</url>
      <description>Accommodation and Meals</description>
      <nominal-code>285</nominal-code>
      <allowable-for-tax type="boolean">true</allowable-for-tax>
      <tax-reporting-name>Travel and subsistence expenses</tax-reporting-name>
      <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
    </admin-expenses-category>
  </admin-expenses-categories>
  <cost-of-sales-categories type="array">
    <cost-of-sales-category>
      <url>https://api.freeagent.com/v2/categories/102</url>
      <description>Commission Paid</description>
      <nominal-code>102</nominal-code>
      <allowable-for-tax type="boolean">true</allowable-for-tax>
      <tax-reporting-name>Commissions Payable</tax-reporting-name>
      <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
    </cost-of-sales-category>
  </cost-of-sales-categories>
  <income-categories type="array">
    <income-category>
      <url>https://api.freeagent.com/v2/categories/001</url>
      <description>Sales</description>
      <nominal-code>001</nominal-code>
      <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
    </income-category>
  </income-categories>
  <general-categories type="array">
    <general-category>
      <url>https://api.freeagent.com/v2/categories/051</url>
      <description>Interest Received</description>
      <nominal-code>051</nominal-code>
  </general-categories>
<freeagent>
```
Show as JSON

## Get a single category

```http
GET https://api.freeagent.com/v2/categories/:nominal_code
```

### Response

```http
Status: 200 OK
```

```json
{
  "income_categories": {
      "url": "https://api.freeagent.com/v2/categories/001",
      "description": "Sales",
      "nominal_code": "001",
      "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <income-categories>
    <url>https://api.freeagent.com/v2/categories/001</url>
    <description>Sales</description>
    <nominal-code>001</nominal-code>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </income-categories>
</freeagent>
```
Show as JSON

## Create a category

```http
POST https://api.freeagent.com/v2/categories
```

Payload should have a root `category` element, containing attributes allowed for the specific category type.

### Create an income category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 001 to 049
- `category_group` - must be equal to `income`

#### Example request body

```json
{
  "category": {
    "description": "Custom Income Category",
    "nominal_code": "047",
    "category_group": "income"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Income Category</description>
  <nominal-code>047</nominal-code>
  <category-group>income</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "income_categories": {
    "url": "https://api.freeagent.com/v2/categories/047",
    "description": "Custom Income Category",
    "group_description": "Income (normally VATable)",
    "nominal_code": "047",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <income-categories>
    <url>https://api.freeagent.com/v2/categories/047</url>
    <description>Custom Income Category</description>
    <group-description>Income (normally VATable)</group-description>
    <nominal-code>047</nominal-code>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </income-categories>
</freeagent>
```
Show as JSON

### Create a cost of sales category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 096 to 199
- `tax_reporting_name` - see below for valid values by company type
- `allowable_for_tax`
- `auto_sales_tax_rate` - defaults to `Standard rate` if not specified
- `category_group` - must be equal to `cost_of_sales`

  **Valid tax reporting names by company type**

 UK Limited Company | UK Sole Trader || `commissions_payable` | `cost_of_goods` |
| `material_costs`                                     | `subcontractor_costs`                                |
| `purchases`                                          | `construction_industry_scheme_subcontractor_costs`\* |
| `subcontractor_costs`                                |                                                      |
| `construction_industry_scheme_subcontractor_costs`\* |                                                      |

\* These tax reporting names are only available if CIS for Contractors is switched on

 UK Partnership | Universal and US Companies || `cost_of_sales` | `cost_of_labor` |
| `subcontractor_costs` | `materials_and_supplies` |     |
|                       | `other_costs`            |     |
|                       | `purchases`              |     |

#### Example request body

```json
{
  "category": {
    "description": "Custom Cost of Sales Category",
    "nominal_code": "101",
    "tax_reporting_name": "purchases",
    "allowable_for_tax": true,
    "auto_sales_tax_rate": "Standard rate",
    "category_group": "cost_of_sales"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Cost of Sales Category</description>
  <nominal-code>101</nominal-code>
  <tax_reporting_name>purchases</tax_reporting_name>
  <allowable-for-tax>true</allowable-for-tax>
  <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  <category-group>cost_of_sales</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "cost_of_sales_categories": {
    "url": "https://api.freeagent.com/v2/categories/101",
    "description": "Custom Cost of Sales Category",
    "group_description": "Cost of sales (normally VATable)",
    "nominal_code": "101",
    "allowable_for_tax": true,
    "tax_reporting_name": "Purchases",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <cost-of-sales-categories>
    <url>https://api.freeagent.com/v2/categories/101</url>
    <description>Custom Cost of Sales Category</description>
    <group-description>Cost of sales (normally VATable)</group-description>
    <nominal-code>101</nominal-code>
    <allowable-for-tax type="boolean">true</allowable-for-tax>
    <tax-reporting-name>Purchases</tax-reporting-name>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </cost-of-sales-categories>
</freeagent>
```
Show as JSON

### Create an admin expenses category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 200 to 399
- `tax_reporting_name` - see below for valid values by company type
- `allowable_for_tax`
- `auto_sales_tax_rate` - defaults to `Standard rate` if not specified
- `category_group` - must be equal to `admin_expenses`

  **Valid tax reporting names by company type**

| UK Limited Company                                | UK Sole Trader                         |
| ------------------------------------------------- | -------------------------------------- |
| `accountancy_fees`                                | `accountancy_and_legal_fees`           |
| `advertising_and_promotional_costs`               | `advertising_costs`                    |
| `amortisation_of_intangible_assets`               | `bank_and_loan_interest`               |
| `bad_debts_written_off`                           | `car_van_and_travel_expenses`          |
| `bank_charges`                                    | `debts_written_off`                    |
| `business_entertaining`                           | `depreciation_and_loss_profit_on_sale` |
| `canteen`                                         | `entertainment_costs`                  |
| `charitable_donations`                            | `other_business_expenses`              |
| `computer_software_costs`                         | `other_finance_charges`                |
| `consumable_items`                                | `phone_and_other_office_costs`         |
| `credit_card_charges`                             | `rent_and_other_property_costs`        |
| `depreciation_of_tangible_fixed_assets`           | `repair_and_renewal_costs`             |
| `directors_pensions`                              | `wages_salaries_and_staff_costs`       |
| `directors_remuneration`                          |                                        |
| `employers_ni_directors`                          |                                        |
| `employers_ni_staff`                              |                                        |
| `finance_charges`                                 |                                        |
| `foreign_exchange_transaction_charges`            |                                        |
| `gain_from_disposal_of_tangible_fixed_assets`     |                                        |
| `gain_on_foreign_currency_transactions`           |                                        |
| `general_consultancy_fees`                        |                                        |
| `general_maintenance`                             |                                        |
| `hire_and_leasing_of_computer_equipment`          |                                        |
| `hire_and_leasing_of_motor_vehicles`              |                                        |
| `hire_and_leasing_of_other_assets`                |                                        |
| `it_and_computer_consumables`                     |                                        |
| `insurance`                                       |                                        |
| `insurance_on_premises`                           |                                        |
| `interest_payable`                                |                                        |
| `irrecoverable_vat`                               |                                        |
| `late_payment_of_tax`                             |                                        |
| `leases_and_hire_purchase_contracts`              |                                        |
| `legal_fees`                                      |                                        |
| `management_fees`                                 |                                        |
| `other_legal_and_professional_fees`               |                                        |
| `political_donations`                             |                                        |
| `postage_costs`                                   |                                        |
| `premises_cleaning`                               |                                        |
| `premises_repairs_and_maintenance`                |                                        |
| `premises_repairs_and_renewals`                   |                                        |
| `printing_costs`                                  |                                        |
| `publication_and_other_information_subscriptions` |                                        |
| `rates_on_premises`                               |                                        |
| `rent_of_premises`                                |                                        |
| `research_and_development_costs`                  |                                        |
| `staff_benefits_in_kind`                          |                                        |
| `staff_entertaining`                              |                                        |
| `staff_pensions`                                  |                                        |
| `staff_training`                                  |                                        |
| `staff_welfare`                                   |                                        |
| `stationery`                                      |                                        |
| `subscriptions_to_professional_and_trade_bodies`  |                                        |
| `sundry_expenses`                                 |                                        |
| `telecommunication_costs`                         |                                        |
| `travel_and_subsistence`                          |                                        |
| `use_of_residence`                                |                                        |
| `vehicle_running_costs`                           |                                        |
| `wages_and_salaries`                              |                                        |

| UK Partnership                    | Universal and US Companies             |
| --------------------------------- | -------------------------------------- |
| `advertising_etc`                 | `advertising`                          |
| `bad_debts`                       | `bad_debts_written_off`                |
| `business_entertaining`           | `car_and_truck_expenses`               |
| `depreciation_and_loss`           | `commissions_and_fees`                 |
| `employee_costs`                  | `contract_labor`                       |
| `general_administrative_expenses` | `depletion`                            |
| `interest`                        | `depreciation`                         |
| `legal_and_professional_costs`    | `employee_benefit_programs`            |
| `motor_expenses`                  | `expenses_for_business_use_of_home`    |
| `other_direct_costs`              | `insurance`                            |
| `other_expenses`                  | `mortgage_interest`                    |
| `other_finance_charges`           | `other_interest`                       |
| `premises_costs`                  | `legal_and_professional_services`      |
| `repairs`                         | `meals_and_entertainment`              |
| `travel_and_subsistence`          | `office_expense`                       |
|                                   | `other_expenses`                       |
|                                   | `pension_and_profit_sharing_plans`     |
|                                   | `other_business_property_rent`         |
|                                   | `vehicle_machinery_and_equipment_rent` |
|                                   | `repairs_and_maintenance`              |
|                                   | `supplies`                             |
|                                   | `taxes_and_licenses`                   |
|                                   | `travel`                               |
|                                   | `utilities`                            |
|                                   | `wages`                                |

#### Example request body

```json
{
  "category": {
    "description": "Custom Admin Expenses Category",
    "nominal_code": "212",
    "tax_reporting_name": "computer_software_costs",
    "allowable_for_tax": true,
    "auto_sales_tax_rate": "Standard rate",
    "category_group": "admin_expenses"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Admin Expenses Category</description>
  <nominal-code>212</nominal-code>
  <tax_reporting_name>computer_software_costs</tax_reporting_name>
  <allowable-for-tax>true</allowable-for-tax>
  <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  <category-group>admin_expenses</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "admin_expenses_categories": {
    "url": "https://api.freeagent.com/v2/categories/213",
    "description": "Custom Admin Expenses Category",
    "group_description": "Admin expenses (normally VATable)",
    "nominal_code": "213",
    "allowable_for_tax": "true",
    "tax_reporting_name": "Computer software costs",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <admin-expenses-categories>
    <url>https://api.freeagent.com/v2/categories/213</url>
    <description>Custom Admin Expenses Category</description>
    <group-description>Admin expenses (normally VATable)</group-description>
    <nominal-code>213</nominal-code>
    <allowable-for-tax type="boolean">true</allowable-for-tax>
    <tax-reporting-name>Computer software costs</tax-reporting-name>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </admin-expenses-categories>
</freeagent>
```
Show as JSON

### Create a current asset category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 671 to 720
- `tax_reporting_name` - see below for valid values by company type
- `category_group` - must be equal to `current_assets`

  **Valid tax reporting names by company type**

 All company types || `debtors` |
| `money_in_transit`               |
| `prepayments_and_accrued_income` |

#### Example request body

```json
{
  "category": {
    "description": "Custom Assets Category",
    "nominal_code": "672",
    "category_group": "current_assets",
    "tax_reporting_name": "debtors"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Assets Category</description>
  <nominal-code>673</nominal-code>
  <tax-reporting-name>debtors</tax-reporting-name>
  <category-group>current_assets</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/672",
    "description": "Custom Assets Category",
    "nominal_code": "672"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/673</url>
    <description>Custom Assets Category</description>
    <nominal-code>673</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

### Create a liabilities category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 731 to 780
- `tax_reporting_name` - see below for valid values by company type
- `category_group` - must be equal to `liabilities`

  **Valid tax reporting names by company type**

 UK Limited Company | All other company types || `provisions_for_liabilities` |  |

#### Example request body

```json
{
  "category": {
    "description": "Custom Liabilities Category",
    "nominal_code": "732",
    "tax_reporting_name": "creditors",
    "category_group": "liabilities"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Liabilities Category</description>
  <nominal-code>732</nominal-code>
  <tax_reporting_name>creditors</tax_reporting_name>
  <category-group>liabilities</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/732",
    "description": "Custom Liabilities Category",
    "nominal_code": "732"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/733</url>
    <description>Custom Liabilities Category</description>
    <nominal-code>732</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

### Create an equity category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 921 to 960
- `category_group` - must be equal to `equities`

#### Example request body

```json
{
  "category": {
    "description": "Custom Equity Category",
    "nominal_code": "922",
    "category_group": "equities"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Equity Category</description>
  <nominal-code>922</nominal-code>
  <category-group>equities</category-group>
</category>
```
Show as JSON

#### Response

```http
Status: 201 Created
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/923",
    "description": "Custom Equity Category",
    "nominal_code": "922"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/922</url>
    <description>Custom Equity Category</description>
    <nominal-code>922</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

## Update a category

```http
PUT https://api.freeagent.com/v2/categories/:nominal_code
```

Only categories that do not contain any items can be updated.
Payload should have a root `category` element, containing attributes allowed for the specific category type.

### Update an income category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 001 to 049

#### Example request body

```json
{
  "category": {
    "description": "Custom Income Category",
    "nominal_code": "047",
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Income Category</description>
  <nominal-code>047</nominal-code>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "income_categories": {
    "url": "https://api.freeagent.com/v2/categories/047",
    "description": "Custom Income Category",
    "group_description": "Income (normally VATable)",
    "nominal_code": "047",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <income-categories>
    <url>https://api.freeagent.com/v2/categories/047</url>
    <description>Custom Income Category</description>
    <group-description>Income (normally VATable)</group-description>
    <nominal-code>047</nominal-code>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </income-categories>
</freeagent>
```
Show as JSON

### Update a cost of sales category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 096 to 199
- `tax_reporting_name` - see below for valid values by company type
- `allowable_for_tax`
- `auto_sales_tax_rate`

  **Valid tax reporting names by company type**

 UK Limited Company | UK Sole Trader || `commissions_payable` | `cost_of_goods` |
| `material_costs`                                     | `subcontractor_costs`                                |
| `purchases`                                          | `construction_industry_scheme_subcontractor_costs`\* |
| `subcontractor_costs`                                |                                                      |
| `construction_industry_scheme_subcontractor_costs`\* |                                                      |

\* These tax reporting names are only available if CIS for Contractors is switched on

 UK Partnership | Universal and US Companies || `cost_of_sales` | `cost_of_labor` |
| `subcontractor_costs` | `materials_and_supplies` |     |
|                       | `other_costs`            |     |
|                       | `purchases`              |     |

#### Example request body

```json
{
  "category": {
    "description": "Custom Cost of Sales Category",
    "nominal_code": "101",
    "tax_reporting_name": "purchases",
    "allowable_for_tax": true,
    "auto_sales_tax_rate": "Standard rate",
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Cost of Sales Category</description>
  <nominal-code>101</nominal-code>
  <tax_reporting_name>purchases</tax_reporting_name>
  <allowable-for-tax>true</allowable-for-tax>
  <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "cost_of_sales_categories": {
    "url": "https://api.freeagent.com/v2/categories/101",
    "description": "Custom Cost of Sales Category",
    "group_description": "Cost of sales (normally VATable)",
    "nominal_code": "101",
    "allowable_for_tax": true,
    "tax_reporting_name": "Purchases",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <cost-of-sales-categories>
    <url>https://api.freeagent.com/v2/categories/101</url>
    <description>Custom Cost of Sales Category</description>
    <group-description>Cost of sales (normally VATable)</group-description>
    <nominal-code>101</nominal-code>
    <allowable-for-tax type="boolean">true</allowable-for-tax>
    <tax-reporting-name>Purchases</tax-reporting-name>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </cost-of-sales-categories>
</freeagent>
```
Show as JSON

### Update an admin expenses category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 200 to 399
- `tax_reporting_name` - see below for valid values by company type
- `allowable_for_tax`
- `auto_sales_tax_rate`

  **Valid tax reporting names by company type**

| UK Limited Company                                | UK Sole Trader                         |
| ------------------------------------------------- | -------------------------------------- |
| `accountancy_fees`                                | `accountancy_and_legal_fees`           |
| `advertising_and_promotional_costs`               | `advertising_costs`                    |
| `amortisation_of_intangible_assets`               | `bank_and_loan_interest`               |
| `bad_debts_written_off`                           | `car_van_and_travel_expenses`          |
| `bank_charges`                                    | `debts_written_off`                    |
| `business_entertaining`                           | `depreciation_and_loss_profit_on_sale` |
| `canteen`                                         | `entertainment_costs`                  |
| `charitable_donations`                            | `other_business_expenses`              |
| `computer_software_costs`                         | `other_finance_charges`                |
| `consumable_items`                                | `phone_and_other_office_costs`         |
| `credit_card_charges`                             | `rent_and_other_property_costs`        |
| `depreciation_of_tangible_fixed_assets`           | `repair_and_renewal_costs`             |
| `directors_pensions`                              | `wages_salaries_and_staff_costs`       |
| `directors_remuneration`                          |                                        |
| `employers_ni_directors`                          |                                        |
| `employers_ni_staff`                              |                                        |
| `finance_charges`                                 |                                        |
| `foreign_exchange_transaction_charges`            |                                        |
| `gain_from_disposal_of_tangible_fixed_assets`     |                                        |
| `gain_on_foreign_currency_transactions`           |                                        |
| `general_consultancy_fees`                        |                                        |
| `general_maintenance`                             |                                        |
| `hire_and_leasing_of_computer_equipment`          |                                        |
| `hire_and_leasing_of_motor_vehicles`              |                                        |
| `hire_and_leasing_of_other_assets`                |                                        |
| `it_and_computer_consumables`                     |                                        |
| `insurance`                                       |                                        |
| `insurance_on_premises`                           |                                        |
| `interest_payable`                                |                                        |
| `irrecoverable_vat`                               |                                        |
| `late_payment_of_tax`                             |                                        |
| `leases_and_hire_purchase_contracts`              |                                        |
| `legal_fees`                                      |                                        |
| `management_fees`                                 |                                        |
| `other_legal_and_professional_fees`               |                                        |
| `political_donations`                             |                                        |
| `postage_costs`                                   |                                        |
| `premises_cleaning`                               |                                        |
| `premises_repairs_and_maintenance`                |                                        |
| `premises_repairs_and_renewals`                   |                                        |
| `printing_costs`                                  |                                        |
| `publication_and_other_information_subscriptions` |                                        |
| `rates_on_premises`                               |                                        |
| `rent_of_premises`                                |                                        |
| `research_and_development_costs`                  |                                        |
| `staff_benefits_in_kind`                          |                                        |
| `staff_entertaining`                              |                                        |
| `staff_pensions`                                  |                                        |
| `staff_training`                                  |                                        |
| `staff_welfare`                                   |                                        |
| `stationery`                                      |                                        |
| `subscriptions_to_professional_and_trade_bodies`  |                                        |
| `sundry_expenses`                                 |                                        |
| `telecommunication_costs`                         |                                        |
| `travel_and_subsistence`                          |                                        |
| `use_of_residence`                                |                                        |
| `vehicle_running_costs`                           |                                        |
| `wages_and_salaries`                              |                                        |

| UK Partnership                    | Universal and US Companies             |
| --------------------------------- | -------------------------------------- |
| `advertising_etc`                 | `advertising`                          |
| `bad_debts`                       | `bad_debts_written_off`                |
| `business_entertaining`           | `car_and_truck_expenses`               |
| `depreciation_and_loss`           | `commissions_and_fees`                 |
| `employee_costs`                  | `contract_labor`                       |
| `general_administrative_expenses` | `depletion`                            |
| `interest`                        | `depreciation`                         |
| `legal_and_professional_costs`    | `employee_benefit_programs`            |
| `motor_expenses`                  | `expenses_for_business_use_of_home`    |
| `other_direct_costs`              | `insurance`                            |
| `other_expenses`                  | `mortgage_interest`                    |
| `other_finance_charges`           | `other_interest`                       |
| `premises_costs`                  | `legal_and_professional_services`      |
| `repairs`                         | `meals_and_entertainment`              |
| `travel_and_subsistence`          | `office_expense`                       |
|                                   | `other_expenses`                       |
|                                   | `pension_and_profit_sharing_plans`     |
|                                   | `other_business_property_rent`         |
|                                   | `vehicle_machinery_and_equipment_rent` |
|                                   | `repairs_and_maintenance`              |
|                                   | `supplies`                             |
|                                   | `taxes_and_licenses`                   |
|                                   | `travel`                               |
|                                   | `utilities`                            |
|                                   | `wages`                                |

#### Example request body

```json
{
  "category": {
    "description": "Custom Admin Expenses Category",
    "nominal_code": "212",
    "tax_reporting_name": "computer_software_costs",
    "allowable_for_tax": true,
    "auto_sales_tax_rate": "Standard rate",
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Admin Expenses Category</description>
  <nominal-code>212</nominal-code>
  <tax_reporting_name>computer_software_costs</tax_reporting_name>
  <allowable-for-tax>true</allowable-for-tax>
  <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "admin_expenses_categories": {
    "url": "https://api.freeagent.com/v2/categories/213",
    "description": "Custom Admin Expenses Category",
    "group_description": "Admin expenses (normally VATable)",
    "nominal_code": "213",
    "allowable_for_tax": "true",
    "tax_reporting_name": "Computer software costs",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <admin-expenses-categories>
    <url>https://api.freeagent.com/v2/categories/213</url>
    <description>Custom Admin Expenses Category</description>
    <group-description>Admin expenses (normally VATable)</group-description>
    <nominal-code>213</nominal-code>
    <allowable-for-tax type="boolean">true</allowable-for-tax>
    <tax-reporting-name>Computer software costs</tax-reporting-name>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </admin-expenses-categories>
</freeagent>
```
Show as JSON

### Update a current asset category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 671 to 720
- `tax_reporting_name` - see below for valid values by company type

  **Valid tax reporting names by company type**

 All company types || `debtors` |
| `money_in_transit`               |
| `prepayments_and_accrued_income` |

#### Example request body

```json
{
  "category": {
    "description": "Custom Assets Category",
    "nominal_code": "672",
    "tax_reporting_name": "debtors"
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Assets Category</description>
  <nominal-code>673</nominal-code>
  <tax-reporting-name>debtors</tax-reporting-name>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/672",
    "description": "Custom Assets Category",
    "nominal_code": "672"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/673</url>
    <description>Custom Assets Category</description>
    <nominal-code>673</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

### Update a liabilities category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 731 to 780
- `tax_reporting_name` - see below for valid values by company type

  **Valid tax reporting names by company type**

 UK Limited Company | All other company types || `provisions_for_liabilities` |  |

#### Example request body

```json
{
  "category": {
    "description": "Custom Liabilities Category",
    "nominal_code": "732",
    "tax_reporting_name": "creditors",
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Liabilities Category</description>
  <nominal-code>732</nominal-code>
  <tax_reporting_name>creditors</tax_reporting_name>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/732",
    "description": "Custom Liabilities Category",
    "nominal_code": "732"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/733</url>
    <description>Custom Liabilities Category</description>
    <nominal-code>732</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

### Update an equity category

Allowed attributes:

- `description`
- `nominal_code` - this can be any unique value from 921 to 960

#### Example request body

```json
{
  "category": {
    "description": "Custom Equity Category",
    "nominal_code": "922",
  }
}
```
Show as XML

```xml
<category>
  <description>Custom Equity Category</description>
  <nominal-code>922</nominal-code>
</category>
```
Show as JSON

#### Response

```http
Status: 200 OK
```

```json
{
  "general_categories": {
    "url": "https://api.freeagent.com/v2/categories/923",
    "description": "Custom Equity Category",
    "nominal_code": "922"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <general-categories>
    <url>https://api.freeagent.com/v2/categories/922</url>
    <description>Custom Equity Category</description>
    <nominal-code>922</nominal-code>
  </general-categories>
</freeagent>
```
Show as JSON

## Delete a category

Only user-created categories that do not contain any items can be deleted.

```http
DELETE https://api.freeagent.com/v2/categories/:nominal_code
```

### Response

```http
Status: 200 OK
```

```json
{
  "admin_expenses_categories": {
    "url": "https://api.freeagent.com/v2/categories/200",
    "description": "Custom Admin Expenses Category",
    "group_description": "Admin expenses (normally VATable)",
    "nominal_code": "200",
    "allowable_for_tax": true,
    "tax_reporting_name": "Accountancy fees",
    "auto_sales_tax_rate": "Standard rate"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <admin-expenses-categories>
    <url>https://api.freeagent.com/v2/categories/200</url>
    <description>Custom Admin Expenses Category</description>
    <group-description>Admin expenses (normally VATable)</group-description>
    <nominal-code>200</nominal-code>
    <allowable-for-tax type="boolean">true</allowable-for-tax>
    <tax-reporting-name>Accountancy fees</tax-reporting-name>
    <auto-sales-tax-rate>Standard rate</auto-sales-tax-rate>
  </admin-expenses-categories>
</freeagent>
```
Show as JSON