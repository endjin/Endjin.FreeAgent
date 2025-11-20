# Cashflow

*Minimum access level*: `Banking`, unless stated otherwise.

Provides read-only access to historic cashflow data generated within FreeAgent. As Projected cashflow is not returned, requests with dates in the future will return a `total` of `0`.

## Cashflow summary for a given date range

```http
GET https://api.freeagent.com/v2/cashflow?from_date=2019-07-01&to_date=2019-09-30
```

- `from_date` - Date in YYYY-MM-DD format
- `to_date` - Date in YYYY-MM-DD format

### Response

```http
Status: 200 OK
```

```json
{
   "cashflow" : {
      "outgoing" : {
         "total" : "56276.55",
         "months" : [
            {
               "month" : 7,
               "year" : 2019,
               "total" : "16322.05"
            },
            {
               "total" : "20372.0",
               "year" : 2019,
               "month" : 8
            },
            {
               "month" : 9,
               "year" : 2019,
               "total" : "19582.5"
            }
         ]
      },
      "from" : "2019-07-01",
      "to" : "2019-09-30",
      "incoming" : {
         "months" : [
            {
               "month" : 7,
               "year" : 2019,
               "total" : "21387.76"
            },
            {
               "total" : "25690.0",
               "year" : 2019,
               "month" : 8
            },
            {
               "total" : "21792.0",
               "year" : 2019,
               "month" : 9
            }
         ],
         "total" : "68869.76"
      },
      "balance" : "12593.21"
   }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <cashflow>
    <from type="date">2019-07-01</from>
    <to type="date">2019-09-30</to>
    <incoming>
      <months type="array">
        <month>
          <month type="integer">7</month>
          <year type="integer">2019</year>
          <total type="decimal">21387.76</total>
        </month>
        <month>
          <month type="integer">8</month>
          <year type="integer">2019</year>
          <total type="decimal">25690.0</total>
        </month>
        <month>
          <month type="integer">9</month>
          <year type="integer">2019</year>
          <total type="decimal">21792.0</total>
        </month>
      </months>
      <total type="decimal">68869.76</total>
    </incoming>
    <outgoing>
      <months type="array">
        <month>
          <month type="integer">7</month>
          <year type="integer">2019</year>
          <total type="decimal">16322.05</total>
        </month>
        <month>
          <month type="integer">8</month>
          <year type="integer">2019</year>
          <total type="decimal">20372.0</total>
        </month>
        <month>
          <month type="integer">9</month>
          <year type="integer">2019</year>
          <total type="decimal">19582.5</total>
        </month>
      </months>
      <total type="decimal">56276.55</total>
    </outgoing>
    <balance type="decimal">12593.21</balance>
  </cashflow>
</freeagent>
```
Show as JSON