# Tasks

*Minimum access level*: `Time`, unless stated otherwise.

## Attributes

| Required | Attribute      | Description                                                                  | Kind      |
| -------- | -------------- | ---------------------------------------------------------------------------- | --------- |
|          | url            | The unique identifier for the task                                           | URI       |
| ✔        | name           | Task name                                                                    | String    |
|          | currency       | [Currency](currencies.md) code of the project (e.g. `USD`, `GBP`, `EUR`, …)  | String    |
|          | is_billable    | `true` if charging your clients for the task, `false` otherwise              | Boolean   |
|          | status         | One of the following: `Active`, `Completed`, `Hidden`                        | String    |
|          | created_at     | Creation of the task resource (UTC)                                          | Timestamp |
|          | updated_at     | When the task resource was last updated (UTC)                                | Timestamp |
|          | is_deletable   | `true` if this task can be deleted, `false` otherwise                        | Boolean   |
|          | billing_rate   | The rate at which the [project](projects.md) is billed, per `billing_period` | Decimal   |
|          | billing_period | One of the following: `day`, `hour`                                          | String    |

## List all tasks

```http
GET https://api.freeagent.com/v2/tasks
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/tasks?view=active
```

- `all`: Show all tasks (default)
- `active`: Show only tasks with status active.
- `completed`: Show only tasks with status completed.
- `hidden`: Show only tasks with status hidden.

#### Date Filters

```http
GET https://api.freeagent.com/v2/tasks?updated_since=2017-04-06
```

- `updated_since`

#### Sort Orders

```http
GET https://api.freeagent.com/v2/tasks?sort=updated_at
```

- `name`: Sort by the task name (default).
- `project`: Sort by the project_id associated with the task.
- `billing_rate`: Sort by the billing rate.
- `created_at`: Sort by the time the task was created.
- `updated_at`: Sort by the time the task was last modified.

### Response

```http
Status: 200 OK
```

```json
{ "tasks":[
  {
    "url":"https://api.freeagent.com/v2/tasks/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "name":"Sample Task",
    "currency":"GBP",
    "is_billable":true,
    "billing_rate":"0.0",
    "billing_period":"hour",
    "status":"Active",
    "created_at":"2011-08-16T11:06:57Z",
    "updated_at":"2011-08-16T11:06:57Z",
    "is_deletable": false
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <tasks type="array">
    <task>
      <url>https://api.freeagent.com/v2/tasks/1</url>
      <project>https://api.freeagent.com/v2/projects/1</project>
      <name>Sample Task</name>
      <currency>GBP</currency>
      <is-billable type="boolean">true</is-billable>
      <billing-rate type="decimal">0.0</billing-rate>
      <billing-period>hour</billing-period>
      <status>Active</status>
      <created-at type="datetime">2011-08-16T11:06:57Z</created-at>
      <updated-at type="datetime">2011-08-16T11:06:57Z</updated-at>
      <is-deletable type="boolean">false</is-deletable>
    </task>
  </tasks>
</freeagent>
```
Show as JSON

## List all tasks under a certain project

```http
GET https://api.freeagent.com/v2/tasks?project=https://api.freeagent.com/v2/projects/2
```

## Get a single task

```http
GET https://api.freeagent.com/v2/tasks/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "task":
  {
    "project":"https://api.freeagent.com/v2/projects/1",
    "name":"Sample Task",
    "currency":"GBP",
    "is_billable":true,
    "billing_rate":"0.0",
    "billing_period":"hour",
    "status":"Active",
    "created_at":"2011-08-16T11:06:57Z",
    "updated_at":"2011-08-16T11:06:57Z",
    "is_deletable": false
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <task>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <name>Sample Task</name>
    <currency>GBP</currency>
    <is-billable type="boolean">true</is-billable>
    <billing-rate type="decimal">0.0</billing-rate>
    <billing-period>hour</billing-period>
    <status>Active</status>
    <created-at type="datetime">2011-08-16T11:06:57Z</created-at>
    <updated-at type="datetime">2011-08-16T11:06:57Z</updated-at>
    <is-deletable type="boolean">false</is-deletable>
  </task>
</freeagent>
```
Show as JSON

## Create a task under a certain project

```http
POST https://api.freeagent.com/v2/tasks?project=:project
```

Payload should have a root `task` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/tasks/2
```

```json
{ "task":
  {
    "project":"https://api.freeagent.com/v2/projects/1",
    "name":"Sample Task",
    "currency":"GBP",
    "is_billable":true,
    "billing_rate":"0.0",
    "billing_period":"hour",
    "status":"Active",
    "created_at":"2011-08-16T11:06:57Z",
    "updated_at":"2011-08-16T11:06:57Z",
    "is_deletable": true
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <task>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <name>Sample Task</name>
    <currency>GBP</currency>
    <is-billable type="boolean">true</is-billable>
    <billing-rate type="decimal">0.0</billing-rate>
    <billing-period>hour</billing-period>
    <status>Active</status>
    <created-at type="datetime">2011-08-16T11:06:57Z</created-at>
    <updated-at type="datetime">2011-08-16T11:06:57Z</updated-at>
    <is-deletable type="boolean">true</is-deletable>
  </task>
</freeagent>
```
Show as JSON

## Update a task

```http
PUT https://api.freeagent.com/v2/tasks/:id
```

Payload should have a root `task` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a task

```http
DELETE https://api.freeagent.com/v2/users/:id
```

### Response

```http
Status: 200 OK
```