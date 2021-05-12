using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Collections;

namespace crawling
{
     public class Rate : TableEntity{    // table storage value
        public Rate(){}
        public Rate(string pk, string rk){
            PartitionKey = pk;
            RowKey = rk;
        }
        public string Nation {get; set;}
        public string Price {get;set;}
        public string Change {get;set;}
        public string Blind {get;set;}
    }
    public class TableStorage
    {
       public static async void Retrieve(){    // table 값 가져오기
            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=storageaccountcloud8748;AccountKey=168XlOBfyODy44AuWw1bMRmkWY51i9NudedDCBu1lDbsyWyniJlJJuiAgYbgMAhj3Hj6rp8w76ioIJq4ZBxk+g==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("outTable");
            await table.CreateIfNotExistsAsync();
            TableQuery<Rate> query = new TableQuery<Rate>();  
            string filter = TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,"exchange_rate");
            query = query.Where(filter);
            List<Rate> results = new List<Rate>();
            TableContinuationToken token = null;
            int count = 0;

            String nation;
            float[] price = new float[5];
            foreach(Rate rate in await table.ExecuteQuerySegmentedAsync(query,token)){
                nation = rate.Nation;
                price[count] = float.Parse(rate.Price.Replace(",",""));
                count++;
                if(count>4)
                    break;
            }
    }
    
    }
}