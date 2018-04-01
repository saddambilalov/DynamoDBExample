using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace DynamoDBExample
{
    public class Program
    {
        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient();
        private const string TableName = "ProductCatalog";
        // The sample uses the following id PK value to add book item.
        private const int SampleBookId = 555;

        private static void Main()
        {
            try
            {
                //new LowLevelTable(Client, TableName).CreateExampleTable();
                var productCatalog = Table.LoadTable(Client, TableName);

                //Create
                productCatalog.CreateBookItem(SampleBookId);
                productCatalog.RetrieveBook(SampleBookId);

                //Couple of sample updates.
                productCatalog.UpdateMultipleAttributes(SampleBookId);
                productCatalog.UpdateBookPriceConditionally(SampleBookId);

                //Delete
                productCatalog.DeleteBook(SampleBookId);

                Console.WriteLine("To continue, press Enter");
                Console.ReadLine();
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }


    }
}