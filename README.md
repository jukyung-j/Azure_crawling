# Azure_crawling
Automated Exchange Rate Crawling

## 1. 프로젝트 소개  
일정 시간마다 변화하는 환율에 대하여 크롤링을 자동화 한다.


## 2. 프로젝트 설명  
<img src="https://user-images.githubusercontent.com/68947314/120102742-77d31500-c187-11eb-8646-678b16711f48.PNG" width="30%" height="40%"></img>

## 3. 프로젝트 세부내용  

1. Azure Function의 TimerTrigger를 이용하여 10분마다 크롤링한다.
2. 크롤링한 데이터를 Azure TableStorage에 저장한다.
3. .Net을 이용하여 TableStorage에 저장된 데이터를 가져온다.
4. .Net MVC를 사용하여 Chart.js를 import해서 시각화한다.


### STEP 01. Azure 환경구축  
<img src="https://user-images.githubusercontent.com/68947314/120102850-e7490480-c187-11eb-800a-69c37d6a1385.jpg" width="30%" height="40%"></img>


런타임 스택을 python으로하고 버전을 로컬환경의 버전과 같게하여 함수앱을 생성한다. 지역은 Korea Central으로 설정한다.

<img src="https://user-images.githubusercontent.com/68947314/120102887-0ba4e100-c188-11eb-8d98-db98e2585670.jpg" width="30%" height="40%"></img>


데이터를 저장할 테이블을 추가한다.

<img src="https://user-images.githubusercontent.com/68947314/120102916-34c57180-c188-11eb-8d0b-d45a5ce8e5a9.jpg" width="30%" height="40%"></img>


StorageAccount의 AccessKey를 복사한다.

<img src="https://user-images.githubusercontent.com/68947314/120102949-532b6d00-c188-11eb-9c2b-8f0b21e7da9d.PNG" width="30%" height="40%"></img>

local.settings.json의 AzureWebJobsStorage에 복사한 Access Key를 넣어서 연결한다.

<img src="https://user-images.githubusercontent.com/68947314/120103005-7eae5780-c188-11eb-9f17-0216d9b678ce.PNG" width="30%" height="40%"></img>

TimerTrigger를 이용하여 10분마다 실행되도록 하고 결과를 table에 저장한다.


### STEP 02. USD Crawling
> run.py <
<img src="https://user-images.githubusercontent.com/68947314/120103040-b3221380-c188-11eb-946a-b13eb4dd7ea2.PNG" width="30%" height="40%"></img>

BeautifulSoup 패키지를 이용하여 네이버 환율을 크롤링하는 코드를 작성한다.

통화명(name_nation), 매매 기준율(name_price), 전일대비(change), 상승/하락(blind)을 리스트에 저장하여 return하는 코드이다. 

### STEP 03. Azure TableStorage 저장

> __init__.py <

<img src="https://user-images.githubusercontent.com/68947314/120103085-ef557400-c188-11eb-8a5f-118b96ed9f52.PNG" width="30%" height="40%"></img>

run.py의 함수를 불러온다.
테이블 속성(Nation, Price, Change, Blind)에 크롤링한 데이터를 저장한다.

PartitionKey는 exchange_rate로하고, 최신데이터가 맨위에 저장되도록 RowKey를 time.time()을 활용하여 음수로 만들어 저장한다. 
function.json에서 name으로한 TablePath를 이용하여 테이블에 저장한다.

<img src="https://user-images.githubusercontent.com/68947314/120103117-0a27e880-c189-11eb-8254-23ad4ef8f561.PNG" width="30%" height="40%"></img>


코드 작성이 완료되면 Azure에 배포한다.

<img src="https://user-images.githubusercontent.com/68947314/120103143-2a57a780-c189-11eb-9bf7-842f7922d167.jpg" width="30%" height="40%"></img>

테이블 응답화면

<img src="https://user-images.githubusercontent.com/68947314/120103185-67bc3500-c189-11eb-9fd5-1063f0484efe.png" width="30%" height="40%"></img>

AzureStorageExplorer를 이용하여 테이블에 데이터가 저장되었는지 확인한다.


### STEP 04. Visualiaztion

> Controller <




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

Rerieve() 메서드의 StoregeConnection변수에 테이블 AccessKey를 넣는다.
table 변수의 Azure 테이블 명을 넣는다.
Query문을 이용하여 PartitionKey가 exchange_rate인 데이터를 찾는다.
반복문을 이용하여 dataPoints 리스트에 최신데이터 5개를 저장한다. 
HomeController.cs 파일에서 위에 작성한 메서드를 불러들인 뒤, ViewBag에 저장한다.


> View <



        @{
            ViewData["Title"] = "Home Page";
        }
         <!DOCTYPE HTML>
        <html>
        <head>
        <script>
        window.onload = function () {

        var chart = new CanvasJS.Chart("chartContainer", {
           animationEnabled: true,
           title: {
              text: "USD 환율"
           },
           axisX: {
              valueFormatString: "HH:mm"
           },
           axisY: {
              title: "Rate (in USD)",
              prefix: "$"
           },
           data: [{
              type: "spline",
              xValueType: "dateTime",
              xValueFormatString: "HH:mm",
              yValueFormatString: "$#,###",               
              dataPoints: @Html.Raw(ViewBag.DataPoints)
           }]
        });
        chart.render();
        }
        </script>
        </head>
        <body>
        <div id="chartContainer" style="height: 370px; width: 100%;"></div>
        <script src="https://canvasjs.com/assets/script/canvasjs.min.js"></script>   
        </body>
        </html>

Chart.js를 이용하여 그래프를 그린다.
x축은 시간, y축은 금액(달러)로 설정한다.
데이터는 Controller에서 저장한 ViewBag을 이용하여 보여준다.



> WebApp <

<img src="https://user-images.githubusercontent.com/68947314/120103266-c97c9f00-c189-11eb-8e8e-edefbde10d97.jpg" width="30%" height="40%"></img>

웹앱을 생성한다.

<img src="https://user-images.githubusercontent.com/68947314/120103287-e022f600-c189-11eb-8612-bf29a27bbc9f.jpg" width="30%" height="40%"></img>

작성한 .Net MVC를 Azure WebApp에 배포한다.


### Deployment Results

<img src="https://user-images.githubusercontent.com/68947314/120103321-02b50f00-c18a-11eb-9695-7b12b3666e8a.png" width="30%" height="40%"></img>


WebApp에 배포된 url로 접속하면 보여지는 결과화면이다.
함수앱을 통해 10분마다 자동으로 크롤링하여, 저장된 데이터중 최신 데이터 5개를 그래프 형태로 시각화하였다.




