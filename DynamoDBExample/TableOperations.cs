using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDBExample.Models;

namespace DynamoDBExample
{
    public static class TableOperations
    {
        // Creates a sample book item.
        public static void CreateBookItem(this AmazonDynamoDBClient client, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing CreateBookItem() ***");
            var book = new Book
            {
                Id = sampleBookId,
                Title = "Book " + sampleBookId,
                Price = 19.99,
                ISBN = "111-1111111111",
                BookAuthors = new List<string> { "Author 1", "Author 2", "Author 3" },
                PageCount = 500,
                Dimensions = "8.5x11x.5",
                InPublication = true,
                InStock = false,
                QuantityOnHand = 0
            };
            using (var context = new DynamoDBContext(client))
            {
                context.Save(book);

            }
        }

        public static void RetrieveBook(this Table productCatalog, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing RetrieveBook() ***");
            // Optional configuration.
            var config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "Id", "ISBN", "Title", "Authors", "Price" },
                ConsistentRead = true
            };
            var document = productCatalog.GetItem(sampleBookId, config);
            Console.WriteLine("RetrieveBook: Printing book retrieved...");
            PrintDocument(document, sampleBookId);
        }

        public static void UpdateMultipleAttributes(this Table productCatalog, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
            Console.WriteLine("\nUpdating multiple attributes....");
            var partitionKey = sampleBookId;

            var book = new Document
            {
                ["Id"] = partitionKey,
                ["Authors"] = new List<string> { "Author x", "Author y" },
                ["newAttribute"] = "New Value",
                ["ISBN"] = null
            };
            // List of attribute updates.
            // The following replaces the existing authors list.
            // Remove it.

            // Optional parameters.
            var config = new UpdateItemOperationConfig
            {
                // Get updated item in response.
                ReturnValues = ReturnValues.AllNewAttributes
            };
            var updatedBook = productCatalog.UpdateItem(book, config);
            Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
            PrintDocument(updatedBook, sampleBookId);
        }

        public static void UpdateBookPriceConditionally(this Table productCatalog, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing UpdateBookPriceConditionally() ***");

            var partitionKey = sampleBookId;

            var book = new Document
            {
                ["Id"] = partitionKey,
                ["Price"] = 29.99
            };

            // For conditional price update, creating a condition expression.
            var expr = new Expression
            {
                ExpressionStatement = "Price = :val",
                ExpressionAttributeValues = { [":val"] = 19.99 }
            };

            // Optional parameters.
            var config = new UpdateItemOperationConfig
            {
                ConditionalExpression = expr,
                ReturnValues = ReturnValues.AllNewAttributes
            };
            var updatedBook = productCatalog.UpdateItem(book, config);
            Console.WriteLine("UpdateBookPriceConditionally: Printing item whose price was conditionally updated");
            PrintDocument(updatedBook, sampleBookId);
        }

        public static void DeleteBook(this Table productCatalog, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing DeleteBook() ***");
            // Optional configuration.
            var config = new DeleteItemOperationConfig
            {
                // Return the deleted item.
                ReturnValues = ReturnValues.AllOldAttributes
            };
            var document = productCatalog.DeleteItem(sampleBookId, config);
            Console.WriteLine("DeleteBook: Printing deleted just deleted...");
            PrintDocument(document, sampleBookId);
        }

        public static void PrintDocument(Document updatedDocument, int sampleBookId)
        {
            foreach (var attribute in updatedDocument.GetAttributeNames())
            {
                string stringValue = null;
                var value = updatedDocument[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                                    in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
                Console.WriteLine("{0} - {1}", attribute, stringValue);
            }
        }
    }
}
