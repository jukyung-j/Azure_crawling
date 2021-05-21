using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Collections;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace crawling.Controllers
{
     [DataContract]
    public class DataPoint
    {
        public DataPoint(double x, double y)
        {
            this.x = x;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<double> x = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
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
        static public List<DataPoint> dataPoints = new List<DataPoint>();
       public static async void Retrieve(){    // table 값 가져오기
            // table storage access key
            string storageConnection = "Access Key";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnection);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("outTable");
            await table.CreateIfNotExistsAsync();

            TableQuery<Rate> query = new TableQuery<Rate>();  
            string filter = TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,"exchange_rate");  // partitionkey가 exchange_rate인경우
            query = query.Where(filter);
            TableContinuationToken token = null;
           
            int count = 0;
            foreach(Rate rate in await table.ExecuteQuerySegmentedAsync(query,token)){
                dataPoints.Add(new DataPoint(rate.Timestamp.ToUnixTimeMilliseconds(),double.Parse(rate.Price)));
                count++;
                if(count>4) // 최신 data 5개만 저장
                    break;
            }
    }
    
    }

   

}