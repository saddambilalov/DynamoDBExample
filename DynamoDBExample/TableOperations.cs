using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DynamoDBExample.Models;

namespace DynamoDBExample
{
    public static class TableOperations
    {
        // Creates a sample book item.
        public static void CreateBookItem(this DynamoDBContext context, int sampleBookId)
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

            context.Save(book);
        }

        public static void RetrieveBook(this DynamoDBContext context, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing RetrieveBook() ***");
            // Optional configuration.
            var config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "Id", "ISBN", "Title", "Authors", "Price" },
                ConsistentRead = true
            };

            var book = context.Load<Book>(sampleBookId, config);
            Console.WriteLine("RetrieveBook: Printing book retrieved...");
            PrintDocument(book);
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
            //PrintDocument(updatedBook);
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
            //PrintDocument(updatedBook);
        }

        public static void DeleteBook(this DynamoDBContext context, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing DeleteBook() ***");
            // Optional configuration.
            var config = new DeleteItemOperationConfig
            {
                // Return the deleted item.
                ReturnValues = ReturnValues.AllOldAttributes
            };
            context.Delete<Book>(sampleBookId, config);
            Console.WriteLine("DeleteBook: Printing deleted just deleted...");
        }

        public static void PrintDocument(Book book)
        {
            foreach (var prop in book.GetType().GetProperties())
            {
                string stringValue = null;
                var value = prop.GetValue(book, null);

                stringValue = value is IList && value.GetType().IsGenericType
                    ? string.Join(", ", ((List<string>)value).Select(primitive => primitive).ToArray())
                    : value?.ToString();

                Console.WriteLine("{0} - {1}", prop.Name, stringValue);
            }
        }
    }
}
