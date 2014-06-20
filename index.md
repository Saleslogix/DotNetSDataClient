---
layout: default
title: DotNetSDataClient
---

DotNetSDataClient is a multi-platform .NET client library that simplifies consuming Saleslogix SData web services.

Features
--------
* Support for .NET 2.0, .NET 3.5, .NET 4.0, Windows 8, Windows Phone 8, Silverlight 5
* Read and write content in JSON and Atom format
* A LINQ provider supporting all the common operators except grouping and aggregation
* Serialization to and from POCOs, anonymous types, dictionaries and dynamic (DLR) objects
* Synchronous and asynchronous operations that can be used with the async/await keywords
* Differential updates via client side change tracking
* Automatic ETag concurrency handling
* High level $service, $batch and $schema APIs

Basic Example
-------------
The following example requests a list of active contacts ordered by last name then first name with addresses included using an asynchronous typed LINQ query:

```csharp
var client = new SDataClient("http://example.com/sdata/slx/dynamic/-/")
    {
        UserName = "admin",
        Password = "password"
    };
var contacts = await client.Query<Contact>()
    .Where(c => c.Status == "Active")
    .OrderBy(c => c.LastName)
    .ThenBy(c => c.FirstName)
    .Fetch(c => c.Address)
    .ToListAsync();

[SDataPath("contacts")]
public class Contact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Status { get; set; }
    public Address Address { get; set; }
}
```

License
-------
Swiftpage Partner Source License, full text available [here](https://raw.githubusercontent.com/Saleslogix/DotNetSDataClient/master/LICENSE).

Copyright 2014 Swiftpage ACT! LLC. All rights reserved.

Links
-----
* [GitHub project](https://github.com/Saleslogix/DotNetSDataClient/)
* [Documentation wiki](https://github.com/Saleslogix/DotNetSDataClient/wiki)
* [NuGet project page](https://www.nuget.org/packages/DotNetSDataClient/)
* [Issue tracker](https://github.com/Saleslogix/DotNetSDataClient/issues)
* [Saleslogix support forums](http://community.saleslogix.com/t5/Developer-Web-Discussions/bd-p/dev_web)
* [Saleslogix SData endpoints whitepaper](http://developer.saleslogix.com/DotNetSDataClient/whitepaper/)
* [SData specification](http://interop.sage.com/daisy/sdata/Introduction.html)
* [Download latest source](https://github.com/Saleslogix/DotNetSDataClient/zipball/master)