using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using CsvHelper;
using opendata_nhi.Models;

namespace opendata_nhi.Controllers
{
    class CsvUtil
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(CsvUtil));

        #region  Get Data From CSV
        public List<T> getDataFromCSV<T>(String filePath, String strTitle) where T : new()
        {
            List<T> resut = new List<T>();
            using (var reader = new StreamReader(filePath)){
                using (var csv = new CsvReader(reader))
                {    
                    IEnumerable<T> records = csv.GetRecords<T>();
                    resut = records.ToList();
                
                }
            }
            return resut;
        }
        #endregion

        #region NHI Mask CSV Parser
        public List<MaskDataModel> getDataFromNHIMask(Stream input)
        {
            List<MaskDataModel> resut = new List<MaskDataModel>();
            string strOrganizationId = "";
            string strOrganizationName = "";
            string strOrganizationAddr = "";
            string strOrganizationTel = "";
            string strHumanCount = "";
            string strChildrenCount = "";
            string strUpdatedAt = "";
            using (var reader = new StreamReader(input)){
                using (var csv = new CsvReader(reader))
                {    
                    bool isFirst = true;
                    while (csv.Read()) {
                        if (isFirst) {
                            isFirst = false;
                            continue;
                        }
                        csv.TryGetField<string>(0,out strOrganizationId);
                        csv.TryGetField<string>(1,out strOrganizationName);
                        csv.TryGetField<string>(2,out strOrganizationAddr);
                        csv.TryGetField<string>(3,out strOrganizationTel);
                        csv.TryGetField<string>(4,out strHumanCount);
                        csv.TryGetField<string>(5,out strChildrenCount);
                        csv.TryGetField<string>(6,out strUpdatedAt);
                        MaskDataModel maskmodel = new MaskDataModel(){
                            organization_id=strOrganizationId,
                            organization_name=strOrganizationName,
                            organization_addr=strOrganizationAddr,
                            organization_tel=strOrganizationTel,
                            human_count=strHumanCount,
                            child_count=strChildrenCount,
                            updated_at=strUpdatedAt
                        };
                        
                        resut.Add(maskmodel);
                    }
                    
                }
            }
            return resut;
        }
        #endregion

        #region Write Data To CSV
        public void writeDataToCSV<T>(List<T> input, String output)
        {
            _log.Info($"Write Data To CSV - start - {output}");

            if (input.Count <= 0)
            {
                _log.Info("no record");
            } else{
                using (var writer = new StreamWriter(output)){
                    using (var csv = new CsvWriter(writer))
                    {    
                        csv.WriteRecords(input);
                    }
                }
            }
            _log.Info($"Write Data To CSV - end ");
            
        }
        #endregion
    }
}
