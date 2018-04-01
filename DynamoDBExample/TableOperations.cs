using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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
                AttributesToGet = new List<string> { "Id", "Title" },
                ConsistentRead = true
            };

            var book = context.Load<Book>(sampleBookId, config);
            Console.WriteLine("RetrieveBook: Printing book retrieved...");
            PrintDocument(book);
        }

        public static void RetrieveBooksPricedLessThanThirty(this DynamoDBContext context)
        {
            const double price = 30;
            var itemsWithWrongPrice = context.Scan<Book>(
                new ScanCondition("Price", ScanOperator.LessThan, price),
                new ScanCondition("PageCount", ScanOperator.Equal, 500)
            );

            Console.WriteLine("\nRetrieveBooksPricedLessThanThirty: Printing result.....");

            foreach (var r in itemsWithWrongPrice)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.Title, r.Price, r.ISBN);
            }
        }

        public static void RetrieveBooksdWithinScan(this DynamoDBContext context,
            string title)
        {
            const double priceFirst = 0;
            const double priceSecond = 30;

            var itemsWithWrongPrice = context.Scan<Book>(new ScanCondition(title, ScanOperator.Between, priceFirst, priceSecond));

            Console.WriteLine("\nFindRepliesPostedWithinTimePeriod: Printing result.....");

            foreach (var r in itemsWithWrongPrice)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.Title, r.Price, r.ISBN);
            }
        }

        public static void FindRepliesInLast3000Days(this DynamoDBContext context,
            string forumName,
            string threadSubject)
        {
            var replyId = forumName + "#" + threadSubject;
            var twoWeeksAgoDate = DateTime.UtcNow - TimeSpan.FromDays(3000);
            var latestReplies =
                context.Query<Reply>(replyId, QueryOperator.GreaterThan, twoWeeksAgoDate);

            Console.WriteLine("\nFindRepliesInLast15Days: Printing result.....");

            foreach (var r in latestReplies)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.PostedBy, r.Message, r.ReplyDateTime);
            }
        }

        public static void RetrieveBooksdWithinQuery(this DynamoDBContext context,
              string title)
        {
            var request = new QueryRequest
            {
                TableName = "ProductCatalog",
                KeyConditions = new Dictionary<string, Condition>
                {
                    {
                        title,
                        new Condition
                        {
                            ComparisonOperator = ComparisonOperator.EQ,
                            AttributeValueList = new List<AttributeValue>
                            {
                                new AttributeValue { N = "1" }
                            }
                        }
                    }
                }
            };

            var itemsWithWrongPrice = context.Query<Book>(request);

            Console.WriteLine("\nFindRepliesPostedWithinTimePeriod: Printing result.....");

            foreach (var r in itemsWithWrongPrice)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", r.Id, r.Title, r.Price, r.ISBN);
            }
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
                var value = prop.GetValue(book, null);

                if (value == null)
                {
                    continue; ;
                }
                var stringValue = value is IList && value.GetType().IsGenericType
                    ? string.Join(", ", ((List<string>)value).Select(primitive => primitive).ToArray())
                    : value?.ToString();

                Console.WriteLine("{0} - {1}", prop.Name, stringValue);
            }
        }
    }
}
