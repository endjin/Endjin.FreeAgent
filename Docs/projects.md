# Projects

*Minimum access level*: `Contacts and Projects`, unless stated otherwise.

## Attributes

| Required | Attribute                              | Description                                                                                                                          | Kind      |
| -------- | -------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ | --------- |
|          | url                                    | The unique identifier for the project                                                                                                | URI       |
| ✔        | contact                                | [Contact](contacts.md) to bill for the project                                                                                       | URI       |
| ✔        | name                                   | Free-text project name                                                                                                               | String    |
| ✔        | status                                 | One of the following: `Active`, `Completed`, `Cancelled`, `Hidden`                                                                   | String    |
|          | contract_po_reference                  | Contract / Purchase order reference                                                                                                  | String    |
|          | uses_project_invoice_sequence          | `true` if [invoice](invoices.md) sequence is project-level, `false` otherwise                                                        | Boolean   |
| ✔        | currency                               | [Currency](currencies.md) code of the project (e.g. `USD`, `GBP`, `EUR`, …)                                                          | String    |
|          | budget                                 | Leave as zero if this project doesn't have a budget                                                                                  | Decimal   |
| ✔        | budget_units                           | One of the following: `Hours`, `Days`, `Monetary` (ex-VAT)                                                                           | String    |
|          | hours_per_day                          | Hours per day (e.g. 1:30 hours → use `1.5`)                                                                                          | Decimal   |
|          | normal_billing_rate                    | Normal billing rate                                                                                                                  | Decimal   |
|          | billing_period                         | Unit for the `normal_billing_rate`: `hour`, `day`                                                                                    | String    |
|          | is_ir35                                | `true` if this project comes under IR35 as de facto employment, `false` otherwise                                                    | Boolean   |
|          | starts_on                              | Start date of the project in `YYYY-MM-DD` format Leave blank if this project doesn't have a starting date                            | Date      |
|          | ends_on                                | End date of the proejct in `YYYY-MM-DD` format Leave blank if this project doesn't have an ending date                               | Date      |
|          | include_unbilled_time_in_profitability | `true` if this project includes unbilled time in the profitiability report, `false` otherwise                                        | Boolean   |
|          | is_deletable                           | `true` if this project can be deleted, `false` otherwise. Only returned in the [single project GET](#get-a-single-project) response. | Boolean   |
|          | created_at                             | Creation of the project resource (UTC)                                                                                               | Timestamp |
|          | updated_at                             | When the project resource was last updated (UTC)                                                                                     | Timestamp |

## List all projects

```http
GET https://api.freeagent.com/v2/projects
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/projects?view=active
```

- `active`: Show only active projects.
- `completed`: Show only completed projects.
- `cancelled`: Show only cancelled projects.
- `hidden`: Show only hidden projects.

#### Sort Orders

```http
GET https://api.freeagent.com/v2/projects?sort=updated_at
```

- `name`: Sort by project name (default).
- `contact_name`: Sort by the concatenation of `organisation_name`, `last_name` and `first_name` of the contact.
- `contact_display_name`: Sort by the concatenation of `organisation_name`, `first_name` and `last_name` of the contact.
- `created_at`: Sort by the time the project was created.
- `updated_at`: Sort by the time the project was last modified.

To sort in descending order, the sort parameter can be prefixed with a hyphen.

```http
GET https://api.freeagent.com/v2/projects?sort=-updated_at
```

#### Nested Response

```http
GET https://api.freeagent.com/v2/projects?nested=true
```

- `true`: Return full contact details for each project as a nested json object.  See [Contact](contacts.md)
- `false`: Return a RESTful reference to the contact for each project and the contact's display name

### Response

```http
Status: 200 OK
```

```json
{ "projects":[
  {
    "url":"https://api.freeagent.com/v2/projects/1",
    "name":"Test Project",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "contact_name":"Acme Trading",
    "budget":0,
    "is_ir35":false,
    "status":"Active",
    "budget_units":"Hours",
    "normal_billing_rate":"0.0",
    "hours_per_day":"8.0",
    "uses_project_invoice_sequence":false,
    "currency":"GBP",
    "billing_period":"hour",
    "include_unbilled_time_in_profitability":true,
    "created_at":"2011-09-14T16:05:57Z",
    "updated_at":"2011-09-14T16:05:57Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <projects type="array">
    <project>
      <url>https://api.freeagent.com/v2/projects/1</url>
      <name>Test Project</name>
      <contact>https://api.freeagent.com/v2/contacts/1</contact>
      <contact_name>Acme Trading</contact_name>,
      <budget type="integer">0</budget>
      <is-ir35 type="boolean">false</is-ir35>
      <status>Active</status>
      <budget-units>Hours</budget-units>
      <normal-billing-rate type="decimal">0.0</normal-billing-rate>
      <hours-per-day type="decimal">8.0</hours-per-day>
      <uses-project-invoice-sequence type="boolean">false</uses-project-invoice-sequence>
      <currency>GBP</currency>
      <billing-period>hour</billing-period>
      <include_unbilled_time_in_profitability>true</include_unbilled_time_in_profitability>,
      <created-at type="datetime">2011-09-14T16:05:57Z</created-at>
      <updated-at type="datetime">2011-09-14T16:05:57Z</updated-at>
    </project>
  </projects>
</freeagent>
```
Show as JSON

## Get a single project

```http
GET https://api.freeagent.com/v2/projects/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "project":
  {
    "name":"Test Project",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "contact_name":"Acme Trading",
    "budget":0,
    "is_ir35":false,
    "status":"Active",
    "budget_units":"Hours",
    "normal_billing_rate":"0.0",
    "hours_per_day":"8.0",
    "uses_project_invoice_sequence":false,
    "currency":"GBP",
    "billing_period":"hour",
    "include_unbilled_time_in_profitability":true,
    "is_deletable": false,
    "created_at":"2011-09-14T16:05:57Z",
    "updated_at":"2011-09-14T16:05:57Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <project>
    <name>Test Project</name>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <contact_name>Acme Trading</contact_name>,
    <budget type="integer">0</budget>
    <is-ir35 type="boolean">false</is-ir35>
    <status>Active</status>
    <budget-units>Hours</budget-units>
    <normal-billing-rate type="decimal">0.0</normal-billing-rate>
    <hours-per-day type="decimal">8.0</hours-per-day>
    <uses-project-invoice-sequence type="boolean">false</uses-project-invoice-sequence>
    <currency>GBP</currency>
    <billing-period>hour</billing-period>
    <include_unbilled_time_in_profitability>true</include_unbilled_time_in_profitability>,
    <is-deletable>false</is-deletable>
    <created-at type="datetime">2011-09-14T16:05:57Z</created-at>
    <updated-at type="datetime">2011-09-14T16:05:57Z</updated-at>
  </project>
</freeagent>
```
Show as JSON

## List all projects related to a contact

```http
GET https://api.freeagent.com/v2/projects?contact=https://api.freeagent.com/v2/contacts/2
```

## Create a project

```http
POST https://api.freeagent.com/v2/projects
```

Payload should have a root `project` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/projects/2
```

```json
{ "project":
  {
    "name":"Test Project",
    "contact":"https://api.freeagent.com/v2/contacts/1",
    "contact_name":"Acme Trading",
    "budget":0,
    "is_ir35":false,
    "status":"Active",
    "budget_units":"Hours",
    "normal_billing_rate":"0.0",
    "hours_per_day":"8.0",
    "uses_project_invoice_sequence":false,
    "currency":"GBP",
    "billing_period":"hour",
    "include_unbilled_time_in_profitability":true,
    "created_at":"2011-09-14T16:05:57Z",
    "updated_at":"2011-09-14T16:05:57Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <project>
    <name>Test Project</name>
    <contact>https://api.freeagent.com/v2/contacts/1</contact>
    <contact_name>Acme Trading</contact_name>,
    <budget type="integer">0</budget>
    <is-ir35 type="boolean">false</is-ir35>
    <status>Active</status>
    <budget-units>Hours</budget-units>
    <normal-billing-rate type="decimal">0.0</normal-billing-rate>
    <hours-per-day type="decimal">8.0</hours-per-day>
    <uses-project-invoice-sequence type="boolean">false</uses-project-invoice-sequence>
    <currency>GBP</currency>
    <billing-period>hour</billing-period>
    <include_unbilled_time_in_profitability>true</include_unbilled_time_in_profitability>,
    <created-at type="datetime">2011-09-14T16:05:57Z</created-at>
    <updated-at type="datetime">2011-09-14T16:05:57Z</updated-at>
  </project>
</freeagent>
```
Show as JSON

## Update a project

```http
PUT https://api.freeagent.com/v2/projects/:id
```

Payload should have a root `project` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a project

```http
DELETE https://api.freeagent.com/v2/projects/:id
```

### Response

```http
Status: 200 OK
```