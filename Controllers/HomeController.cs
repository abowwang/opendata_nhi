using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using opendata_nhi.Models;

namespace opendata_nhi.Controllers
{
    public class HomeController : Controller
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(HomeController));
        private readonly IUtility _util;
        public HomeController(IUtility util)
        {
            _util = util;
        }

        public IActionResult Index()
        {
            
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
        /*
            取得 健保特約機構口罩剩餘數量明細清單
            http://{url}/home/GetMaskFromNHI
        */
        public async Task<IActionResult> GetMaskFromNHI()
        {
            JObject joGrid = new JObject() { { "retCode", 0 }, { "retMessage", "" }};
            try{
                /* 取得 健保特約機構口罩剩餘數量明細清單 */
                MyService service = new MyService().WithNHI();
                Dictionary<string, string> dictQuery = new Dictionary<string, string>() { 
                    { "rid", "A21030000I-D50001-001" },
                    { "l", "https://data.nhi.gov.tw/resource/mask/maskdata.csv" }
                };
                using (WebResponse resp =await service.sendRequestAsync(service.GET_DATASET_DOWNLOAD, "GET", dictQuery, null, null)){
                    CsvUtil csvutil = new CsvUtil();
                    List<MaskDataModel> result = csvutil.getDataFromNHIMask(resp.GetResponseStream());

                    /*
                        將取得的 .csv 寫入 ./temp.csv
                        resp.Position = 0;
                        var fileStream = new FileStream("./temp.csv", FileMode.Create, FileAccess.Write);
                        resp.CopyTo(fileStream);
                        fileStream.Dispose();
                    */
                    
                    // Insert 2 DB
                    DBUtil dbutil = new DBUtil();
                    MySqlConnection targetConn = new MySqlConnection(_util.readConfig("mydemo"));
                    try{
                        await targetConn.OpenAsync();
                        string strDuplicate = String.Format(@" ON DUPLICATE KEY UPDATE {0}", _util.getDuplicate(typeof(MaskDataModel)));
                        dbutil.ins2MySQL<MaskDataModel>(targetConn, result, "nhi_gauzemask", strDuplicate, "Insert2_NHI_GauzeMask");

                    }catch(Exception e){
                        _log.Error(e.Message);
                        _log.Error(e.StackTrace);
                    }finally{
                        await targetConn.CloseAsync();
                    }
                }
            }catch (Exception e)
            {
                _log.Error(e.StackTrace);
                joGrid["retCode"] = "1";
                joGrid["retMessage"] = e.Message;
            }
            return Content(JsonConvert.SerializeObject(joGrid), "application/json");
            
        }

        /*
            依 座標 找最近的 20 筆 藥局列表
            http://{url}/home/FindNearHealthOgranization?lat={latitude}&lng={longitude}
        */
        public async Task<IActionResult> FindNearHealthOgranization(string lat = "0",string lng = "0")
        {
            JObject joGrid = new JObject() { { "retCode", 0 }, { "retMessage", "" },{"list",new JArray()}};
            try{
                DBUtil dbutil = new DBUtil();
                MySqlConnection sourceConn = new MySqlConnection(_util.readConfig("mydemo"));
                try{
                    await sourceConn.OpenAsync();
                    string[] conditions = {lat,lng,lat,lat,lng,lat,lng};
                    string strTitle = @"GetNearlyOrg";
                    string strFile = _util.combineSQLFilePath(strTitle);
                    List<MaskDataViewModel> lstAddressList = dbutil.getDataFromMySQL<MaskDataViewModel>(sourceConn,conditions,strFile,strTitle);
                    joGrid["list"] = JArray.FromObject(lstAddressList);

                }catch(Exception e){
                    throw e;
                }finally{
                    await sourceConn.CloseAsync();
                }
                
            }catch (Exception e)
            {
                _log.Error(e.StackTrace);
                joGrid["retCode"] = "1";
                joGrid["retMessage"] = e.Message;
            }
            return Content(JsonConvert.SerializeObject(joGrid), "application/json");
            
        }

        /* 
            取得 健保特約機構口罩剩餘數量明細清單 後，針對數據庫中沒有座標的藥局
            呼叫 Google GeoCode API 取得座標
            http://{url}/home/GetEmptyLatLng
        */
        public async Task<IActionResult> GetEmptyLatLng()
        {
            JObject joGrid = new JObject() { { "retCode", 0 }, { "retMessage", "" }};
            try{
                DBUtil dbutil = new DBUtil();
                MySqlConnection sourceConn = new MySqlConnection(_util.readConfig("mydemo"));
                try{
                    await sourceConn.OpenAsync();
                    // 取得沒有座標的藥局
                    string[] conditions = {};
                    string strTitle = @"GetEmptyLatLng";
                    string strFile = _util.combineSQLFilePath(strTitle);
                    List<MaskDataModel> lstAddressList = dbutil.getDataFromMySQL<MaskDataModel>(sourceConn,conditions,strFile,strTitle);
                    MyService service = new MyService().WithGEOCODE();
                    string strKey = _util.readConfig("GEOCODE_KEY");
                    List<MaskLocationModel> lstLatLngList = new List<MaskLocationModel>();
                    foreach (MaskDataModel obj in lstAddressList){
                        
                        Dictionary<string, string> dictQuery = new Dictionary<string, string>() { 
                            { "address", obj.organization_addr },
                            { "key", strKey }
                        };
                        _log.Info(obj.organization_addr);
                        // 呼叫 Google GeoCode API 取得座標
                        using (WebResponse resp =await service.sendRequestAsync(service.GET_LATLNG, "GET", dictQuery, null, null)){
                            JObject joReturn = _util.JSONParse(resp.GetResponseStream());
                            if (joReturn["results"].ToArray().Length >0){
                                MaskLocationModel model = new MaskLocationModel(){
                                    organization_id = obj.organization_id,
                                    lat = joReturn?["results"]?[0]?["geometry"]?["location"]?["lat"]?.ToString(),
                                    lng = joReturn?["results"]?[0]?["geometry"]?["location"]?["lng"]?.ToString()
                                };
                                lstLatLngList.Add(model);    
                            }
                        }
                    }
                    // 更新數據庫藥局座標
                    string strDuplicate = String.Format(@" ON DUPLICATE KEY UPDATE {0}", _util.getDuplicate(typeof(MaskLocationModel)));
                    dbutil.ins2MySQL<MaskLocationModel>(sourceConn, lstLatLngList, "nhi_gauzemask_location", strDuplicate, "Insert2_NHI_GauzeMask_Location");

                }catch(Exception e){
                    throw e;
                }finally{
                    await sourceConn.CloseAsync();
                }
                
            }catch (Exception e)
            {
                _log.Error(e.StackTrace);
                joGrid["retCode"] = "1";
                joGrid["retMessage"] = e.Message;
            }
            return Content(JsonConvert.SerializeObject(joGrid), "application/json");
            
        }
    }
}
