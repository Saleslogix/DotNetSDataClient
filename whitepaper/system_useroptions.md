---
layout: default
title: UserOptions System Endpoint
back: system_systemoptions
next: service_operations
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The userOptions system endpoint provides GET, POST, PUT, and DELETE access to the
userOptions attributes in both Atom and JSON/BSON formats. The endpoint also supports
standard payload options such as filtering, sorting, and paging.

The userOptions system endpoint uses the special composite key format to request items.
Short and normal key formats are supported in literal predicates. For example,
/userOptions('General;Currency') is equivalent to
/userOptions('Category=General;Name=Currency'). For security reasons, users can access
only their own user options through this endpoint.

### Background ###
Sage SalesLogix provides tools to allow users to modify some parts of the interface so that they
can work in the way that suits them best. The Sage SalesLogix Web Client User Options allow
users to set specific preferences for such features as the view that appears when they start the
Sage SalesLogix Web Client and their Activity Alerts. The settings that a user enters are
recorded against the authenticated user and are not visible to other Sage SalesLogix users. For
more information on user options, refer to the help topic, "Editing User Options" in the Sage
SalesLogix Web Client help system.

As you create your own customizations, you may identify additional user preferences that you
want to persist. For example, you may want to remember the default tab for a new form or when
a user turns off future display of a particular message box. You can add custom user options to
track this information.

User options are managed in two tables, `USEROPTIONSDEF` and `USEROPTIONS`. The
`USEROPTIONDEF` table stores options, definitions, and default values. The name of the option
in combination with the option category provides the primary key and must be unique. You can
see which user options are stored by browsing the Sage SalesLogix-defined options in the
Sage SalesLogix database.

The `USEROPTIONS` table stores values that override the default values for individual users. In
order for a value to exist in userOptions, a corresponding definition of the option should exist in
the `USEROPTIONDEF` table. Queries are based on the `USEROPTIONDEF` table so "dangling"
user options (which only exist in `USEROPTIONS`) are not exposed.

### Endpoint Segments ###
| Component         | Name              | Purpose                 | Example                        |
|-------------------|-------------------|-------------------------|--------------------------------|
| /userOptions      | Resource kind     | Returns all user options for logged in user |            |
| ('category;name') | Resource selector | Selects a specific user option for logged in user | /userOptions('General;Currency') or /userOptions ('category=General;name=Currency') |
| where             |                   | Filters feed results    | /userOptions?where=category eq 'General' |
| select            |                   | Selects the properties that will be returned in the payload. Properties are category, name, displayName, defaultValue, value, and locked. | /userOptions?select=value |

### Errors and Warnings ###
Requesting a user option that does not exist generates one of the following:

- If a where condition is used in the SData query, the endpoint will return an empty
feed.
- If a predicate is used in the SData query, the endpoint will return an error with
diagnosis.

All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match
header during updates.)

### Sample Calls ###
List all options available for the logged in user:

    /slx/system/-/userOptions

List all options for a specific category:

    /slx/system/-/userOptions?where=category eq 'General'

Get a specific option using the short key format:

    /slx/system/-/userOptions('General;Currency')

Or any predicate expression, for example:

    /slx/system/-/userOptions(category eq 'General' and name eq 'Currency')

Get a specific option using the normal key format:

    /slx/system/-/userOptions('category=General;name=Currency')

Get a specific option using an expression:

    /slx/system/-/userOptions(category eq 'General' and name eq 'Currency')

Get just the value of a specific user option:

    /slx/system/-/userOptions('category=General;name=Currency')?select=value

### Code Samples Using SData Client Libraries ###

**Create a new user option**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var userOption = new
    {
        category = "_Category",
        name = "_Name",
        displayName = "_DisplayName",
        defaultValue = "_DefaultValue",
        value = "_Value",
        locked = true,
    };
await client.PostAsync(userOption, "userOptions");
```

**Update an existing user option**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var userOption = await client.GetAsync("category=_Category;name=_Name", "userOptions");
userOption["displayName"] += "_CHANGED";
userOption["defaultValue"] += "_CHANGED";
userOption["value"] += "_CHANGED";
userOption["locked"] = false;
await client.PutAsync(userOption, "userOptions");
```

**Delete an existing user option**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var userOption = await client.GetAsync("category=_Category;name=_Name", "userOptions");
await client.DeleteAsync(userOption, "userOptions");
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)