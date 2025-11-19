# Using the FreeAgent API with Curl

Getting started is easy.  First, [get an Access Token](quick_start.md).

Open a terminal and paste in the following command, replacing **TOKEN** with the Access Token:

```json
curl https://api.freeagent.com/v2/company \
  -H "Authorization: Bearer TOKEN" \
  -H "Accept: application/xml" \
  -H "Content-Type: application/xml" \
  -X GET 
```

You should see the response:

```json
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <company>
    <url>https://api.freeagent.com/v2/company</url>
    <name>My Company</name>
    <subdomain>mycompany</subdomain>
    <type>UkLimitedCompany</type>
    <currency>GBP</currency>
    <mileage-units>miles</mileage-units>
    <company-start-date type="date">2010-05-01</company-start-date>
    <freeagent-start-date type="date">2010-05-01</freeagent-start-date>
    <first-accounting-year-end type="date">2010-05-01</first-accounting-year-end>
    <sales-tax-registration-status>Registered</sales-tax-registration-status>
    <sales-tax-registration-number>123456</sales-tax-registration-number>
  </company>
</freeagent>
```

By modifying the above command you can access all of the FreeAgent API.

To use the sandbox API change the server in the examples above to:
      **https://api.sandbox.freeagent.com**

To use JSON instead of XML change **application/xml** to
      **application/json**

[Learn more about the FreeAgent API](introduction.md)