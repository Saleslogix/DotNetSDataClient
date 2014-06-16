---
layout: default
title: Mashup Endpoints
back: metadata_endpoints
next: gcrm_endpoints
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

The SData mashup adapter is a platform level adapter that exposes rich mashup metadata and
a $queries endpoint that allows mashups to be executed on the server, similar to the groups
endpoint.

### Background ###
Mashups provide access to related data in several locations, usually database tables, in one
easy to access location or feed. For example, a mashup may contain related information from
the Activities, Resources, Resource Schedules, and User tables to provide information from one
location to populate an Activities calendar. For more information see the "Working with
Mashups" topic in the Application Architect Help.

### Sample Calls ###
List the available mashup names without content:

    /$app/mashups/-/mashups?precedence=0

Look up a specific mashup by name:

    /$app/mashups/-/mashups('AccountTimeline')

Execute a mashup:

    /$app/mashups/-/mashups('TicketTimeline')/$queries/execute?_resultName=AllResults

The required _resultName argument is the name of the mashup processor that produces
the record stream.

Execute a mashup with a run-time parameter value specified:

    /$app/mashups/-/mashups('TicketTimeline')/$queries/execute?_resultName=AllResults&_EntityId=tDEMOA00000A

Run-time parameters can be referenced in expressions and templated properties anywhere
in the mashup and do not need to correspond to a named parameter defined on the
mashup.

Execute a mashup with selects, filters, and sorts:

    /$app/mashups/-/mashups('TicketTimeline')/$queries/execute?_resultName=AllResults&select=Title&where=IsDuration&orderBy=Start

### Code Samples Using SData Client Libraries ###

**Create and execute a new mashup**

```csharp
var client = new SDataClient(BaseUri + "$app/mashups/-")
    {
        UserName = UserName,
        Password = Password
    };
var mashup = new
    {
        name = "HqlTest",
        processors = SDataCollection.Create(true, new[]
            {
                new
                    {
                        queryProcessor = new
                            {
                                name = "Query",
                                query = "select Id from Contact order by LastName",
                                maximumResults = 10
                            }
                    }
            })
    };
await client.PostAsync(mashup, "mashups");
var ids = await client.Query("mashups('HqlTest')/$queries/execute")
    .WithExtensionArg("resultName", "AllResults")
    .WithExtensionArg("EntityId", "AGHEA0002669")
    .Select(x => (string) x["Id"])
    .ToListAsync();
Console.WriteLine(string.Join(Environment.NewLine, ids));
```

**Execute an existing mashup**

```csharp
var client = new SDataClient(BaseUri + "$app/mashups/-")
    {
        UserName = UserName,
        Password = Password
    };
var descriptions = await client.Query("mashups('AccountTimeline')/$queries/execute")
    .WithExtensionArg("resultName", "AllResults")
    .WithExtensionArg("EntityId", "AGHEA0002669")
    .Select(x => (string) x["Description"])
    .ToListAsync();
Console.WriteLine(string.Join(Environment.NewLine, descriptions));
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)