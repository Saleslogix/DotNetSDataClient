---
layout: default
title: Security System Endpoint
back: system_resources
next: system_speedsearch
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
Use these endpoints to manage security profile information. Use the securityProfiles endpoint
for a profile-centric view point. For example, you could use it to change the default access for
the Sales profile or find out what columns can be edited. For a column-centric view, use the
securityProfileColumns endpoint. For example, for the `CREDITRATING` column, you could find out
what security profiles can view it.

### Background ###
SalesLogix combines record ownership with security profile information to implement field level
security. For more information on the implementation of field level security, refer to the
Administrator help.

Security profile information is maintained in two tables, `SECPROFILE` and `SECTABLEDEFS`.

- **SECPROFILE**: Contains information on field level security profiles that can be
assigned to users. Security profiles are created in the Administrator using the Security
Profile Manager. Each profile record contains a data blob that encodes the permission
or access for every column for which security is being managed. To look up a profile's
access for a particular column, you must know its position in the blob. All records have
the same length blob within a particular SalesLogix database.
- **SECTABLEDEFS**: Defines all tables and fields in the Sage SalesLogix database.
There is one record for every column in every table. Two of the things each record
specifies about its column are: 1) whether security is being managed for the column,
and 2) if so, the offset into the blob.

The process for extracting the access level for a particular security profile for a particular
column involves looking up the offset into the blob from `SECTABLEDEFS`, and then using the
offset to get the actual access (READONLY, READWRITE, or NOACCESS) from the blob in
`SECPROFILE`. The security endpoints eliminate the need to for you to do these complex
calculations yourself.

### Endpoint Segments ###
| Component         | Name              | Purpose                 | Example                        |
|-------------------|-------------------|-------------------------|--------------------------------|
| /securityProfiles | Resource collection URL | Returns a collection of security profiles |        |
| ('id')            | Resource selector | Returns selected security profile | /securityProfiles('PROF00000001') |
| /securityProfileColumns | Resource collection URL | Returns a list of access information for a collection of columns. | |
| ('id;table;column') | Resource selector | Returns all the information on a single table column. | /securityProfileColumns ('PROF00000001;ACCOUNT;ACCOUNT') |
| where             |                   | Filters feed results. Used on any payload element except `access` | /securityProfiles?where=(defaultPermission eq 'W') |
| select            |                   | Selects the properties that will be returned in the payload. See the payload elements in the following table. | /securityProfiles?select=profileDescription /securityProfileColumns?select=entity,property,access |
| include           |                   | Expands the columns children in the payload. | /securityProfiles('PROF00000001')?include=columns |

### Errors and Warnings ###
- If you request a profile that does not exist, you get an empty feed.
- All the standard SData errors apply (for example, malformed URLs or payloads and
missing `If-Match` header during updates.)

### Sample calls ###
Get all security profiles:

    /slx/system/-/securityProfiles

Select a single profile with a resource selector of profile ID:

    /slx/system/-/securityProfiles('PROF00000001')

Get the security profile named Sales:

    /slx/system/-/securityProfiles?where=(profileDescription eq 'Sales')

Expand the columns of the profile within the payload:

    /slx/system/-/securityProfiles('PROF00000001')?include=columns

Filter profiles by profile type User:

    /slx/system/-/securityProfiles?where=profileType eq 'U'

Get first 100 security profile columns:

    /slx/system/-/securityProfileColumns

Get the access info for all profiles for Account.CreditRating. Include the profile information:

    /slx/system/-/securityProfileColumns?where=entity eq 'Account' and property eq 'CreditRating'&include=profile

Get the access info for the "PROF00000001" profile for the `ACCOUNT` column in the `ACCOUNT` table:

    /slx/system/-/securityProfileColumns('PROF00000001;ACCOUNT;ACCOUNT')

Get the column entries that belong to the Read Only Default profile:

    /slx/system/-/securityProfileColumns?where=profile.profileDescription eq 'Read Only Default'

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)