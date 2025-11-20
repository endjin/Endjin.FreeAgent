# Versioning

The FreeAgent API will soon use versioning so that non-backwards compatible changes can be deployed faster and with less risk.

Examples of a non-backwards compatible change could be:

- deleting an attribute
- modifying the name of an attribute
- changing the meaning of the response

Changes which we would consider to be backwards compatible would not generate a new version. For example:

- adding a new endpoint
- adding an attribute to an existing endpoint
- fixing a bug with the existing response

All endpoints will accept a version from the `X-Api-Version` header in date format (e.g. `2024-10-01`).

You should set the date in the `X-Api-Version` header in your request to be on or after the version of the API endpoint you wish to access.

If there is a version available for that date, then you'll get the new behaviour. If there isn't then it'll fall back to the previous version.

If no version header is passed to the API, then you'll get back the version as it stands today, before versioning is introduced. This should mean that integrations not supplying version numbers will still work as normal.

If there are multiple versions available for an endpoint, you will see them listed on the documentation for that endpoint.

An example API call with the header specified would look like this:

```json
curl https://api.freeagent.com/v2/company \
  -H "Authorization: Bearer TOKEN" \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -H "X-Api-Version: 2024-10-01" \
  -X GET
```

See the [Using the FreeAgent API with curl](using_curl.md) page for more information on this format.