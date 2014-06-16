---
layout: default
title: SystemOptions System Endpoint
back: system_speedsearch
next: system_useroptions
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The systemOptions system endpoint provides Read-Only access to system level information
such as whether multi-currency or Unicode is enabled.

### Background ###
System level information such as Company Name and database version is stored in the
`SYSTEMINFO` table in the Sage SalesLogix database. It also contains a blob column that holds
information such as default password, database admin ID, and current remote password. The
systemOptions system endpoint returns information from this table. For security, the
systemOptions endpoint is Read-Only.

### Errors and Warnings ###
All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all information:

    /slx/system/-/systemOptions

List select information:

    /slx/system/-/systemOptions?select=name,value

Get company name:

    /slx/system/-/systemOptions('CompanyName')

Get company name and version of database:

    /slx/system/-/systemOptions?where=name eq 'CompanyName' or name eq 'DatabaseVersion'

### Code Samples Using SData Client Libraries ###

**Retrieve a system option value**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var option = await client.GetAsync("DbType", "systemOptions");
Console.WriteLine(option["value"]);
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)