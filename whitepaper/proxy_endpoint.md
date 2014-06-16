---
layout: default
title: Proxy Endpoint
back: gcrm_endpoints
next: scheduling_endpoints
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The proxy endpoint provides access to an external SData feed by looking up the necessary
authentication information and making the connection to the feed for you. Information the proxy
endpoint uses to access the external feeds is stored in the `APPIDMAPPING` table. For more
information on how to add external web service information, see "Using an external feed with an
Editable Grid Control - Example" in the Sage SalesLogix Application Architect Help system.

### Background ###
The `APPIDMAPPING` table in the Sage SalesLogix database contains the information that is
needed for the proxy endpoint to access external SData feeds. Adding the information for
external feeds can be done by logging into the Sage SalesLogix Web Client as admin. The
table contains the appId, name, endpointUrl, username, and password for accessing the feed.

### Sample Calls ###
Query an external feed for a list of products:

    /slx/proxy/{appid}/products

Where {appid} is the ID of a record in the `APPIDMAPPING` table.

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)