﻿using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Tests.Utilities
{
    internal class DatabaseFactory
    {
        internal static Database ReadOrCreateDatabase(DocumentClient client, string databaseId)
        {
            var databases = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).ToArray();

            if (databases.Any())
            {
                return databases.First();
            }

            var database = new Database {Id = databaseId};
            return client.CreateDatabaseAsync(database).Result;
        }

        internal static DocumentCollection ReadOrCreateCollection(DocumentClient client, string databaseLink, string collectionId)
        {
            var collections = client.CreateDocumentCollectionQuery(databaseLink).Where(col => col.Id == collectionId).ToArray();

            if (collections.Any())
            {
                return collections.First();
            }

            var collection = new DocumentCollection {Id = collectionId};
            return client.CreateDocumentCollectionAsync(databaseLink, collection).Result;
        }
    }
}