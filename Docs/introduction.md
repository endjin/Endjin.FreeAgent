# The FreeAgent API

All API access is over HTTPS and accessed from `api.freeagent.com`.  JSON and XML formats are supported.  A user-agent must be specified to allow the FreeAgent API to identify your app.  Users can authenticate Apps using OAuth 2.0

Four HTTP verbs are used to access resources:

*GET* - Use when requesting an existing resource

*POST* - Use when creating a new resource

*PUT* - Use when updating an existing resource

*DELETE* - Use when deleting a resource

Use these verbs with the paths provided by the API.

## Company API and Accountancy Practice API

FreeAgent offers two related but different products: online accounting for companies, and [a practice dashboard](https://www.freeagent.com/accountants/practice-dashboard) for accountants and bookkeepers who have clients that use FreeAgent. API access is slightly different in the two cases. You’ll need different API Apps to access both - see details in the [Accountancy Practice API section](accountancy_practice_api.md).

## About Request and Response Formats

Requests and responses can be provided in either JSON or XML.  Set the HTTP Accept header to specify the response format and the HTTP Content-Type header to specify the request format.

JSON

```json
Accept: application/json
Content-Type: application/json
```

XML

```json
Accept: application/xml
Content-Type: application/xml
```

## Formatting XML

There are five characters which must be substituted by XML Entities when creating XML.  These are:

| Character | XML Entity |
| --------- | ---------- |
| &         | &amp;      |
| &lt;      | &lt;       |
| &gt;      | &gt;       |
| "         | &quot;     |
| '         | &apos;     |

## About Access Levels

Access to FreeAgent resources is limited by user permissions.  The minimum required access level is specified for each resource.  The access levels are as follows:

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

## Pagination

API requests that returns multiple items, such as listing one of the resources, will be paginated (default to 25 items per page). A client can retrieve a specific page by providing the `page` param. In order to alter the number of records returned per page, a client can provide the `per_page` param (limited to 100 items per page).

```http
GET https://api.freeagent.com/v2/invoices?page=5&per_page=50
```

The pagination info is included in the Link header:

```json
Link: <https://api.freeagent.com/v2/invoices?page=4&per_page=50>; rel="prev",
<https://api.freeagent.com/v2/invoices?page=6&per_page=50>; rel="next",
<https://api.freeagent.com/v2/invoices?page=1&per_page=50>; rel="first",
<https://api.freeagent.com/v2/invoices?page=10&per_page=50>; rel="last"
```

*line breaks are for display purposes only.*

The possible `rel` values are:

- `prev`: The URL of the previous page of results.
- `next`: The URI of the next page of results.
- `first`: The URI of the first page of results.
- `last`: The URI of the last page of results.

In addition, the `X-Total-Count` header will contain the total number of entries that it is possible to paginate over:

```json
    X-Total-Count: 26
```

## Rate Limits

The following rate limits are enforced on our API.

- 120 user requests per minute
- 3600 user requests per hour
- 15 token refreshes per minute

These limits are per individual user of your integration and are reset at the start of every hour/minute.

For users of the accountancy practice API, rate limits apply to individual clients (by sub-domain included in the headers),
rather than to the number of requests made by the practice as a whole.

If your integration exceeds these limits, our API will respond with a [429 (Too Many Requests)](https://tools.ietf.org/html/rfc6585#section-4) status code until the restriction has expired.

### Respecting the Rate Limits

Please ensure you take steps to have your integrations handle this response and back off before retrying. We will be reviewing apps which continue to make a high volume of requests to our API while rate limited, and may have to take action to further restrict apps which do not respect the limits.

The returned HTTP response of any throttled API request gives the details you need to implement an effective back off strategy:

```json
curl -X POST https://api.freeagent.com/v2/token_endpoint\?client_id\=CLIENT_ID\&client_secret\=CLIENT_SECRET\&grant_type\=refresh_token\&refresh_token\=TOKEN
HTTP/1.1 429 Too Many Requests
Date: Wed, 27 Mar 2019 20:47:40 GMT
Retry-After: 60

You must not exceed 15 requests per 60 seconds
```

In order for developers to test handling and back off strategy passing the custom header `X-RateLimit-Test` with a truthy value will artificially lower any API calls to the sandbox to 5 req/minute.

```json
curl -H "X-RateLimit-Test: true" -H "Authorization: Bearer {token}" https://api.sandbox.freeagent.com/v2/company
```