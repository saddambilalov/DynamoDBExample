using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace DynamoDBExample
{
    public class Program
    {
        private const string TableName = "ProductCatalog";

        private static void Main()
        {
            try
            {
                using (var client = new AmazonDynamoDBClient())
                {
                    //client.CreateExampleTable(TableName);

                    using (var context = new DynamoDBContext(client))
                    {
                        // Sample queries.
                        context.FindRepliesInLast3000Days("Amazon DynamoDB", "DynamoDB Thread 1");

                        //context.RetrieveBooksdWithinQuery("Id");
                        for (var index = 1; index <= 2; index++)
                        {
                            // context.CreateBookItem(index);
                            //context.RetrieveBook(index);

                            //Couple of sample updates.
                            //context.UpdateMultipleAttributes(index);
                            //context.UpdateBookPriceConditionally(index);

                            //Delete
                            //context.DeleteBook(index);
                        }
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