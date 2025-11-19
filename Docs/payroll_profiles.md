# Payroll Profiles

*Minimum access level*: `Tax and Limited Accounting`. Only available for UK companies (i.e. those which support payroll in FreeAgent).

Provides read-only access to payroll profiles set up in FreeAgent.

## Attributes

| Attribute                        | Description                                                                                                                     | Kind     |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- | -------- |
| user                             | The unique identifier for the user being paid                                                                                   | URL      |
| address_line_1                   | The first line of the address for the user being paid                                                                           | String   |
| address_line_2                   | The second line of the address for the user being paid                                                                          | String   |
| address_line_3                   | The third line of the address for the user being paid                                                                           | String   |
| address_line_4                   | The fourth line of the address for the user being paid                                                                          | String   |
| postcode                         | The postcode for the user being paid - only users in the UK will have a postcode                                                | String   |
| country                          | The country for the user being paid - only users outside the UK, Isle of Man or Channel Islands will have a country listed here | String   |
| title                            | The title or honorific of the user being paid                                                                                   | String   |
| gender                           | The current gender of the user being paid. The options are defined by HMRC.                                                     | String   |
| date_of_birth                    | The date of birth of the user being paid.                                                                                       | Date     |
| total_pay_in_previous_employment | The total amount the user was paid in previous employment during the tax year                                                   | Decimal  |
| total_tax_in_previous_employment | The total amount the user was taxed in previous employment during the tax year                                                  | Decimal  |
| employment_starts_on             | Date the employee started. Only present if the employee started during the tax year.                                            | Date     |
| created_at                       | Date and time at which the payroll profile was created                                                                          | Datetime |
| updated_at                       | Date and time at which the payroll profile was last edited                                                                      | Datetime |

## List all profiles for a given tax year

Returns the list of all payroll profiles for the given year end. For example, for the payroll year April 2020 - March 2021, the `:year` parameter should be 2021.

```http
GET https://api.freeagent.com/v2/payroll_profiles/:year
```

### Response

```http
Status: 200 OK
```

```json
{
    "profiles": [
        {
            "user": "https://api.freeagent.com/v2/users/107",
            "address_line_1": "133 Fountainbridge",
            "address_line_2": "Tollcross",
            "address_line_3": "Edinburgh",
            "address_line_4": "City of Edinburgh",
            "postcode": "EH3 9QJ",
            "title": "Dr",
            "gender": "Female",
            "date_of_birth": "1990-08-17",
            "total_pay_in_previous_employment": "1000.0",
            "total_tax_in_previous_employment": "200.0",
            "created_at": "2024-08-07T14:27:32.000Z",
            "updated_at": "2024-08-07T14:27:32.000Z"
        },
        {
            "user": "https://api.freeagent.com/v2/users/96",
            "address_line_1": "31 Rue du Soleil",
            "address_line_2": "Paris",
            "country": "France",
            "title": "Mx",
            "gender": "Male",
            "date_of_birth": "2001-07-14",
            "total_pay_in_previous_employment": "0.0",
            "total_tax_in_previous_employment": "0.0",
            "employment_starts_on": "2024-06-29",
            "created_at": "2024-06-07T15:13:24.000Z",
            "updated_at": "2024-07-11T17:31:04.000Z"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <profiles type="array">
    <profile>
      <user>https://api.freeagent.com/v2/users/107</user>
      <address-line-1>133 Fountainbridge</address-line-1>
      <address-line-2>Tollcross</address-line-2>
      <address-line-3>Edinburgh</address-line-3>
      <address-line-4>City of Edinburgh</address-line-4>
      <postcode>EH3 9QJ</postcode>
      <title>Dr</title>
      <gender>Female</gender>
      <date-of-birth type="date">1990-08-17</date-of-birth>
      <total-pay-in-previous-employment type="decimal">1000.0</total-pay-in-previous-employment>
      <total-tax-in-previous-employment type="decimal">200.0</total-tax-in-previous-employment>
      <created-at type="dateTime">2024-08-07T14:27:32Z</created-at>
      <updated-at type="dateTime">2024-08-07T14:27:32Z</updated-at>
    </profile>
    <profile>
      <user>https://api.freeagent.com/v2/users/96</user>
      <address-line-1>31 Rue du Soleil</address-line-1>
      <address-line-2>Paris</address-line-2>
      <country>France</country>
      <title>Mx</title>
      <gender>Male</gender>
      <date-of-birth type="date">2001-07-14</date-of-birth>
      <total-pay-in-previous-employment type="decimal">0.0</total-pay-in-previous-employment>
      <total-tax-in-previous-employment type="decimal">0.0</total-tax-in-previous-employment>
      <created-at type="dateTime">2024-06-07T15:13:24Z</created-at>
      <updated-at type="dateTime">2024-07-11T17:31:04Z</updated-at>
    </profile>
  </profiles>
</freeagent>
```
Show as JSON

## Payroll profile for a particular user

Returns the payroll profile in the given tax year for a particular user.

```http
GET https://api.freeagent.com/v2/payroll_profiles/:year?user=https://api.freeagent.com/v2/users/107
```

### Response

```http
Status: 200 OK
```

```json

{
    "profiles": [
        {
            "user": "https://api.freeagent.com/v2/users/107",
            "address_line_1": "133 Fountainbridge",
            "address_line_2": "Tollcross",
            "address_line_3": "Edinburgh",
            "address_line_4": "City of Edinburgh",
            "postcode": "EH3 9QJ",
            "title": "Dr",
            "gender": "Female",
            "date_of_birth": "1990-08-17",
            "total_pay_in_previous_employment": "1000.0",
            "total_tax_in_previous_employment": "200.0",
            "created_at": "2024-08-07T14:27:32.000Z",
            "updated_at": "2024-08-07T14:27:32.000Z"
        }
    ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <profiles type="array">
    <profile>
      <user>https://api.freeagent.com/v2/users/107</user>
      <address-line-1>133 Fountainbridge</address-line-1>
      <address-line-2>Tollcross</address-line-2>
      <address-line-3>Edinburgh</address-line-3>
      <address-line-4>City of Edinburgh</address-line-4>
      <postcode>EH3 9QJ</postcode>
      <title>Dr</title>
      <gender>Female</gender>
      <date-of-birth type="date">1990-08-17</date-of-birth>
      <total-pay-in-previous-employment type="decimal">1000.0</total-pay-in-previous-employment>
      <total-tax-in-previous-employment type="decimal">200.0</total-tax-in-previous-employment>
      <created-at type="dateTime">2024-08-07T14:27:32Z</created-at>
      <updated-at type="dateTime">2024-08-07T14:27:32Z</updated-at>
    </profile>
  </profiles>
</freeagent>
```
Show as JSON