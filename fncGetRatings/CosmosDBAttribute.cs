using System;

namespace GetRatingsFNC
{
    internal class CosmosDBAttribute : Attribute
    {
        public CosmosDBAttribute(
            string databaseName, 
            string collectionName, 
            string ConnectionStringSetting, 
            string Id, 
            string PartitionKey)
        {
       
            this.ConnectionStringSetting = ConnectionStringSetting;
            this.Id = Id;
            this.PartitionKey = PartitionKey;
        }

        //public CosmosDBAttribute(string v1, string v2, string ConnectionStringSetting, string Id, string PartitionKey)
        //{
        //    V1 = v1;
        //    V2 = v2;
        //    this.ConnectionStringSetting = ConnectionStringSetting;
        //    this.Id = Id;
        //    this.PartitionKey = PartitionKey;
        //}


        public string ConnectionStringSetting { get; }
        public string databaseName { get; }

        public string Id { get; }
        public string PartitionKey { get; }
    }
}