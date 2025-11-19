# Timeslips

*Minimum access level*: `Time`, unless stated otherwise.

## Timeslip Attributes

| Required | Attribute                                 | Description                                                | Kind    |
| -------- | ----------------------------------------- | ---------------------------------------------------------- | ------- |
|          | url                                       | The unique identifier for the timeslip                     | URI     |
| ✔        | task                                      | [Task](tasks.md) that was completed                        | URI     |
| ✔        | user                                      | [User](users.md) that completed the `task`                 | URI     |
| ✔        | project                                   | [Project](projects.md) for which the `task` was completed  | URI     |
| ✔        | dated_on                                  | Date of the timeslip, in `YYYY-MM-DD` format               | Date    |
| ✔        | hours                                     | Number of hours worked For e.g. 1:30 hours, use 1.5        | Decimal |
|          | comment                                   | Free-text comment                                          | String  |
|          | billed_on_invoice                         | [Invoice](invoices.md) billing the timeslip, if any exists | URI     |
|          | created_at                                | Creation of the timeslip resource (UTC)                    | Date    |
|          | updated_at                                | When the timeslip resource was last updated (UTC)          | Date    |
| timer    | See [Timer Attributes](#timer-attributes) | Object                                                     |         |

#### Nested Response

For endpoints that return timeslips in the response, the following URL parameter can be used for to retrieve full
details of resources associated with the timeslip(s) - such as [user](users.md),
[project](projects.md), [task](tasks.md) - as nested JSON objects rather than URL
references.

- `nested`
    - `true`: Return associated resources as nested JSON objects
    - `false`: Return a URL reference to each associated resource

For example:

```http
GET https://api.freeagent.com/v2/timeslips?nested=true
```

## Timer Attributes

| Attribute  | Description                                                                                                                                             | Kind     |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- |
| running    | Will always be `true` as this is not exposed unless a timer is running                                                                                  | Boolean  |
| start_from | Effective start date of the timer. This will include any time already recorded, so to get the current elapsed time subtract this from the current time. | Datetime |

## List all timeslips

```http
GET https://api.freeagent.com/v2/timeslips
```

#### Date Filters

```http
GET https://api.freeagent.com/v2/timeslips?from_date=2012-01-01&to_date=2012-03-31&view=all
```

```http
GET https://api.freeagent.com/v2/timeslips?updated_since=2017-05-22T09:00:00.000Z
```

- `from_date`
- `to_date`
- `updated_since`

#### Other Filters

- `view`
    - `all` - return all timeslips (default when omitted)
    - `unbilled` - return only timeslips which have not yet been rebilled to a project.
    - `running` - return show only timeslips which have running timers.

### Response

```http
Status: 200 OK
```

```json
{ "timeslips":[
  {
    "url":"https://api.freeagent.com/v2/timeslips/25",
    "user":"https://api.freeagent.com/v2/users/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "task":"https://api.freeagent.com/v2/tasks/1",
    "dated_on":"2011-08-15",
    "hours":"12.0",
    "timer": {
      "running": true,
      "start_from": "2011-08-16T01:32:00Z"
    },
    "updated_at":"2011-08-16T13:32:00Z",
    "created_at":"2011-08-16T13:32:00Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <timeslips type="array">
    <timeslip>
      <url>https://api.freeagent.com/v2/timeslips/25</url>
      <user>https://api.freeagent.com/v2/users/1</user>
      <project>https://api.freeagent.com/v2/projects/1</project>
      <task>https://api.freeagent.com/v2/tasks/1</task>
      <dated-on type="date">2011-08-15</dated-on>
      <hours type="decimal">12.0</hours>
      <timer>
        <running type="boolean">true</running>
        <start-from type="datetime">2011-08-16T13:32:00Z</start-from>
      </timer>
      <updated-at type="datetime">2011-08-16T13:32:00Z</updated-at>
      <created-at type="datetime">2011-08-16T13:32:00Z</created-at>
    </timeslip>
  </timeslips>
</freeagent>
```
Show as JSON

## Get a single timeslip

```http
GET https://api.freeagent.com/v2/timeslips/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "timeslip":
  {
    "url":"https://api.freeagent.com/v2/timeslips/25",
    "user":"https://api.freeagent.com/v2/users/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "task":"https://api.freeagent.com/v2/tasks/1",
    "billed_on_invoice" : "https://api.freeagent.com/v2/invoices/7",
    "dated_on":"2011-08-15",
    "hours":"12.0",
    "updated_at":"2011-08-16T13:32:00Z",
    "created_at":"2011-08-16T13:32:00Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <timeslip>
    <url>https://api.freeagent.com/v2/timeslips/25</url>
    <user>https://api.freeagent.com/v2/users/1</user>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <task>https://api.freeagent.com/v2/tasks/1</task>
    <billed-on-invoice>https://api.freeagent.com/v2/invoices/7</billed-on-invoice>
    <dated-on type="date">2011-08-15</dated-on>
    <hours type="decimal">12.0</hours>
    <updated-at type="datetime">2011-08-16T13:32:00Z</updated-at>
    <created-at type="datetime">2011-08-16T13:32:00Z</created-at>
  </timeslip>
</freeagent>
```
Show as JSON

## List all timeslips related to a user

```http
GET https://api.freeagent.com/v2/timeslips?user=https://api.freeagent.com/v2/users/2
```

## List all timeslips related to a task

```http
GET https://api.freeagent.com/v2/timeslips?task=https://api.freeagent.com/v2/tasks/2
```

## List all timeslips related to a project

```http
GET https://api.freeagent.com/v2/timeslips?project=https://api.freeagent.com/v2/projects/2
```

## Create a timeslip

```http
POST https://api.freeagent.com/v2/timeslips
```

Payload should have a root `timeslip` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
```

```json
{ "timeslip":
  {
    "url":"https://api.freeagent.com/v2/timeslips/25",
    "user":"https://api.freeagent.com/v2/users/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "task":"https://api.freeagent.com/v2/tasks/1",
    "dated_on":"2011-08-15",
    "hours":"12.0",
    "updated_at":"2011-08-16T13:32:00Z",
    "created_at":"2011-08-16T13:32:00Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <timeslip>
    <url>https://api.freeagent.com/v2/timeslips/25</url>
    <user>https://api.freeagent.com/v2/users/1</user>
    <project>https://api.freeagent.com/v2/projects/1</project>
    <task>https://api.freeagent.com/v2/tasks/1</task>
    <dated-on type="date">2011-08-15</dated-on>
    <hours type="decimal">12.0</hours>
    <updated-at type="datetime">2011-08-16T13:32:00Z</updated-at>
    <created-at type="datetime">2011-08-16T13:32:00Z</created-at>
  </timeslip>
</freeagent>
```
Show as JSON

### Batch create

You can also post an array of timeslips inside a `timeslips` tag:

```json
{ "timeslips":
  [{
   "user":"https://api.freeagent.com/v2/users/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "task":"https://api.freeagent.com/v2/tasks/1",
    "dated_on":"2011-08-15",
    "hours":"12.0",
    "updated_at":"2011-08-16T13:32:00Z",
    "created_at":"2011-08-16T13:32:00Z"
  },
  {
    "user":"https://api.freeagent.com/v2/users/1",
    "project":"https://api.freeagent.com/v2/projects/1",
    "task":"https://api.freeagent.com/v2/tasks/1",
    "dated_on":"2011-08-14",
    "hours":"12.0",
    "updated_at":"2011-08-16T13:32:00Z",
    "created_at":"2011-08-16T13:32:00Z"
  }]
}
```
Show as XML

## Update a timeslip

```http
PUT https://api.freeagent.com/v2/timeslips/:id
```

Payload should have a root `timeslip` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a timeslip

```http
DELETE https://api.freeagent.com/v2/timeslips/:id
```

### Response

```http
Status: 200 OK
```

## Timers

When starting and stopping running timers, the timeslip data will be returned in the response.

### Start a Timer

```http
POST https://api.freeagent.com/v2/timeslips/:id/timer
```

#### Response

```http
Status: 200 OK
```

### Stop a Timer

```http
DELETE https://api.freeagent.com/v2/timeslips/:id/timer
```

#### Response

```http
Status: 200 OK
```