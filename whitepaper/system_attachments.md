---
layout: default
title: Attachments System Endpoint
back: system_activities
next: system_groups
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The Attachments system endpoint provides GET, POST, PUT and DELETE access to
attachments in both Atom and JSON/BSON formats. The endpoint supports standard payload
options such as filtering, sorting, and paging.

### Background ###
Attachments can be any documents, text files, or graphics that provide relevant information to
an account, contact, opportunity, ticket, lead, contract, return, or defect. For more information,
refer to the "Attachments Tab" topic in the Sage SalesLogix Web Client help system.

### Endpoint Segments ###
| Component    | Name              | Purpose                      | Example                        |
|--------------|-------------------|------------------------------|--------------------------------|
| /attachments | Resource kind     | Returns all attachments      |                                |
| ('id')       | Resource selector | Returns selected attachment  | /attachments('QDEMOA00036B')   |
| /file        | Relationship property | Downloads the file requested |                            |
| where        |                   | Filters feed results         | attachments?where=fileName eq 'AbbottLtd2005gcr.pdf' |
| select       |                   | Select specific items and excludes all others. | /attachments?select=name |
| include      |                   | Include specific relationships in addition to others |        |

### Errors and Warnings ###
If you request an attachment that does not exist, you receive one of the following:

- If a where condition is used in the SData query, the endpoint will return an empty
feed.
- If a predicate is used in the SData query, the endpoint will return an error with
diagnosis.

All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all attachments:

    /slx/system/-/attachments

List all attachments ordered by file name:

    /slx/system/-/attachments?orderBy=fileName

Look up an attachment by ID:

    /slx/system/-/attachments('QDEMOA000646')

Look up an attachment by file name:

    /slx/system/-/attachments?where=fileName eq '2004FactBook.pdf'

Note that fileName should not be used in predicate expressions since it is not guaranteed to
be unique.

Look up all attachments for a specific account:

    /slx/system/-/attachments?where=accountId eq 'AGHEA0002669'

Fetch an attachment file:

    /slx/system/-/attachments('QDEMOA00064I')?precedence=0

Precedence has been set to zero because we only want the associated file, not the entity
payload.
If the server is configured correctly and the file can be found, then the response should be a
multipart MIME message containing both the entity payload and the file contents.

### Code Samples Using SData Client Libraries ###

**Download and open an attachment file**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var result = await client.ExecuteAsync(new SDataParameters
    {
        Path = "attachments('QDEMOA00063B')",
        ExtensionArgs = {{ "{{" }}"includeFile", "true"}}
    });
foreach (var file in result.Files)
{
    var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
    using (var stream = File.OpenWrite(filePath))
    {
        await file.Stream.CopyToAsync(stream);
    }
    Process.Start(filePath);
}
```

**Upload a file as an attachment**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var stream = File.OpenRead(@"C:\Temp\Foo.txt");
var file = new AttachedFile(null, "Foo.txt", stream);
await client.ExecuteAsync(new SDataParameters
    {
        Method = HttpMethod.Post,
        Path = "attachments",
        Content = new {},
        ContentType = MediaType.Json,
        Files = {file}
    });
```

The attachment payload contains a sortable/filterable "url" property that's populated with the
URL value found inside the associated file, provided its extension is .url and it can be
successfully parsed. Filtering and sorting of this property must be performed in memory, so the
usual in-memory performance implications apply.

Creation of URL link attachments can be accomplished by either posting a payload with the
URL property set or posting an .url file attachment just like any other file type.
Unlike normal attachments, files are not included as MIME parts in single GET requests unless
the _includeFile=true custom query argument is explicitly specified.

**Upload a URL attachment with a file**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
const string data = "[InternetShortcut]\r\nURL=http://google.com";
var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
var file = new AttachedFile(null, "google.url", stream);
var results = await client.ExecuteAsync<SDataResource>(new SDataParameters
    {
        Method = HttpMethod.Post,
        Path = "attachments",
        Content = new {},
        ContentType = MediaType.Json,
        Files = {file}
    });
var attachment = results.Content;
Debug.Assert(Equals(attachment["fileName"], "google.url"));
Debug.Assert(Equals(attachment["url"], "http://google.com"));
```

**Upload a URL attachment with a payload**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var attachment = new
    {
        url = "http://google.com",
        fileName = "google.url"
    };
await client.PostAsync(attachment, "attachments");
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)