
# Persistify
                                                                                  
            ____                           _            __     _     ____             
           / __ \  ___    _____   _____   (_)   _____  / /_   (_)   / __/   __  __    
          / /_/ / / _ \  / ___/  / ___/  / /   / ___/ / __/  / /   / /_    / / / /    
         / ____/ /  __/ / /     (__  )  / /   (__  ) / /_   / /   / __/   / /_/ /     
        /_/      \___/ /_/     /____/  /_/   /____/  \__/  /_/   /_/      \__, /      
                                                                         /____/       
                                                                                      
Persistify is a powerful, yet simple database that stores JSON documents. It was developed as a final project at the Warsaw University of Technology. Built using ASP.NET Core gRPC, it is currently under development and expected to be ready for production in the near future.
## Features

Persistify excels at storing structured data. Although it stores the actual documents in the file system (or in another storage provider that may be implemented in the future), it keeps most of the search-related information in memory. This approach makes Persistify use more memory but allows it to provide blazing fast performance.

Understanding Persistify requires familiarity with two key concepts:
- **Document** - the JSON string itself.
- **Type** - the schema of the stored documents.

Currently, Persistify supports the following field types:
- **text**
- **number**
- **boolean**

A typical workflow using this database would be as follows:
- Login using username and password or a refresh token.
- Create a type.
- Index a document that matches the created type.
- Search for a document using a tree-like query.

A query in Persistify is a tree-like structure composed of two types of nodes:
- **Operators**: 'And', 'Or', and 'Not' (coming soon).
- **Queries**: 'Text', 'Number', and 'Boolean'.

**And Operator** - A node that has a collection of nodes. It returns a match if all nodes in the collection are evaluated as a match.

**Or Operator** - A node that has a collection of nodes. It returns a match if at least one node in the collection is evaluated as a match.

**Not Operator** - Coming soon.

All queries operate on a single field of a JSON that needs to be specified.

**Text Query** - A highly useful query that uses full text search to tokenize the provided string. It has two extra parameters:

- 'Exact': Determines if the token must exactly match another token in the document or just be a prefix.
- 'CaseSensitive': Determines whether the query is case-sensitive.

**Number Query** - Allows querying a range of numbers. To query an exact number, specify the minimum and maximum as that number.

**Boolean Query** - Allows querying the value of a field, either true or false.


## Typical use cases

Persistify was designed to allow the creation of an app using Persistify as the only database in the system. However, a more typical use case would involve using Persistify for its rapid read and full-text search capabilities. It can also be used as a cache layer.
## Roadmap

- Support for 'Not' operator.
- Support for '?' and '*' wildcards.
- Optimizations for serialization and memory efficiency.
- A full .NET client.
- A web app for monitoring traffic in the database.
## Stay tuned for more updates and features to come!
