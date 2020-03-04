using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using opendata_nhi.Models;
using Newtonsoft.Json.Linq;

namespace opendata_nhi
{
    public interface IUtility
    {
        string readConfig(string key);
        string getDuplicate(Type t);
        string combineSQLFilePath(String strTitle);
        JObject JSONParse(Stream input);
    }
    class Util:IUtility
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Util));
        private static readonly String pathProjectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly String sqlFolderPath = @"SqlCommands";

        public string readConfig(string key){
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            return config[key];
        }

        public string combineSQLFilePath(String strTitle)
        {
            String strFile = String.Format("{0}.sql", strTitle);
            return Path.Combine(pathProjectPath, sqlFolderPath, strFile);
        }

        public string getDuplicate(Type t)
        {
            string strDuplicate = "";
            PropertyInfo[] fields = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo field in fields)
            {

                Boolean isIgnore = false;
                var dnAttribute = field.GetCustomAttributes(typeof(MyAttr), true).FirstOrDefault() as MyAttr;
                if (dnAttribute != null)
                {
                    if (dnAttribute.isPK) isIgnore = true;
                }
                if (!isIgnore) strDuplicate += string.Format(@"{0}=VALUES({0}),", field.Name);
            }
            if (strDuplicate.Length > 0) strDuplicate = strDuplicate.Substring(0, strDuplicate.Length - 1);
            return strDuplicate;
        }

        public JObject JSONParse(Stream input)
        {
            JObject joReturn = new JObject(){};
            if (input !=null){
                using (var reader = new StreamReader(input))
                {
                    String retValue = reader.ReadToEnd();
                    joReturn = JObject.Parse(retValue);
                }
            }
            return joReturn;
        }
    }
}
