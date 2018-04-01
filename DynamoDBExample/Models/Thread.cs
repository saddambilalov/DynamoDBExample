using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBExample.Models
{
    [DynamoDBTable("Thread")]
    public class Thread
    {
        // PK mapping.
        [DynamoDBHashKey] //Partition key
        public string ForumName
        {
            get; set;
        }
        [DynamoDBRangeKey] //Sort key
        public DateTime Subject
        {
            get; set;
        }
        // Implicit mapping.
        public string Message
        {
            get; set;
        }
        public string LastPostedBy
        {
            get; set;
        }
        public int Views
        {
            get; set;
        }
        public int Replies
        {
            get; set;
        }
        public bool Answered
        {
            get; set;
        }
        public DateTime LastPostedDateTime
        {
            get; set;
        }
        // Explicit mapping (property and table attribute names are different.
        [DynamoDBProperty("Tags")]
        public List<string> KeywordTags
        {
            get; set;
        }
        // Property to store version number for optimistic locking.
        [DynamoDBVersion]
        public int? Version
        {
            get; set;
        }
    }
}