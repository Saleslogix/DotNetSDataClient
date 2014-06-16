---
layout: default
title: Groups System Endpoint
back: system_attachments
next: system_library
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The groups system endpoint provides GET, POST, PUT, and DELETE access to the groups
attributes. It is a simple CRUD implementation used to create ad hoc groups and is not
designed for complex dynamic group creation. The endpoint also supports standard payload
options such as filtering, sorting, and paging.

The groups system endpoint will return duplicates if the current user has access to multiple
releases of a given group. When requesting a specific group's metadata via a predicate
expression, usually involving family and name, duplicates will automatically be removed so that
a single entry can be returned. The version and modify date properties are exposed so
duplicates may be removed from the feed on the client-side.

### Background ###
For general information on Groups, refer to the help topic "Using Groups" in the Sage
SalesLogix Web Client help system. Sage SalesLogix has two kinds of groups: standard and
ad hoc. Groups are created by individual users but can be shared with other users. Multiple
versions and releases of a group may exist.

The `PLUGIN` table contains the plugin information for groups created in Sage SalesLogix. Ad
hoc groups are stored in the `ADHOCGROUP` table. The table contains the plugin name, family,
numeric type, user creating the plugin, date created or modified, version number of the plugin,
company details, read only and other attributes.

### Endpoint Segments ###
| Component      | Name              | Purpose                  | Example                          |
|----------------|-------------------|--------------------------|----------------------------------|
| /groups        | Resource kind     | Returns all metadata on all groups for the logged in user | |
| ('id')         | Resource selector | Selects a specific group | /groups('p6UJ9A0004TS') or any predicate expression, for example: /groups(name eq 'All Contacts') |
| /$queries/execute | Named query    | Executes the specified group | /groups('p6UJ9A0004TS')/$queries/execute |
| where          |                   | Filters feed results     | /groups?where=mainTable eq 'Account' |
| select         |                   | Selects the properties that will be returned in the payload. | /groups?select=name |

### Errors and Warnings ###
If you request a group that does not exist, you get:

- an empty feed if a where condition is used
- an "entity does not exist" response if a predicate is used.

All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List available groups:

    /slx/system/-/groups

Look up a specific group using its plugin ID:

    /slx/system/-/groups('p6UJ9A0004TS')

Or any predicate expression, for example:

    /slx/system/-/groups(name eq 'All Contacts')

Look up a specific group using its family and name:

    /slx/system/-/groups(family eq 'Account' and name eq 'All Accounts')

Any predicate expression can be used so long as all the results have the same family and name.
If multiple releases or versions are found, then the latest is returned based on version and modifyDate.

List all groups for a given family:

    /slx/system/-/groups?where=family eq 'Account'

List all groups ordered by their names:

    /slx/system/-/groups?orderBy=name

List all groups that have not been explicitly hidden by the current user:

    /slx/system/-/groups?where=not isHidden

List all ad hoc groups:

    /slx/system/-/groups?where=isAdHoc

List all ad hoc groups and include the entity IDs:

    /slx/system/-/groups?where=isAdHoc&include=adHocIds

List all local and global joins available in the group:

    /slx/system/-/groups('p6UJ9A00024K')/$queries/availableJoins

or

    /slx/system/-/groups(upper(family) eq 'ACCOUNT' and upper(name) eq 'ALL ACCOUNTS')/$queries/availableJoins

### Executing Groups ###

The payload is generated from the group layout.

Execute a group based on its plugin ID:

    /slx/system/-/groups/$queries/execute?_groupId=p6UJ9A0004TS

Where _groupId is a custom query argument. The SData specification requires that they be
prefixed with an underscore.

Execute a group based on its family and name:

    /slx/system/-/groups/$queries/execute?_family=Account&_name=All Accounts

Both _family and _name must be specified.

Execute a group with a custom order clause:

    /slx/system/-/groups/$queries/execute?_groupId=p6UJ9A0004TS&orderBy=PERIOD desc

Where `PERIOD` is a column in the Active Contracts group.

Execute a group with a custom filter clause:

    /slx/system/-/groups/$queries/execute?_groupId=p6UJ9A0004TS&where=ISACTIVE eq 'T'

### Code Samples Using SData Client Libraries ###

**Create an Account Ad-Hoc group with three accounts**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var group = new
    {
        family = "Account",
        name = "TestAdHocGroup",
        adHocIds = new[]
            {
                "AA2EK0013017",
                "AA2EK0013018",
                "AA2EK0013019"
            }
    };
await client.PostAsync(group, "groups");
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)