---
layout: default
title: SpeedSearch System Endpoint
back: system_security
next: system_systemoptions
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## SpeedSearch System Endpoint ##

### Purpose ###
The indexDefinitions system endpoint provide GET, POST, PUT, and DELETE access to the
index definition's attributes in both Atom and JSON/BSON formats. The endpoint also supports
standard payload options such as filtering, sorting, and paging.

### Background ###
SpeedSearch enables users to search for information stored in your Sage SalesLogix database
as well as external documents on your network. SpeedSearch indexes are managed on the
`INDEXDEFINITION` table. Indexes are managed through the Sage SalesLogix Administrator by
going to Manage > SpeedSearch Indexes. This is typically done by an administrator. For more
information on managing SpeedSearch indexes, see the "Managing SpeedSearch Indexes"
help topic in the Sage SalesLogix Administrator help.

### Endpoint Segments ###
| Component         | Name              | Purpose                 | Example                        |
|-------------------|-------------------|-------------------------|--------------------------------|
| /indexDefinitions | Resource kind     | Returns all the SpeedSearch indexes |                    |
| ('id')            | Resource selector | Selects a specific SpeedSearch index | /indexDefinitions('QDEMOA006M8R') |
| where             |                   | Filters feed results | /indexDefinitions?where=enabled eq 'false' |
| select            |                   | Selects the properties that will be returned in the payload. Properties are category, name, displayName, defaultValue, value, and locked. | /indexDefinitions?select=enabled |
| include           |                   | Expands the children into the payload. | /indexDefinitions('QDEMOA006M8R')?include=filterFields |

### Errors and Warnings ###
- If you request an index that does not exist, you get an empty feed.
- All the standard SData errors apply (for example, malformed URLs or payloads and
missing `If-Match` header during updates.)

### Sample Calls ###

List all indexDefinitions:

    /slx/system/-/indexDefinitions

List all indexDefinitions ordered by index name:

    /slx/system/-/indexDefinitions?orderBy=indexName

Look up an indexDefinition by indexID:

    /slx/system/-/indexDefinitions('QDEMOA006M9B')

Look up an indexDefinition by index name:

    /slx/system/-/indexDefinition?where=indexName eq 'Account'

Look up all enabled indexDefinitions:

    /slx/system/-/indexDefinition?where=enabled

### Code Samples Using SData Client Libraries ###

**Display a list of index names**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var indexes = await client.Query("indexDefinitions")
    .Select(x => x["indexName"])
    .ToArrayAsync();
Console.WriteLine(string.Join(Environment.NewLine, indexes));
```

**Display a table of index filters**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var filters = (await client.Query("indexDefinitions")
    .Fetch(x => x["filterFields"])
    .ToListAsync())
    .SelectMany(x => ((IList<SDataResource>) x["filterFields"] ?? new SDataResource[0])
        .DefaultIfEmpty(new SDataResource {{ "{{" }}"displayName", null}, {"fieldType", null}})
        .Select(f => new
            {
                IndexId = x.Key,
                IndexName = x["indexName"],
                FilterName = f["displayName"],
                FilterType = f["fieldType"]
            }))
    .ToList();
new Form
    {
        Controls =
            {
                new DataGridView
                    {
                        DataSource = filters,
                        Dock = DockStyle.Fill
                    }
            }
    }.ShowDialog();
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)