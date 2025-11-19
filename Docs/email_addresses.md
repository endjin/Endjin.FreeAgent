# Email Addresses

*Minimum access level*: `Time`, unless stated otherwise.

## Get a list of verified sender email addresses

```http
GET https://api.freeagent.com/v2/email_addresses
```

### Response

```http
Status: 200 OK
```

```json
{
  "email_addresses": [
    "John Smith <jsmith@example.com>"
  ]
}
```
Show as XML

```xml
<freeagent>
  <email-addresses type="array">
    <email-address>John Smith &lt;jsmith@example.com&gt;</email-address>
  </email-addresses>
</freeagent>
```
Show as JSON