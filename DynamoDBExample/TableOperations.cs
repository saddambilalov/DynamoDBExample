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

        public static void UpdateMultipleAttributes(this DynamoDBContext context, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing UpdateMultipleAttributes() ***");
            Console.WriteLine("\nUpdating multiple attributes....");
            var partitionKey = sampleBookId;

            var book = new Book
            {
                Id = partitionKey,
                BookAuthors = new List<string> { "Author x", "Author y" },
                ISBN = null
            };

            context.Save(book);
            Console.WriteLine("UpdateMultipleAttributes: Printing item after updates ...");
        }

        public static void UpdateBookPriceConditionally(this DynamoDBContext context, int sampleBookId)
        {
            Console.WriteLine("\n*** Executing UpdateBookPriceConditionally() ***");

            var partitionKey = sampleBookId;

            var book = context.Load<Book>(partitionKey);
            book.Price = 29.99;

            context.Save(book);
            Console.WriteLine("UpdateBookPriceConditionally: Printing item whose price was conditionally updated");
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
