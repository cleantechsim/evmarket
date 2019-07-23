

using System;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;

namespace CleanTechSim.MainPage.Helpers.Storage.AzureTableStorage
{

    public class AzureTableStorage : IDataStorage
    {
        private readonly CloudTableClient client;
        private readonly ReflectionMapper mapper;

        public AzureTableStorage(string connectionString, params Type[] types)
            : this(connectionString, new List<Type>(types))

        {

        }

        public AzureTableStorage(string connectionString, IEnumerable<Type> types)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            this.client = storageAccount.CreateCloudTableClient();

            this.mapper = new ReflectionMapper(types);
        }


        public IEnumerable<T> GetAll<T>(Type type)
        {

            string tableName = ReflectionMapper.GetTableName(type);

            CloudTable table = client.GetTableReference(tableName);

            TableQuery query = new TableQuery();

            EntityResolver<T> resolver = (partitionKey, rowKey, timestamp, properties, eTag) =>
            {
                return mapper.Convert<T>(type, partitionKey, rowKey, properties);
            };

            IEnumerable<T> results = table.ExecuteQuery(query, resolver);

            // Trigger to get all at once by ToArray()
            return results.ToArray();
        }
    }
}

