---
layout: default
title: Service Operations
back: system_useroptions
next: metadata_endpoints
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Dynamic Adapter Business Rules ###
SData supports the use of Business Rules in service operations.

A business rule is used to define the constraints that apply to an entity within a package. For
more information on Business Rules, including adding or editing business rules, see the
Application Architect Help. Many business rules defined for entities can be executed as service
calls. You can see what is available for an entity by browsing to `/slx/dynamic/-/{resourcekind}/$service`.
Not all the business rules for an entity are necessarily in this list.

### Sample Calls ###
List of Business Rules for entity:

    /slx/dynamic/-/accounts/$service

GET templates:

    /slx/dynamic/-/activities/$service/Complete/$template

POST:

    /slx/dynamic/-/activities/$service/Complete

### Code Samples Using SData Client Libraries ###

**Complete an activity and display the History ID**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var service = new
    {
        Request = new
            {
                entity = new SDataResource {Key = "VDEMOA00007A"},
                userId = "UDEMOA000002",
                result = "Complete",
                completeDate = new DateTime(2014, 1, 1)
            },
        Response = new
            {
                HistoryId = default(string)
            }
    };
service = await client.PostAsync(service, "activities/$service/Complete");
Console.WriteLine(service.Response.HistoryId);
```

To get the ResourceName property for your request payload, enter the GET template sample call
example using the entity name. Then, from the browser view the page source. Look for the
`<sdata:payload>` tag in the HTML. The next line will be the ResourceName and Namespace values.

    <sdata:payload>
      <slx:ActivityComplete xmlns:slx="http://schemas.sage.com/dynamic/2007">
        <slx:request>
          <slx:entity xsi:nil="true" />
          <slx:ActivityId xsi:nil="true" />
          <slx:userId xsi:nil="true" />
          <slx:result xsi:nil="true" />
          <slx:resultCode xsi:nil="true" />
          <slx:completeDate>0001-01-01T00:00:00+00:00</slx:completeDate>
        </slx:request>
        <slx:response xsi:nil="true" />
      </slx:ActivityComplete>
    </sdata:payload>

### System Adapter Services ###
SData supports service operations. Service operations are operations that do not fit naturally
into the CRUD model. For more information about service calls, see
[http://interop.sage.com/daisy/sdata/ServiceOperations.html](http://interop.sage.com/daisy/sdata/ServiceOperations.html)

The list below shows the service calls available through the System adapter.

- applyFilterToEntity
- applyFilterToGroup
- executeSearch
- getCurrentUser
- getEntityFilters
- getGroupContext
- getHiddenFilters
- getPicklistsChangeState
- setGroupContext
- setGroupLookupConditions
- setHiddenFilters

### GetCurrentUser ###
The getCurrentUser service operation is a service that returns the userId, userName, and
prettyName properties of the currently authenticated portal user via IUserService. In cases
where the Sage SalesLogix user is different than the login credentials, the getCurrentUser
service operation can be used to retrieve the current user ID needed by service calls to post the
correct data.

### Code Samples Using SData Client Libraries ###

**Retrieve the name of the current user**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var results = await client.ExecuteAsync<SDataResource>(new SDataParameters
    {
        Method = HttpMethod.Post,
        Path = "$service/getCurrentUser",
        Content = new {},
        ContentType = MediaType.Json
    });
var user = (SDataResource) results.Content["response"];
Console.WriteLine("User ID: " + user["userId"]);
Console.WriteLine("User Name: " + user["userName"]);
Console.WriteLine("Pretty Name: " + user["prettyName"]);
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)