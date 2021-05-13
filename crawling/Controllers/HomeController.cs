using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using crawling.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace crawling.Controllers
{
    
    
   
    public class HomeController : Controller
    {
        [HttpPost]  
    public JsonResult NewChart()  
    {  
        List<object> iData = new List<object>();  
        //Creating sample data  
        DataTable dt = new DataTable() ;  
        dt.Columns.Add("Employee",System.Type.GetType("System.String"));  
        dt.Columns.Add("Credit",System.Type.GetType("System.Int32"));  
    
        DataRow dr = dt.NewRow();  
        dr["Employee"] = "Sam";  
        dr["Credit"] = 123;  
        dt.Rows.Add(dr);  
    
        dr = dt.NewRow();  
        dr["Employee"] = "Alex";  
        dr["Credit"] = 456;  
        dt.Rows.Add(dr);  
    
        dr = dt.NewRow();  
        dr["Employee"] = "Michael";  
        dr["Credit"] = 587;  
        dt.Rows.Add(dr);  
        //Looping and extracting each DataColumn to List<Object>  
        foreach (DataColumn dc in dt.Columns)  
        {  
            List<object> x = new List<object>();  
            x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();  
            iData.Add(x);  
        }  
        //Source data returned as JSON  
        return Json(iData, new Newtonsoft.Json.JsonSerializerSettings());  
    }  
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {  
            //TableStorage.Retrieve();
            return View();
        }
         
               
        
            
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
