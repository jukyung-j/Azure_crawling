<script language="javascript" type="text/javascript">
    function drawrealchart()
    {
       var xRequest = null;
      
      // 크로스 브라우저 체크
      if(window.XMLHttpRequest)
      {  // 파이어폭스 계열이라면
        xRequest = new XMLHttpRequest();
      }
      else if (window.ActiveXObject)
      {  // 익스플로어 계열이라면
        xRequest = new ActiveXObject("Microsoft.XMLHTTP");
      }  
   
      // 차트 이미지 캐쉬 문제를 해결하기 위해 의미없는 쿼리스트링을 주어 이미지 이름을 동적으로 만든다.
      var string = "websampleWHippoChart1.Png?hpo=" + Date();     
     
       // 비동기로 차트를 그리는 웹페이지를 호출한다.
       xRequest.open("GET", "WebForm1.aspx");
       xRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded"); 
       xRequest.send(null);  
     
       xRequest.onreadystatechange = function ()
        {
            // 연결이 성공하였을 경우 demo2.aspx에 있던 빈 이미지 태그에 경로를 바인딩한다.
            if(xRequest.readyState == 4)
            {  
               var imgs = document.getElementById("img"); 
               imgs.src = string;     
               
               // 실시간 효과를 위해 setTimeout 메소드를 이용한다.
               setTimeout("drawrealchart()", 1000); 
            }   
        }  
    }    
</script>
