# Notes

*Minimum access level* `Contacts and Projects`, unless stated otherwise.

Contacts and Projects can both have Notes.

## Attributes

| Required | Attribute  | Description                                                                           | Kind      |
| -------- | ---------- | ------------------------------------------------------------------------------------- | --------- |
|          | url        | The unique identifier for the note                                                    | URI       |
| ✔        | note       | The content of the note                                                               | String    |
|          | parent_url | URL to the [project](projects.md) or [contact](contacts.md) which the note belongs to | URI       |
|          | author     | Name of the [user](users.md) that created the note                                    | String    |
|          | created_at | When the note was created (UTC)                                                       | Timestamp |
|          | updated_at | When the note was last updated (UTC)                                                  | Timestamp |

## List all notes for a contact

Requires the contact to be specified

```http
GET  https://api.freeagent.com/v2/notes?contact=https://api.freeagent.com/v2/contact/1
```

### Response

```http
Status: 200 OK
```

```json
{ "notes": [
        {
            "url": "https://api.freeagent.com/v2/notes/1",
            "note": "A new note",
            "parent_url": "https://api.freeagent.com/v2/contacts/1",
            "author": "Development Team",
            "created_at": "2012-05-30T10:22:34Z",
            "updated_at": "2012-05-30T10:22:34Z"
        }
    ]
}

```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <notes type="array">
    <note>
      <url>https://api.freeagent.com/v2/notes/1</url>
      <note>A new note</note>
      <parent-url>https://api.freeagent.com/v2/contacts/1</parent-url>
      <author>Development Team</author>
      <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
      <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
    </note>
  </notes>
</freeagent>
```
Show as JSON

## List all notes for a project

Requires the project to be specified

```http
GET  https://api.freeagent.com/v2/notes?project=https://api.freeagent.com/v2/project/1
```

### Response

```http
Status: 200 OK
```

```json
{ "notes": [
        {
            "url": "https://api.freeagent.com/v2/notes/1",
            "note": "A new note",
            "parent_url": "https://api.freeagent.com/v2/project/1",
            "author": "Development Team",
            "created_at": "2012-05-30T10:22:34Z",
            "updated_at": "2012-05-30T10:22:34Z"
        }
    ]
}

```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <notes type="array">
    <note>
      <url>https://api.freeagent.com/v2/notes/1</url>
      <note>A new note</note>
      <parent-url>https://api.freeagent.com/v2/project/1</parent-url>
      <author>Development Team</author>
      <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
      <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
    </note>
  </notes>
</freeagent>
```
Show as JSON

## Get a single note

```http
GET https://api.freeagent.com/v2/notes/1
```

### Response

```http
Status: 200 OK
```

```json
{ "note": {
      "url": "https://api.freeagent.com/v2/notes/1",
      "note": "A new note",
      "parent_url": "https://api.freeagent.com/v2/project/1",
      "author": "Development Team",
      "created_at": "2012-05-30T10:22:34Z",
      "updated_at": "2012-05-30T10:22:34Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <note>
    <url>https://api.freeagent.com/v2/notes/1</url>
    <note>A new note</note>
    <parent-url>https://api.freeagent.com/v2/project/1</parent-url>
    <author>Development Team</author>
    <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
    <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
  </note>
</freeagent>
```
Show as JSON

## Create a note for a contact

```http
POST https://api.freeagent.com/v2/notes?contact=https://api.freeagent.com/v2/contact/1
```

### Input

A `note` root object containing the following attribute:

- `note`  (Required, the text of the note)

### Response

```http
Status: 200 OK
```

```json
{ "note": {
      "url": "https://api.freeagent.com/v2/notes/1",
      "note": "A new note",
      "parent_url": "https://api.freeagent.com/v2/contact/1",
      "author": "Development Team",
      "created_at": "2012-05-30T10:22:34Z",
      "updated_at": "2012-05-30T10:22:34Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <note>
    <url>https://api.freeagent.com/v2/notes/1</url>
    <note>A new note</note>
    <parent-url>https://api.freeagent.com/v2/contact/1</parent-url>
    <author>Development Team</author>
    <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
    <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
  </note>
</freeagent>
```
Show as JSON

## Create a note for a project

```http
POST https://api.freeagent.com/v2/notes?project=https://api.freeagent.com/v2/projects/1
```

A `note` root object containing the following attribute:

- `note`  (Required, the text of the note)

### Response

```http
Status: 200 OK
```

```json
{ "note": {
      "url": "https://api.freeagent.com/v2/notes/1",
      "note": "A new note",
      "parent_url": "https://api.freeagent.com/v2/project/1",
      "author": "Development Team",
      "created_at": "2012-05-30T10:22:34Z",
      "updated_at": "2012-05-30T10:22:34Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <note>
    <url>https://api.freeagent.com/v2/notes/1</url>
    <note>A new note</note>
    <parent-url>https://api.freeagent.com/v2/project/1</parent-url>
    <author>Development Team</author>
    <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
    <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
  </note>
</freeagent>
```
Show as JSON

## Update a note

```http
PUT https://api.freeagent.com/v2/notes/1
```

### Input

A `note` root object containing the following attribute:

- `note`  (Required, the text of the note)

### Response

```http
Status: 200 OK
```

```json
{ "note": {
      "url": "https://api.freeagent.com/v2/notes/1",
      "note": "A new note",
      "parent_url": "https://api.freeagent.com/v2/project/1",
      "author": "Development Team",
      "created_at": "2012-05-30T10:22:34Z",
      "updated_at": "2012-05-30T10:22:34Z"
    }
}
```
Show as XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<freeagent>
  <note>
    <url>https://api.freeagent.com/v2/notes/1</url>
    <note>A new note</note>
    <parent-url>https://api.freeagent.com/v2/project/1</parent-url>
    <author>Development Team</author>
    <created-at type="datetime">2012-05-30T10:22:34Z</created-at>
    <updated-at type="datetime">2012-05-30T10:22:34Z</updated-at>
  </note>
</freeagent>
```
Show as JSON

## Delete a note

```http
DELETE https://api.freeagent.com/v2/notes/1
```

### Response

```http
Status: 200 OK
```