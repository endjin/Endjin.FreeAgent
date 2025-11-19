# Construction Industry Scheme Bands

*Only available to UK companies enrolled in the Construction Industry Scheme for Subcontractors*

*Minimum access level*:

- `Estimates & Invoices`

## List all CIS bands for a company

```http
GET https://api.freeagent.com/v2/cis_bands
```

These are the Construction Industry Scheme bands that have been selected for the company in the
CIS for Subcontractors settings.

Since these bands do not change frequently, we suggest caching them client-side and updating
every once in a while, for example once a day or at app start-up. Use the names of the bands in
conjunction with the `cis_rate` attribute on invoices.

### Response

```http
Status: 200 OK
```

```json
{
  "available_bands": [
    {
      "name": "cis_gross",
      "deduction_rate": "0.0",
      "income_description": "CIS Income (Gross)",
      "deduction_description": "CIS Deduction (Gross)",
      "nominal_code": "061"
    },
    {
      "name": "cis_standard",
      "deduction_rate": "0.2",
      "income_description": "CIS Income (20%)",
      "deduction_description": "CIS Deduction (20%)",
      "nominal_code": "062"
    },
    {
      "name": "cis_higher",
      "deduction_rate": "0.3",
      "income_description": "CIS Income (30%)",
      "deduction_description": "CIS Deduction (30%)",
      "nominal_code": "063"
    }
  ]
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <available-bands type="array">
    <available-band>
      <name>cis_gross</name>
      <deduction-rate type="decimal">0.0</deduction-rate>
      <income-description>CIS Income (Gross)</income-description>
      <deduction-description>CIS Deduction (Gross)</deduction-description>
      <nominal-code>061</nominal-code>
    </available-band>
    <available-band>
      <name>cis_standard</name>
      <deduction-rate type="decimal">0.2</deduction-rate>
      <income-description>CIS Income (20%)</income-description>
      <deduction-description>CIS Deduction (20%)</deduction-description>
      <nominal-code>062</nominal-code>
    </available-band>
    <available-band>
      <name>cis_higher</name>
      <deduction-rate type="decimal">0.3</deduction-rate>
      <income-description>CIS Income (30%)</income-description>
      <deduction-description>CIS Deduction (30%)</deduction-description>
      <nominal-code>063</nominal-code>
    </available-band>
  </available-bands>
</freeagent>
```
Show as JSON