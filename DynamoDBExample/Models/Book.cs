using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBExample.Models
{
    [DynamoDBTable("ProductCatalog")]
    public class Book
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int PageCount { get; set; }

        public string Dimensions { get; set; }

        public bool InPublication { get; set; }

        public bool InStock { get; set; }

        public int QuantityOnHand { get; set; }

        public string ISBN { get; set; }

        [DynamoDBProperty("Authors")]
        public List<string> BookAuthors { get; set; }

        [DynamoDBIgnore]
        public string CoverPage { get; set; }
    }
}