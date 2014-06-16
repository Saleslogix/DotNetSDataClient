---
layout: default
title: Dynamic Endpoints
back: introduction
next: system_endpoints
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

Dynamic endpoints provide create, read, update, and delete (CRUD) access to Sage
SalesLogix entities. They can be used to query, filter, sort, and manipulate data through Sage
SalesLogix or third-party applications. Dynamic endpoints are enabled by default when the
SData portal is deployed. When SData is enabled for a Sage SalesLogix entity, the Application
Architect platform build process generates a new dynamic endpoint.

### Endpoint Segments ###
SData uses URLs to address the resources, resource collections, schema and operations that
are exposed by a service. For example: `http://www.example.com/sdata/slx/dynamic/prod/accounts?startIndex=21&count=10`

| Component       | Name              | Description/Comments                                       |
|-----------------|-------------------|------------------------------------------------------------|
| http            | Protocol          | https is also allowed.                                     |
| www.example.com | Server name       | IP address is also allowed (192.168.1.1). Can be followed by port number. For example www.example.com:5493. 5493 is the recommended port number for SData services that are not exposed on the Internet. |
| sdata           | Virtual Directory | Should be SData, unless the framework imposes something different. |
| slx             | Application       | Name of the application.                                   |
| dynamic         | Contract name     | An SData service can support several contracts side-by-side. For example, a typical Sage SalesLogix service supports system, dynamic, GCRM, and proxy contracts. |
| prod            | Dataset           | Identifies the dataset when the application gives access to several datasets, such as several companies and production/test datasets. If the application can only handle a single dataset, or if it can be configured with a default dataset, a hyphen can be used as a placeholder for the default dataset. |
| accounts        | Resource Kind     | This URL segment identifies the kind of resource that is queried (`account`, `contact`, `salesOrder`, etc.) This URL returns the collection of all account resources, as an Atom feed. |
| startIndex=21&count=10 | Query parameters | The `startIndex` and `count` parameters allow the service consumer to request a specific page in a collection. Use ampersand to add multiple query parameters. |

### Call Parameters ###
| Name    | Description                                           |
|---------|-------------------------------------------------------|
| where   | Filters results                                       |
| select  | Selects specific properties and excludes all others   |
| include | Includes specific relationships in addition to others |
| orderBy | Sorts the results                                     |

### Sample Calls ###
List all the dynamic feeds:

    /slx/dynamic/-/

Look up Account by name:

    /slx/dynamic/-/accounts?where=AccountName eq 'Above Marine'

Sort Contacts by last name:

    /slx/dynamic/-/contacts?orderBy=LastName

### Dynamic Endpoints in Sage SalesLogix v8.0 ###
You can view a list of all of the enabled feeds by browsing to
`http://servername:port/sdata/slx/dynamic/-/`. SData feeds for entities can be enabled and
disabled within the Application Architect. For more information on how to enable SData feeds,
see the "Generating SData Feeds for an Entity" help topic in the Application Architect Help files.

### Creating Your Own Application ###
It is possible to consume these endpoints from your own application using Visual Studio. To
build your own application, you will need to reference the Sage.SData.Client.dll in your
application. You can copy this DLL from the SData\bin folder in the virtual directory for the
SData portal. The default location is C:\inetpub\wwwroot\sdata\bin on the IIS Server where the
SData portal is deployed. For more information on how to create your own application, please
refer to Sage University's SData Master's class.

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)