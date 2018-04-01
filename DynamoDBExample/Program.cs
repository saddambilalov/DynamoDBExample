using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

namespace DynamoDBExample
{
    public class Program
    {
        private const string TableName = "ProductCatalog";
        private const int SampleBookId = 555;

        private static void Main()
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    using (var context = new DynamoDBContext(client))
                    {
                        context.CreateBookItem(SampleBookId);
                        context.RetrieveBook(SampleBookId);

                        ////Couple of sample updates.
                        //productCatalog.UpdateMultipleAttributes(SampleBookId);
                        //productCatalog.UpdateBookPriceConditionally(SampleBookId);

                        ////Delete
                        context.DeleteBook(SampleBookId);
                    }
                }

                Console.WriteLine("To continue, press Enter");
                Console.ReadLine();
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }


    }
}