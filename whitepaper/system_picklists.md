---
layout: default
title: PickLists System Endpoint
back: system_library
next: system_resources
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The pick list system endpoint provides GET, POST, PUT, and DELETE access to the pick list
attributes in both Atom and JSON/BSON formats. The endpoint also supports standard
payload options such as filtering, sorting, and paging. A relationship property endpoint is
available for pick list items. Access to pick list items is also made possible through the use of
nested payloads.

### Background ###
A pick list definition consists of attribute settings and list items. User interfaces apply pick list
attribute settings to enforce security and business rules for individual pick lists. For example,
there are attributes that specify whether the items of a pick list are editable and whether multiple
items can be selected.

Pick lists are administered through the Pick List Manager which is available to the system
administrator and users who have been assigned the appropriate administrative role. Pick lists
can also be administered in the Sage SalesLogix Web Client. For more information, refer to the
"Managing Pick Lists" help topic in the Sage SalesLogix Administrator and "What are Pick
Lists?" help topic in the Web Client help systems.

### Endpoint Segments ###
| Component  | Name              | Purpose                 | Example                               |
|------------|-------------------|-------------------------|---------------------------------------|
| /pickLists | Resource kind     | Returns attribute information for all pick lists |              |
| ('id')     | Resource selector | Selects a specific pick list | /pickLists('kSYST0000331') |
| /items     | Relationship property | Returns items of the selected pick list without the pick list attribute information | /pickLists('kSYST0000331')/items |
| where      |                   | Filters feed results    | /pickLists?where=name like '%Skill%' |
| select     |                   | Selects the properties that will be returned in the payload. | /pickLists?select=name |
| include    |                   | Expands children in the payload. | /pickLists('kSYST0000331')?include=items |

### Errors and Warnings ###
Requesting a pick list that does not exist generates one of the following:

- If a where condition is used in the SData query, the endpoint will return an empty
feed.
- If a predicate is used in the SData query, the endpoint will return an error with
diagnosis.

All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all pick lists:

    /slx/system/-/pickLists

Look up a pick list by ID:

    /slx/system/-/pickLists('kSYST0000385')

Or any predicate expression that returns a single resource, for example:

    /slx/system/-/picklists(name eq 'Skill Category')

Look up a pick list by name:

    /slx/system/-/pickLists(name eq 'Region')

Include the pick list items in the payload with the pick list attribute values:

    /slx/system/-/pickLists?include=items

List all items for a pick list by ID (without the pick list attributes):

    /slx/system/-/pickLists('kSYST0000316')/items

List all items for a pick list by name:

    /slx/system/-/pickLists(name eq 'Opportunity Cycle')/items

Select only the text for each item:

    /slx/system/-/pickLists('kSYST0000316')/items?select=text

Order items by the definition sort order:

    /slx/system/-/pickLists('kSYST0000316')/items?orderBy=sort

Order items alphabetically:

    /slx/system/-/pickLists('kSYST0000316')/items?orderBy=text

### Code Samples Using SData Client Libraries ###

**Create a new pick list**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var pickList = new
    {
        name = "TestPickList",
        allowMultiples = true,
        valueMustExist = true,
        required = true,
        alphaSorted = true,
        noneEditable = true,
        items = new[]
            {
                new
                    {
                        text = "Text1",
                        code = "Code1",
                        number = 1
                    },
                new
                    {
                        text = "Text2",
                        code = "Code2",
                        number = 2
                    }
            }
    };
await client.PostAsync(pickList, "pickLists");
```

**Update an new pick list**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var pickList = await client.Query("pickLists")
    .Fetch(x => x["items"])
    .FirstOrDefaultAsync(x => x["name"] == "TestPickList");
pickList["allowMultiples"] = false;
pickList["valueMustExist"] = false;
pickList["required"] = false;
pickList["alphaSorted"] = false;
pickList["noneEditable"] = false;
var items = (IList<SDataResource>) pickList["items"];
items[0]["code"] += "_CHANGED";
items[0]["number"] += "0";
items[1]["code"] += "_CHANGED";
items[1]["number"] += "0";
await client.PutAsync(pickList, "pickLists");
```

**Delete an existing pick list**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var pickList = await client.Query("pickLists").FirstOrDefaultAsync(x => x["name"] == "TestPickList");
await client.DeleteAsync(pickList, "pickLists");
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)