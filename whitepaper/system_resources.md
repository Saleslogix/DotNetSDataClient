---
layout: default
title: Resources System Endpoint
back: system_picklists
next: system_security
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The resources system endpoints provide GET, POST, PUT, and DELETE access to the
resource's attributes in both Atom and JSON/BSON formats. The endpoints also support
standard payload options such as filtering, sorting, and paging.

### Background ###
Resources are managed on two tables, `RESOURCELIST` and `RESOURCESCHEDULE`. The
`RESOURCELIST` table stores the resource and the `RESOURCESCHEDULE` table stores the
information on when the resource is being used. Administrative users can manage the list of
resources in the Web Client. See the "Managing Resources" topic in the Sage SalesLogix Web
Client help.

### Endpoint Segments ###
| Component          | Name              | Purpose                 | Example                       |
|--------------------|-------------------|-------------------------|-------------------------------|
| /resources         | Resource kind     | Returns all resources   |                               |
| ('id')             | Resource selector | Returns selected resource | /resources('RGHEA0002884')  |
| /resourceSchedules | Resource kind     | Returns all resource schedules |                        |
| where              |                   | Filters feed results | /resources?where=Type eq 'Projector' |
| select             |                   | Selects the properties that will be returned in the payload. | /resources?select=name |
| include            |                   |                         | /resourceSchedules?include=Resource |

### Errors and Warnings ###
All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all resources:

    /slx/system/-/resources

List all resource schedules:

    /slx/system/-/resourceSchedules

Get schedule of a specific resource:

    /slx/system/-/resourceSchedules?where=Resource.Id eq 'RGHEA0002884'

### Code Samples Using SData Client Libraries ###

**Display a table of resources**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var resources = (await client.Query("resources")
    .WithPrecedence(0)
    .ToListAsync())
    .Select(x => new {Id = x.Key, Name = x.Descriptor})
    .ToList();
new Form
    {
        Controls =
            {
                new DataGridView
                    {
                        DataSource = resources,
                        Dock = DockStyle.Fill
                    }
            }
    }.ShowDialog();
```

**Display a table of resource schedules**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var schedules = (await client.Query("resourceSchedules")
    .Where(x => ((SDataResource) x["Resource"])["Id"] == "RGHEA0002884")
    .Select(x => new {Id = x.Key, StartDate = x["StartDate"], EndDate = x["EndDate"]})
    .ToListAsync())
    .ToList();
new Form
    {
        Controls =
            {
                new DataGridView
                    {
                        DataSource = schedules,
                        Dock = DockStyle.Fill
                    }
            }
    }.ShowDialog();
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)