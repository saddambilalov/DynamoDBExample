using System;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBExample.Models
{
    [DynamoDBTable("Forum")]
    public class Forum
    {
        [DynamoDBHashKey]
        public string Name
        {
            get; set;
        }
        // All the following properties are explicitly mapped,
        // only to show how to provide mapping.
        [DynamoDBProperty]
        public int Threads
        {
            get; set;
        }
        [DynamoDBProperty]
        public int Views
        {
            get; set;
        }
        [DynamoDBProperty]
        public string LastPostBy
        {
            get; set;
        }
        [DynamoDBProperty]
        public DateTime LastPostDateTime
        {
            get; set;
        }
        [DynamoDBProperty]
        public int Messages
        {
            get; set;
        }
    }
}