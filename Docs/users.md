# Users

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

## Permissions

Guide to the permission_level attribute:

```json
0 : No Access
1 : Time
2 : My Money
3 : Contacts & Projects
4 : Invoices, Estimates & Files
5 : Bills
6 : Banking
7 : Tax, Accounting & Users
8 : Full
```

## Attributes

| Required | Attribute               | Description                                                                                                                                                                                                                                                                           | Kind      |
| -------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- |
|          | url                     | The unique identifier for the user                                                                                                                                                                                                                                                    | URI       |
| ✔        | email                   | Login email address                                                                                                                                                                                                                                                                   | String    |
| ✔        | first_name              | First name                                                                                                                                                                                                                                                                            | String    |
| ✔        | last_name               | Last name                                                                                                                                                                                                                                                                             | String    |
|          | ni_number               | UK National Insurance Number                                                                                                                                                                                                                                                          | String    |
|          | unique_tax_reference    | 10-digit UK Tax Reference                                                                                                                                                                                                                                                             | String    |
| ✔        | role                    | One of the following: `Owner`, `Director`, `Partner`, `Company Secretary`, `Employee`, `Shareholder`, `Accountant` Certain roles can not be set, depending on the type of company - e.g. Sole trader businesses don't have Directors, Partners, Company Secretaries, or Shareholders. | String    |
| ✔        | opening_mileage         | Opening mileage as of [company](company.md) start date                                                                                                                                                                                                                                | Decimal   |
|          | send_invitation         | `true` to send the user an invitation to set their password, `false` otherwise                                                                                                                                                                                                        | Boolean   |
|          | permission_level        | See [Permissions](#permissions) above                                                                                                                                                                                                                                                 | Integer   |
|          | created_at              | Creation of the user resource (UTC)                                                                                                                                                                                                                                                   | Timestamp |
|          | updated_at              | When the user resource was last updated (UTC)                                                                                                                                                                                                                                         | Timestamp |
|          | current_payroll_profile | A subsection containing payroll information for the current tax year                                                                                                                                                                                                                  | Object    |

## Read-only current payroll profile attributes

| Required | Attribute                        | Description                               | Kind    |
| -------- | -------------------------------- | ----------------------------------------- | ------- |
|          | total_pay_in_previous_employment | Total pay during previous employment      | Decimal |
|          | total_tax_in_previous_employment | Total tax paid during previous employment | Decimal |

## List all users

```http
GET https://api.freeagent.com/v2/users
```

### Input

#### View Filters

```http
GET https://api.freeagent.com/v2/users?view=all
```

- `all`: Show all users (default)
- `staff`: Show users with a role as an Owner, Director, Partner, Company Secretary, Employee or Shareholder.
- `active_staff`: Show non-hidden users with a role as an Owner, Director, Partner, Company Secretary, Employee or Shareholder.
- `advisors`: Show users with an Accountant role.
- `active_advisors`: Show non-hidden users with an Accountant role.

### Response

```http
Status: 200 OK
```

```json
{ "users":[
  {
    "url":"https://api.freeagent.com/v2/users/1",
    "first_name":"Development",
    "last_name":"Team",
    "email":"dev@example.com",
    "role":"Director",
    "permission_level":8,
    "ni_number":"QQ123456C",
    "unique_tax_reference":"1234567890",
    "opening_mileage":0,
    "updated_at":"2011-08-24T08:10:23Z",
    "created_at":"2011-07-28T11:25:11Z"
  }
]}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <users type="array">
    <user>
      <url>https://api.freeagent.com/v2/users/1</url>
      <first-name>Development</first-name>
      <last-name>Team</last-name>
      <email>dev@example.com</email>
      <role>Director</role>
      <permission-level type="integer">8</permission-level>
      <ni-number>QQ123456C</ni-number>
      <unique-tax-reference>1234567890</unique-tax-reference>
      <opening-mileage type="integer">0</opening-mileage>
      <updated-at type="datetime">2011-08-24T08:10:23Z</updated-at>
      <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
    </user>
  </users>
</freeagent>
```
Show as JSON

## Get a single user

```http
GET https://api.freeagent.com/v2/users/:id
```

### Response

```http
Status: 200 OK
```

```json
{ "user":
  {
    "url":"https://api.freeagent.com/v2/users/1",
    "first_name":"Development",
    "last_name":"Team",
    "email":"dev@example.com",
    "role":"Director",
    "permission_level":8,
    "ni_number":"QQ123456C",
    "unique_tax_reference":"1234567890",
    "opening_mileage":0,
    "updated_at":"2011-08-24T08:10:23Z",
    "created_at":"2011-07-28T11:25:11Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <user>
    <url>https://api.freeagent.com/v2/users/1</url>
    <first-name>Development</first-name>
    <last-name>Team</last-name>
    <email>dev@example.com</email>
    <role>Director</role>
    <permission-level type="integer">8</permission-level>
    <ni-number>QQ123456C</ni-number>
    <unique-tax-reference>1234567890</unique-tax-reference>
    <opening-mileage type="integer">0</opening-mileage>
    <updated-at type="datetime">2011-08-24T08:10:23Z</updated-at>
    <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
  </user>
</freeagent>
```
Show as JSON

## Get personal profile

```http
GET https://api.freeagent.com/v2/users/me
```

This will return the details for the currently active user

```json
{ "user":
  {
    "url":"https://api.freeagent.com/v2/users/1",
    "first_name":"My",
    "last_name":"User",
    "email":"me@example.com",
    "role":"Director",
    "permission_level":8,
    "ni_number":"QQ123456C",
    "unique_tax_reference":"1234567890",
    "opening_mileage":0,
    "updated_at":"2011-08-24T08:10:23Z",
    "created_at":"2011-07-28T11:25:11Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <user>
    <url>https://api.freeagent.com/v2/users/1</url>
    <first-name>My</first-name>
    <last-name>User</last-name>
    <email>me@example.com</email>
    <role>Director</role>
    <permission-level type="integer">8</permission-level>
    <ni-number>QQ123456C</ni-number>
    <unique-tax-reference>1234567890</unique-tax-reference>
    <opening-mileage type="integer">0</opening-mileage>
    <updated-at type="datetime">2011-08-24T08:10:23Z</updated-at>
    <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
  </user>
</freeagent>
```
Show as JSON

### Response

```http
Status: 200 OK
```

*Minimum access level*: `Time`

## Create a user

```http
POST https://api.freeagent.com/v2/users
```

Payload should have a root `user` element, containing elements listed
under Attributes.

### Response

```http
Status: 201 Created
Location: https://api.freeagent.com/v2/users/74
```

```json
{ "user":
  {
    "url":"https://api.freeagent.com/v2/users/1",
    "first_name":"Development",
    "last_name":"Team",
    "email":"dev@example.com",
    "role":"Director",
    "permission_level":8,
    "ni_number":"QQ123456C",
    "unique_tax_reference":"1234567890",
    "opening_mileage":0,
    "updated_at":"2011-08-24T08:10:23Z",
    "created_at":"2011-07-28T11:25:11Z"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <user>
    <url>https://api.freeagent.com/v2/users/1</url>
    <first-name>Development</first-name>
    <last-name>Team</last-name>
    <email>dev@example.com</email>
    <role>Director</role>
    <permission-level type="integer">8</permission-level>
    <ni-number>QQ123456C</ni-number>
    <unique-tax-reference>1234567890</unique-tax-reference>
    <opening-mileage type="integer">0</opening-mileage>
    <updated-at type="datetime">2011-08-24T08:10:23Z</updated-at>
    <created-at type="datetime">2011-07-28T11:25:11Z</created-at>
  </user>
</freeagent>
```
Show as JSON

## Update a user

```http
PUT https://api.freeagent.com/v2/users/:id
```

### Update personal profile

*Minimum access level*: `Time`

```http
PUT https://api.freeagent.com/v2/users/me
```

Payload should have a root `user` element, containing elements listed
under Attributes that should be updated.

### Response

```http
Status: 200 OK
```

## Delete a user

```http
DELETE https://api.freeagent.com/v2/users/:id
```

### Response

```http
Status: 200 OK
```