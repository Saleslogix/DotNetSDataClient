---
layout: default
title: GCRM Endpoints
back: mashup_endpoints
next: proxy_endpoint
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The Global CRM (GCRM) contract is a common interface between CRM products and ERP
products in Sage. The aim of the contract is to enable ERP teams to develop one integration
solution which works with Sage ACT!, Sage SalesLogix, and Sage CRM. The contract defines
a common set of functionality that is specifically designed to ease the development and running
of point-to-point integrations with ERP products.

Provided the specification is adhered to it is possible to synchronize trading accounts,
customer, supplier, sales orders, sales quotations, commodity and other ERP information with
CRM to a reasonably sophisticated level without significant additional customization or coding.

Sage SalesLogix provides GCRM endpoints so that it can synchronize information to an ERP
system. This will allow for fast, accurate updates between Sage SalesLogix and your
accounting program. You can view a list of the provided endpoints by browsing to
`http://servername:port/sdata/slx/gcrm/-/`.

For more information on the GCRM contract and how you can take advantage of it, please see
[http://interop.sage.com/daisy/SGCRMContract/274-DSY.html](http://interop.sage.com/daisy/SGCRMContract/274-DSY.html).

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)