# Profit & Loss

*Minimum access level*: `Tax, Accounting & Users`, unless stated otherwise.

## Attributes

| Attribute                       | Description                                                                                                                                                                                                                                         | Kind    |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| from                            | Start date of the P&L report                                                                                                                                                                                                                        | Date    |
| to                              | End date of the P&L report                                                                                                                                                                                                                          | Date    |
| income                          | Income at the end of the report's period, in the company's native [currency](currencies.md)                                                                                                                                                         | Decimal |
| expenses                        | Expenses at the end of the report's priod, in the company's native [currency](currencies.md)                                                                                                                                                        | Decimal |
| operating_profit                | Profit from the business' day-to-day trading and activities, at the end of the report's period, in the company's native [currency](currencies.md)                                                                                                   | Decimal |
| less                            | Totals which are subtracted from the operating profit in order to calculate the retained profit. Each item has the following elements: `title`: What is being subtracted, `total`: Amount subtracted, in company's native [currency](currencies.md) | Array   |
| retained_profit                 | Retained profit at the end of the report's period                                                                                                                                                                                                   | Decimal |
| retained_profit_brought_forward | Retained profit brought forward from previous year                                                                                                                                                                                                  | Decimal |
| retained_profit_carried_forward | Distributable profit at the end of the report's period                                                                                                                                                                                              | Decimal |

## Get the P&L summary

```http
GET https://api.freeagent.com/v2/accounting/profit_and_loss/summary
```

#### Date Filters

*Note:* Requested date periods must be equal to or less than 12 months or be contained within a single accounting year.

The request can be filtered either with an annual accounting period or by specifying start and end dates.
When no parameters are provided the default time period will be the current accounting year
to date.

- `from_date`
- `to_date`

The `from_date` and `to_date` params should be in the format `YYYY-MM-DD`. If specifying only one of the date params,
the other will default: `from_date` to the start of the current accounting period and `to_date` to today.

- `accounting_period`

The accounting period parameter should be in the format `2022/23`, indicating the start and end year of the period.

### Response

The profit and loss for the current accounting period.

```http
Status: 200 OK
```

```json
{
  "profit_and_loss_summary": {
    "from": "2016-06-01",
    "to": "2016-09-05",
    "income": "3800",
    "expenses": "8200",
    "operating_profit": "-4400",
    "less": [
      {
        "title": "Corp. Tax",
        "total": "0"
      },
      {
        "title": "Dividends",
        "total": "50"
      },
      {
        "title": "Adjustments",
        "total": "0"
      }
    ],
    "retained_profit": "-4450",
    "retained_profit_brought_forward": "15817",
    "retained_profit_carried_forward": "11367"
  }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <profit-and-loss-summary>
    <from type="date">2016-06-01</from>
    <to type="date">2016-09-05</to>
    <income>3800</income>
    <expenses>8200</expenses>
    <operating-profit>-4400</operating-profit>
    <less type="array">
      <less>
        <title>Corp. Tax</title>
        <total>0</total>
      </less>
      <less>
        <title>Dividends</title>
        <total>50</total>
      </less>
      <less>
        <title>Adjustments</title>
        <total>0</total>
      </less>
    </less>
    <retained-profit>-4450</retained-profit>
    <retained-profit-brought-forward>15817</retained-profit-brought-forward>
    <retained-profit-carried-forward>11367</retained-profit-carried-forward>
  </profit-and-loss-summary>
</freeagent>
```
Show as JSON