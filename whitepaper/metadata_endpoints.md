---
layout: default
title: Metadata Endpoints
back: service_operations
next: mashup_endpoints
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
Use these endpoints to read, write, change, and delete Entity model information.

### Endpoints ###

- dataTypes
- entities
- forms
- formUsages
- packages
- portalPages
- relationships

### Background ###
The entities endpoint provides CRUD access to entity information such as table name,
package, properties, and filters. Its filters sub-endpoint provides CRUD access to the filters of a
specified entity. Filters CRUD is also supported at the top level via nested payloads.

### Endpoint Segments ###
| Component | Name                    | Purpose            | Example                               |
|-----------|-------------------------|--------------------|---------------------------------------|
| /entities | Resource Collection URL | Returns a collection of entities (default limit = 10) |    |
| ('name')  | Resource selector       | Returns selected entity | /entities('account') Do not use predicate expressions except in the where syntax |
| /filters  | Relationship property   | Returns filter metadata of selected entity without the other entity attributes | /entities('account')/filters |
| where     |                         | Filters feed results | /entities?where=(package eq 'SalesLogix Application Entities') /entities?where=isDynamic |
| select    |                         | Selects the properties that will be returned in the payload. Properties are id, package, name, tableName, displayName, displayNamePlural, isDynamic, properties, validationRules | /entities?select=package,name |
| include   |                         |                    | /entities('account')?include=validationRules /entities('account')?include=filters |

### Errors and Warnings ###
- If you request an entity that does not exist, you get an empty feed.
- All the standard SData errors apply (for example, malformed URLs or payloads and
missing `If-Match` header during updates.)
- The filters endpoint will raise an error if the polymorphic "details" property is not
specified during a POST or it is changed during a PUT. This property is required so
the appropriate filter type can be created on the server. The filter type cannot be
changed once it has been created for technical reasons.

### Sample Calls ###
Get schema on entities:

    /slx/metadata/-/entities/$schema

Get all entities:

    /slx/metadata/-/entities?count=200

Returns first 10 unless a count given

Get specific entity using a resource selector: (must use entity name, does not accept predicate expression):

    /slx/metadata/-/entities('Account')

Filter entities by name:

    /slx/metadata/-/entities?where=name like 'E%'
    /slx/metadata/-/entities?where=name eq 'Account'

This endpoint is case sensitive, so 'account' will not work here.

Get all entities belonging to package 'Process Orchestration Support':

    /slx/metadata/-/entities?where=package.name eq 'Process Orchestration Support'

Get package of Account entity:

    /slx/metadata/-/entities('Account')?include=package

Notice this expands the package resource within the entity. You will see the 'name'
element for the package is included, SalesLogix Application Entities
Get the package of Account entity. Remove all properties by taking advantage of precedence:

    /slx/metadata/-/entities('Account')?include=package&_includeContent=false&precedence=1

Get the properties of the Ticket entity:

    /slx/metadata/-/entities('Ticket')/properties

### Sample calls for filters sub-endpoint ###

List all filters for the Account entity:

    /slx/metadata/-/entities('Account')/filters

List all "checked list" style filters for the Account entity:

    /slx/metadata/-/entities('Account')/filters?where=filterType eq 'checkedList'

Fetch a single account filter by its ID:

    /slx/metadata/-/entities('Account')/filters('d7652a611922480a80ea77a69c993d6c')

### Code Samples Using SData Client Libraries ###

**Create a distinct filter**

```csharp
var client = new SDataClient(BaseUri + "slx/metadata/-")
    {
        UserName = UserName,
        Password = Password
    };
var filter = new
    {
        filterName = "_test_distinct",
        propertyName = "Employees",
        details = new
            {
                distinctFilter = new
                    {
                        characters = 1
                    }
            }
    };
await client.PostAsync(filter, "entities('Account')/filters");
```

**Create a range filter**

```csharp
var client = new SDataClient(BaseUri + "slx/metadata/-")
    {
        UserName = UserName,
        Password = Password
    };
var filter = new
    {
        filterName = "_test_range",
        propertyName = "Employees",
        details = new
            {
                rangeFilter = new
                    {
                        characters = 2,
                        ranges = SDataCollection.Create(true, new[]
                            {
                                new
                                    {
                                        rangeName = "0-9",
                                        lower = 0,
                                        upper = 9
                                    }
                            })
                    }
            }
    };
await client.PostAsync(filter, "entities('Account')/filters");
```

**Create a user lookup filter**

```csharp
var client = new SDataClient(BaseUri + "slx/metadata/-")
    {
        UserName = UserName,
        Password = Password
    };
var filter = new
    {
        filterName = "_test_lookup",
        propertyName = "Employees",
        details = new
            {
                userLookupFilter = new
                    {
                        operators = new[]
                            {
                                "Contains",
                                "GreaterThan"
                            }
                    }
            }
    };
await client.PostAsync(filter, "entities('Account')/filters");
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)