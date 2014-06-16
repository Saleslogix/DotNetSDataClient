---
layout: default
title: Activities System Endpoint
back: system_endpoints
next: system_attachments
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The Activities system endpoint provides GET, POST, and PUT access to activities in both Atom
and JSON/BSON formats. The endpoint supports standard payload options such as filtering,
sorting, and paging.

### Background ###
Activities include placing phone calls, attending meetings, and completing to-dos such as
sending correspondence or preparing presentations. Information for activities is stored in the
`ACTIVITY` table.

### Endpoint Segments ###
| Component   | Name              | Purpose                   | Example                            |
|-------------|-------------------|---------------------------|------------------------------------|
| /activities | Resource kind     | Returns all activities    |                                    |
| ('id')      | Resource selector | Returns selected activity | /activities('QDEMOA00036B')        |
| where       |                   | Filters feed results      | /activities?where=Type eq 'atPhoneCall' |
| select      |                   | Select specific items and excludes all others. | /activities?select=accountName |
| include     |                   | Include specific relationships in addition to others |         |

### Errors and Warnings ###
All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all activities:

    /slx/system/-/activities

List all activities ordered by Start Date:

    /slx/system/-/activities?orderBy=StartDate

Look up an activity by ID:

    /slx/system/-/activities('VDEMOA00003M')

Look up an activity by type:

    /slx/system/-/activities?where=Type eq 'atPhoneCall'

Look up all activities for a specific account:

    /slx/system/-/activities?where=AccountName eq 'Abbott Ltd.'

### Code Samples Using SData Client Libraries ###

**Display a list of all activities in chronological order**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var activities = await client.Query("activities")
    .Select(x => new
        {
            StartDate = x["StartDate"],
            Type = x["Type"],
            Description = x["Description"],
            AccountName = x["AccountName"],
            ContactName = x["ContactName"]
        })
    .OrderBy(a => a.StartDate)
    .ToListAsync();
new Form
    {
        Controls =
            {
                new DataGridView
                    {
                        DataSource = activities,
                        Dock = DockStyle.Fill
                    }
            }
    }.ShowDialog();
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)