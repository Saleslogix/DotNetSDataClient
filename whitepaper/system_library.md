---
layout: default
title: Library System Endpoint
back: system_groups
next: system_picklists
---

# Saleslogix SData Endpoints Whitepaper #

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)

---

## {{ page.title }} ##

### Purpose ###
The Library system endpoints provide GET, POST, PUT, and DELETE access to the Sage
SalesLogix Library. The endpoints allow for filtering, sorting, and paging of the Library. The
libraryDirectories endpoint contains the directories information while the libraryDocuments
endpoint provides information about the library documents.

### Background ###
The Library is a central repository for company information. It is used to provide the latest
information to Sage SalesLogix users and to distribute information to Remote users. For more
information, refer to the "Library" help topic in the Sage SalesLogix Administrator and Web
Client help systems.

The Library is managed in two tables, `LIBRARYDIRS` and `LIBRARYDOCS`. The `LIBRARYDIRS`
table stores the location (path) in relationship to the "Library" folder. (The path to the Library
folder is stored in the `BRANCHOPTIONS` table.)

The `LIBRARYDOCS` table stores information about properties for each document or Web
address URL that is in a Sage SalesLogix Library folder. Documents and URLs are added to
the Library folder using the Administrator. The information includes file name, directory ID of the
folder, description, available status, revision date, expiration date, file size, and a flag is set if
the file can be sent to Remotes. The `ABSTRACT` column can contain up to a 1024-character comment
about the file.

### Endpoint Segments ###
| Component      | Name              | Purpose                    | Example                        |
|----------------|-------------------|----------------------------|--------------------------------|
| /libraryDirectories | Resource kind | Returns all library directories |                          |
| ('id')         | Resource selector | Returns selected directory | /libraryDirectories('DDEMOA00000Y') |
| /libraryDocuments | Resource kind  | Returns all library documents |                             |
| ('id')         | Resource selector | Returns selected document  | /libraryDocuments('dDEMOA000010') |
| where          |                   | Filters feed results       | /libraryDirectories?where=fullPath eq '\\\\Policies\\\\Travel' (note escaped backslashes) /libraryDirectories(fullPath eq '\\\\Policies\\\\Travel')/documents |
| select         |                   | Selects the properties that will be returned in the payload. | |
| include        |                   | Includes subdirectory      | /libraryDocuments?include=directory |

### Errors and Warnings ###
Requesting a library directory or document that does not exist generates one of the following:

- If a where condition is used in the SData query, the endpoint will return an empty
feed.
- If a predicate is used in the SData query, the endpoint will return an error with
diagnosis.

All the standard SData errors apply (for example, malformed URLs or payloads and missing `If-Match`
header during updates.)

### Sample Calls ###
List all documents:

    /slx/system/-/libraryDocuments

List all directories:

    /slx/system/-/libraryDirectories

Get a directory by its full path:

    /slx/system/-/libraryDirectories?where=fullPath eq '\\Policies\\Travel'

Notice that the backslashes are doubled up since backslash is the special escape
sequence character.

    /slx/system/-/libraryDirectories(fullPath eq '\\Policies\\Travel')/documents

Get a document and include the directory it is in:

    /slx/system/-/libraryDocuments('dDEMOA000010')?include=directory

Get a directory including all the documents it contains:

    /slx/system/-/libraryDirectories('DDEMOA00000Y')?include=documents

### Code Samples Using SData Client Libraries ###

**Build a tree view with directory and document nodes and display the selected item in a property grid**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var treeView = new TreeView {Dock = DockStyle.Fill};
var propGrid = new PropertyGrid {Dock = DockStyle.Fill};
var expand = new Action<string, TreeNodeCollection>(
    (id, nodes) =>
    {
        var directories = client.Query("libraryDirectories")
            .Where(x => x["parentId"] == id)
            .OrderBy(x => x["directoryName"])
            .Fetch(x => x["documents"])
            .ToList();
        foreach (var dir in directories)
        {
            var dirName = (string) dir["directoryName"];
            var node = new TreeNode(dirName)
                {
                    Tag = dir,
                    Nodes = {new TreeNode()}
                };
            nodes.Add(node);
        }
    });
expand("0", treeView.Nodes);
treeView.BeforeExpand +=
    (sender, e) =>
    {
        if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag == null)
        {
            var dir = (SDataResource) e.Node.Tag;
            e.Node.Nodes.Clear();
            expand(dir.Key, e.Node.Nodes);
            var documents = (IEnumerable<SDataResource>) dir["documents"];
            foreach (var file in  documents)
            {
                var fileName = (string) file["fileName"];
                e.Node.Nodes.Add(new TreeNode(fileName)
                    {
                        Tag = file,
                        ForeColor = Color.Blue
                    });
            }
        }
    };
treeView.AfterSelect += (sender, e) => propGrid.SelectedObject = e.Node.Tag;
new Form
    {
        Controls =
            {
                new SplitContainer
                    {
                        Dock = DockStyle.Fill,
                        Panel1 = {Controls = {treeView}},
                        Panel2 = {Controls = {propGrid}}
                    }
            }
    }.ShowDialog();
```

**Upload a library document using the file endpoint and form-data**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var stream = new MemoryStream(Encoding.UTF8.GetBytes("Bar"));
await client.ExecuteAsync(new SDataParameters
    {
        Method = HttpMethod.Post,
        Path = "libraryDocuments/file",
        Form = {{ "{{" }}"directoryId", "DDEMOA00000Y"}},
        Files = {new AttachedFile("text/plain", "Foo.txt", stream)}
    });
```

**Upload a library document using the file endpoint with the directory specified in the URL**

```csharp
var client = new SDataClient(BaseUri + "slx/system/-")
    {
        UserName = UserName,
        Password = Password
    };
var stream = new MemoryStream(Encoding.UTF8.GetBytes("Bar"));
await client.ExecuteAsync(new SDataParameters
    {
        Method = HttpMethod.Post,
        Path = "libraryDirectories('DDEMOA00000Y')/documents/file",
        Files = {new AttachedFile("text/plain", "Foo.txt", stream)}
    });
```

---

[< Back]({{ page.back }}.html) | [Home](index.html) | [Next >]({{ page.next }}.html)