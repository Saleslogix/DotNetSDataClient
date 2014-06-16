---
layout: default
title: Supported Functions
back: scheduling_endpoints
next: index
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

Sage SalesLogix supports the following functions in the SData specification for use in the SData
queries. Refer to [http://interop.sage.com/daisy/sdata/AnatomyOfAnSDataURL/QueryLanguage](http://interop.sage.com/daisy/sdata/AnatomyOfAnSDataURL/QueryLanguage)
for more information.

| String Function | Example Query                                                         |
|-----------------|-----------------------------------------------------------------------|
| concat          | /slx/dynamic/-/accounts?where=concat(Status,Type) eq 'ActiveCustomer' |
| left            | /slx/dynamic/-/accounts?where=left(AccountName,2) eq 'Co'             |
| length          | /slx/dynamic/-/accounts?where=length(AccountName) eq 11               |
| lower           | /slx/dynamic/-/accounts?where=lower(Type) eq 'customer'               |
| right           | /slx/dynamic/-/accounts?where=right(AccountName,2) eq 'es'            |
| substring       | /slx/dynamic/-/accounts?where=substring(AccountName,3,2) eq 'te'      |
| upper           | /slx/dynamic/-/accounts?where=upper(Type) eq 'CUSTOMER'               |

| Numeric Function | Example Query                                           |
|------------------|---------------------------------------------------------|
| abs              | /slx/dynamic/-/accounts?where=abs(Revenue) lt 1000      |
| floor            | /slx/dynamic/-/accounts?where=floor(Revenue) lt 1000    |
| round            | /slx/dynamic/-/accounts?where=round(Revenue,-3) lt 1000 |
| sign             | /slx/dynamic/-/accounts?where=sign(Revenue) eq -1       |
| trim             | /slx/dynamic/-/accounts?where=trim(Type) eq 'Customer'  |

| Date Function    | Example Query                                                      |
|------------------|--------------------------------------------------------------------|
| currentTimestamp | /slx/dynamic/-/accounts?where=currentTimestamp() gt ModifyDate     |
| dateAdd          | /slx/dynamic/-/accounts?where=dateAdd(ModifyDate, 2) gt CreateDate |
| dateSub          | /slx/dynamic/-/accounts?where=dateSub(ModifyDate, 2) lt CreateDate |
| day              | /slx/dynamic/-/accounts?where=day(ModifyDate) eq 3                 |
| hour             | /slx/dynamic/-/accounts?where=hour(ModifyDate) eq 21               |
| minute           | /slx/dynamic/-/accounts?where=minute(ModifyDate) eq 9              |
| month            | /slx/dynamic/-/accounts?where=month(ModifyDate) eq 7               |
| second           | /slx/dynamic/-/accounts?where=second(ModifyDate) eq 15             |
| year             | /slx/dynamic/-/accounts?where=year(ModifyDate) eq 2012             |

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)