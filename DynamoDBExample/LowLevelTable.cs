using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBExample
{
    public static class LowLevelTable
    {
        public static void CreateExampleTable(this AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine("\n*** Creating table ***");
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition
                {
                    AttributeName = "Id",
                    AttributeType = "N"
                },
                new AttributeDefinition
                {
                    AttributeName = "PageCount",
                    AttributeType = "N"
                }
            },
                KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Id",
                    KeyType = "HASH" //Partition key
                },
                new KeySchemaElement
                {
                    AttributeName = "PageCount",
                    KeyType = "RANGE" //Partition key
                }
            },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 6
                },
                TableName = tableName
            };

            var response = client.CreateTable(request);

            var tableDescription = response.TableDescription;
            Console.WriteLine("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
                      tableDescription.TableStatus,
                      tableDescription.TableName,
                      tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                      tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            string status = tableDescription.TableStatus;
            Console.WriteLine(tableName + " - " + status);

            client.WaitUntilTableReady(tableName);
        }

        public static void ListMyTables(this AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine("\n*** listing tables ***");
            string lastTableNameEvaluated = null;
            do
            {
                var request = new ListTablesRequest
                {
                    Limit = 2,
                    ExclusiveStartTableName = lastTableNameEvaluated
                };

                var response = client.ListTables(request);
                foreach (string name in response.TableNames)
                    Console.WriteLine(name);

                lastTableNameEvaluated = response.LastEvaluatedTableName;
            } while (lastTableNameEvaluated != null);
        }

        public static void GetTableInformation(this AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine("\n*** Retrieving table information ***");
            var request = new DescribeTableRequest
            {
                TableName = tableName
            };

            var response = client.DescribeTable(request);

            TableDescription description = response.Table;
            Console.WriteLine("Name: {0}", description.TableName);
            Console.WriteLine("# of items: {0}", description.ItemCount);
            Console.WriteLine("Provision Throughput (reads/sec): {0}",
                      description.ProvisionedThroughput.ReadCapacityUnits);
            Console.WriteLine("Provision Throughput (writes/sec): {0}",
                      description.ProvisionedThroughput.WriteCapacityUnits);
        }

        public static void UpdateExampleTable(this AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine("\n*** Updating table ***");
            var request = new UpdateTableRequest()
            {
                TableName = tableName,
                ProvisionedThroughput = new ProvisionedThroughput()
                {
                    ReadCapacityUnits = 6,
                    WriteCapacityUnits = 7
                }
            };

            client.UpdateTable(request);

            client.WaitUntilTableReady(tableName);
        }

        public static void DeleteExampleTable(this AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine("\n*** Deleting table ***");
            var request = new DeleteTableRequest
            {
                TableName = tableName
            };

            client.DeleteTable(request);

            Console.WriteLine("Table is being deleted...");
        }

        private static void WaitUntilTableReady(this AmazonDynamoDBClient client, string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = client.DescribeTable(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    Console.WriteLine("Table name: {0}, status: {1}",
                              res.Table.TableName,
                              res.Table.TableStatus);
                    status = res.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
    }
}