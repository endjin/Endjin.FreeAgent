# Attachments

*Minimum access level*: To access attachments you must have the same access level as the item the file is attached to.

## Attributes

| Attribute            | Description                                                                                             | Kind      |
| -------------------- | ------------------------------------------------------------------------------------------------------- | --------- |
| url                  | The unique identifier for the attachment                                                                | URI       |
| content\_src         | URL to the original full-size file                                                                      | URI       |
| content\_src\_medium | URL to a medium-size thumbnail (if a thumbnail exists)                                                  | URI       |
| content\_src\_small  | URL to a small-size thumbnail (if a thumbnail exists)                                                   | URI       |
| expires\_at          | When the `content_src` URLs expire. New URLs should be fetched from the API once this timestamp passes. | Timestamp |
| content_type         | MIME-type                                                                                               | String    |
| file_name            | Original name of the file                                                                               | String    |
| file_size            | Size of original file in bytes                                                                          | Integer   |
| description          | Free-text description                                                                                   | String    |

## Show a single attachment

```http
GET https://api.freeagent.com/v2/attachments/:id
```

### Response

```http
Status: 200 OK
```

```json
{  "attachment":
     {
        "url":"https://api.freeagent.com/v2/attachments/3",
        "content_src":"https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&Expires=1314281186&Signature=GFAKDo%2Bi%2FsUMTYEgg6ZWGysB4k4%3D",
        "content_src_medium": "https://5tak8uqfvj.execute-api.eu-west-1.amazonaws.com/thumbnail/freeagent-production-attachments/eyJrZXkiOiJhdHRhY2htZW50cy8yNDk5NzY3Ni9vcmlnaW5hbC5wbmciLCJyZWdpb24iOiJldS13ZXN0LTEiLCJ3aWR0aCI6NTAwLCJoZWlnaHQiOjUwMH0=",
        "content_src_small": "https://5tak8uqfvj.execute-api.eu-west-1.amazonaws.com/thumbnail/freeagent-production-attachments/eyJrZXkiOiJhdHRhY2htZW50cy8yNDk5NzY3Ni9vcmlnaW5hbC5wbmciLCJyZWdpb24iOiJldS13ZXN0LTEiLCJ3aWR0aCI6MjUwLCJoZWlnaHQiOjI1MH0=",
        "expires_at": "2020-12-03T10:27:52.000Z",
        "content_type":"image/png",
        "file_name":"barcode.png",
        "file_size":7673,
        "description":"Receipt for coffee"
      }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
    <attachment>
      <url>https://api.freeagent.com/v2/attachments/3</url>
      <content-src>https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png?AWSAccessKeyId=1K3MW21E6T8KWBY84B02&amp;Expires=1314281298&amp;Signature=jhGeAgqdnDwyFKHJoPI6AKU%2Fb2s%3D</content-src>
      <content-src-medium>https://5tak8uqfvj.execute-api.eu-west-1.amazonaws.com/thumbnail/freeagent-production-attachments/eyJrZXkiOiJhdHRhY2htZW50cy8yNDk5NzY3Ni9vcmlnaW5hbC5wbmciLCJyZWdpb24iOiJldS13ZXN0LTEiLCJ3aWR0aCI6NTAwLCJoZWlnaHQiOjUwMH0=</content-src-medium>
      <content-src-small>https://5tak8uqfvj.execute-api.eu-west-1.amazonaws.com/thumbnail/freeagent-production-attachments/eyJrZXkiOiJhdHRhY2htZW50cy8yNDk5NzY3Ni9vcmlnaW5hbC5wbmciLCJyZWdpb24iOiJldS13ZXN0LTEiLCJ3aWR0aCI6MjUwLCJoZWlnaHQiOjI1MH0=</content-src-small>
      <expires-at>image/png</expires-at>
      <content-type>image/png</content-type>
      <file-name>barcode.png</file-name>
      <file-size type="integer">7673</file-size>
      <description>Receipt for coffee</description>
    </attachment>
  </expense>
</freeagent>
```
Show as JSON

## Delete a single attachment

```http
DELETE https://api.freeagent.com/v2/attachments/:id
```

### Response

```http
Status: 200 OK
```