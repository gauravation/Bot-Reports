﻿using ISE_Solutions.Model;
using System.Web.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for StorageAccounts
using Microsoft.WindowsAzure.Storage.Table;
//using Microsoft.Azure.CosmosDB.Table; // Namespace for Table storage types
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;

namespace ISE_Solutions.Controllers
{
    public class HomeController : Controller//  : BaseController
    {
        private CloudTable table;
        public ActionResult Index()
        {
           
            return View("Login");
        }

        
        public ActionResult DashBoard(HomeLoginViewModel objLogin)
        {
           
            return View();
           
        }
        [HttpGet]
        public async Task<JsonResult> GetIsSolvedReport(string SDate, string EDate)
        {
            string TotalSolved = String.Empty; string TotalUnSolved = String.Empty; string Dates = String.Empty;
            List<SolutionProvidedReportValues> IsSolvedRecordJson = new List<SolutionProvidedReportValues>();
            List<SolutionResult> ResultRecordJson = new List<SolutionResult>();
            
            try
                {
                string a = Convert.ToString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                // Create the table client.
                Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                // Retrieve a reference to the table.
                // CloudTable table = tableClient.GetTableReference("SolutionProvidedReport");
                table = tableClient.GetTableReference("SolutionProvidedReport");

                await table.CreateIfNotExistsAsync();

                string StartdateString = SDate; //"2018-10-25T00:00:00.000Z";
                string EnddateString = EDate;// "2018-11-10T00:00:00.000Z";
                DateTime StartDate = DateTime.Parse(StartdateString, System.Globalization.CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.Parse(EnddateString, System.Globalization.CultureInfo.InvariantCulture);

                List<SolutionProvidedReport> SutdentListObj = RetrieveEntity<SolutionProvidedReport>();//"RowKey eq '636764819133111512'"

                //List<SolutionProvidedReport> SutdentListObj = RetrieveEntity<SolutionProvidedReport>("Timestamp gt '" + StartDate + "' and Timestamp lt '" + EndDate + "'");


                var SutdentListObj1 = SutdentListObj.Where(item => item.Timestamp >= StartDate && item.Timestamp <= EndDate).OrderByDescending(item => item.Timestamp).GroupBy(item => item.Timestamp.Date).ToList();

               

               

                foreach (var singleData in SutdentListObj1)
                {
                    SolutionProvidedReportValues DataList = new SolutionProvidedReportValues();
                    SolutionResult resultdata = new SolutionResult();
                    DataList.Timestamp1 = (singleData.Key).ToString();
                    foreach (var result in singleData)
                    {
                        if (result.IsSolved == true)
                        {
                            DataList.isSolvedTrue += 1;
                        }
                        else
                        {
                            DataList.isSolvedFalse += 1;
                        }
                    }
                    resultdata.TotalSolved += DataList.isSolvedTrue; //+ ", ";
                    resultdata.TotalUnSolved += DataList.isSolvedFalse;// + ", ";
                    resultdata.Dates += Convert.ToDateTime(DataList.Timestamp1).ToString("dd MMM");// + ", ";

                    ResultRecordJson.Add(resultdata);
                    //IsSolvedRecordJson.Add(DataList);
                }
            }
            catch (Exception ex)
                {
                     Utility.Utility.GenrateLog(ex.Message);
                }
                finally
                {
                    
                }
               var output = JsonConvert.SerializeObject(ResultRecordJson);
           // var resultData = new {TotalSolved = TotalSolved, TotalUnSolved = TotalUnSolved, Dates = Dates };
            
            // return Json(resultData, JsonRequestBehavior.AllowGet);
            //return Json(c, JsonRequestBehavior.AllowGet);
            return Json(output,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetRatingReport(string SDate, string EDate)
        {
            string TotalSolved = String.Empty; string TotalUnSolved = String.Empty; string Dates = String.Empty;
            List<SolutionProvidedReportValues> IsSolvedRecordJson = new List<SolutionProvidedReportValues>();
            List<SolutionResult> ResultRecordJson = new List<SolutionResult>();

            try
            {
                string a = Convert.ToString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                // Create the table client.
                Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                // Retrieve a reference to the table.
                // CloudTable table = tableClient.GetTableReference("SolutionProvidedReport");
                table = tableClient.GetTableReference("SolutionProvidedReport");

                await table.CreateIfNotExistsAsync();

                string StartdateString = SDate; //"2018-10-25T00:00:00.000Z";
                string EnddateString = EDate;// "2018-11-10T00:00:00.000Z";
                DateTime StartDate = DateTime.Parse(StartdateString, System.Globalization.CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.Parse(EnddateString, System.Globalization.CultureInfo.InvariantCulture);

                List<SolutionProvidedReport> SutdentListObj = RetrieveEntity<SolutionProvidedReport>();//"RowKey eq '636764819133111512'"
                var SutdentListObj1 = SutdentListObj.Where(item => item.Timestamp >= StartDate && item.Timestamp <= EndDate).OrderByDescending(item => item.Timestamp).GroupBy(item => item.Timestamp.Date).ToList();

                foreach (var singleData in SutdentListObj1)
                {
                    SolutionProvidedReportValues DataList = new SolutionProvidedReportValues();
                    SolutionResult resultdata = new SolutionResult();
                    DataList.Timestamp1 = (singleData.Key).ToString();
                    foreach (var result in singleData)
                    {
                        if (result.Rating > 0)
                        {
                            DataList.isRatingTrue += 1;
                        }
                        else
                        {
                            DataList.isRatingFalse += 1;
                        }
                    }
                    resultdata.TotalNoRating += DataList.isRatingFalse; //+ ", ";
                    resultdata.TotalRating += DataList.isRatingTrue;// + ", ";
                    resultdata.Dates += Convert.ToDateTime(DataList.Timestamp1).ToString("dd MMM");// + ", ";

                    ResultRecordJson.Add(resultdata);
                    //IsSolvedRecordJson.Add(DataList);
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.GenrateLog(ex.Message);
            }
            finally
            {

            }
            var output = JsonConvert.SerializeObject(ResultRecordJson);
            // var resultData = new {TotalSolved = TotalSolved, TotalUnSolved = TotalUnSolved, Dates = Dates };

            // return Json(resultData, JsonRequestBehavior.AllowGet);
            //return Json(c, JsonRequestBehavior.AllowGet);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAvgRatingReport(string SDate, string EDate)
        {
            string TotalSolved = String.Empty; string TotalUnSolved = String.Empty; string Dates = String.Empty;
            List<SolutionProvidedReportValues> IsSolvedRecordJson = new List<SolutionProvidedReportValues>();
            List<SolutionResult> ResultRecordJson = new List<SolutionResult>();

            try
            {
                string a = Convert.ToString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                // Create the table client.
                Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                // Retrieve a reference to the table.
                // CloudTable table = tableClient.GetTableReference("SolutionProvidedReport");
                table = tableClient.GetTableReference("SolutionProvidedReport");

                await table.CreateIfNotExistsAsync();

                string StartdateString = SDate; //"2018-10-25T00:00:00.000Z";
                string EnddateString = EDate;// "2018-11-10T00:00:00.000Z";
                DateTime StartDate = DateTime.Parse(StartdateString, System.Globalization.CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.Parse(EnddateString, System.Globalization.CultureInfo.InvariantCulture);

                List<SolutionProvidedReport> SutdentListObj = RetrieveEntity<SolutionProvidedReport>();//"RowKey eq '636764819133111512'"
                var SutdentListObj1 = SutdentListObj.Where(item => item.Timestamp >= StartDate && item.Timestamp <= EndDate).OrderByDescending(item => item.Timestamp).GroupBy(item => item.Timestamp.Date).ToList();
                //    .Select(g => new {
                //    Date = g.Key,
                //    Count = g.Count(),
                //    Total = g.Sum(i => i.Rating),
                //    Average = g.Average(i => i.Rating)
                //}).ToList();

                foreach (var singleData in SutdentListObj1)
                {
                    SolutionProvidedReportValues DataList = new SolutionProvidedReportValues();
                    SolutionResult resultdata = new SolutionResult();
                    DataList.Timestamp1 = (singleData.Key).ToString();
                    foreach (var result in singleData)
                    {
                        if (result.Rating > 0)
                        {
                            DataList.RatingTotal += result.Rating;
                            DataList.RatingCount += 1;
                        }
                        else
                        {
                            DataList.isSolvedFalse += 1;
                        }
                    }
                    if (DataList.RatingTotal > 0 && DataList.RatingCount > 0)
                    {
                        resultdata.AvgRating += (DataList.RatingTotal / DataList.RatingCount);
                    }
                    else resultdata.AvgRating = 0;
                    //+ ", ";
                    //resultdata.TotalRating += DataList.isRatingTrue;// + ", ";
                    resultdata.Dates += Convert.ToDateTime(DataList.Timestamp1).ToString("dd MMM");// + ", ";

                    ResultRecordJson.Add(resultdata);
                    //IsSolvedRecordJson.Add(DataList);
                }
            }
            catch (Exception ex)
            {
                Utility.Utility.GenrateLog(ex.Message);
            }
            finally
            {

            }
            var output = JsonConvert.SerializeObject(ResultRecordJson);
            // var resultData = new {TotalSolved = TotalSolved, TotalUnSolved = TotalUnSolved, Dates = Dates };

            // return Json(resultData, JsonRequestBehavior.AllowGet);
            //return Json(c, JsonRequestBehavior.AllowGet);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public List<T> RetrieveEntity<T>(string Query = null) where T : TableEntity, new()
        {
          
            try
            {
                // Create the Table Query Object for Azure Table Storage  
                TableQuery<T> DataTableQuery = new TableQuery<T>();
                if (!String.IsNullOrEmpty(Query))
                {
                    DataTableQuery = new TableQuery<T>().Where(Query);
                }
                IEnumerable<T> IDataList = table.ExecuteQuery(DataTableQuery);
                List<T> DataList = new List<T>();
                foreach (var singleData in IDataList)
                    DataList.Add(singleData);
                return DataList;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }


        }

    public class SolutionProvidedReport : TableEntity
    {
        public string Issue { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp1 { get; set; }
        public bool? IsSolved { get; set; }
        public int Rating { get; set; }
        public List<SolutionProvidedReportValues> IsSolvedRecord { get; set; }
        public List<SolutionResult> SolutionResult { get; set; }
    }

    public class SolutionProvidedReportValues : TableEntity
    {
        public String Timestamp1 { get; set; }
        public  int  isSolvedTrue { get; set; }
        public int isSolvedFalse { get; set; }
        public int isRatingFalse { get; set; }
        public int isRatingTrue { get; set; }
        public int RatingCount { get; set; }
        public int RatingTotal { get; set; }
    }

    public class SolutionResult: TableEntity
    {
        public String TotalSolved { get; set; }
        public String TotalUnSolved { get; set; }
        public String TotalRating { get; set; }
        public String TotalNoRating { get; set; }
        public String Dates { get; set; }
        public int AvgRating { get; set; }
    }

    
    public class ComplaintsEntity : TableEntity

    {
        public ComplaintsEntity() { }
        public ComplaintsEntity(string pkey, string rkey)
        {
            this.PartitionKey = pkey;
            this.RowKey = rkey;
        }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string mobileNo { get; set; }

    }

}