using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace opendata_nhi.Controllers
{
    class DBUtil
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(DBUtil));

        #region  Get Data From MySQL
        public List<T> getDataFromMySQL<T>(MySqlConnection conn, string [] conditions, String filePath, String strTitle) where T : new()
        {
            _log.Info($"getDataFromMySQL - start - {strTitle} ");
            _log.Info($"SQL - {filePath} ");
            foreach (string item in conditions)
            {
                _log.Info($"conditions - {item} ");
            }
            List<T> resut = new List<T>();
            String sqlFormat = File.ReadAllText(filePath);
            String strSQL = String.Format(sqlFormat, conditions);
            MySqlCommand cmd = new MySqlCommand(strSQL, conn);
            cmd.CommandTimeout = 600;
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                
                var type = typeof(T);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                while (reader.Read())
                {
                    var obj = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);
                        var prop = props.FirstOrDefault(x => x.Name.ToLower() == fieldName.ToLower());
                        if (prop != null)
                        {
                            if (reader[i] != DBNull.Value)
                                prop.SetValue(obj, reader[i], null);
                        }
                    }
                    resut.Add(obj);
                }
                
                _log.Info($"Record Count - {resut.Count} ");
            }else{
                _log.Info($"no record");
            }
            reader.Close();
            _log.Info($"getDataFromMySQL - end - {strTitle} ");
            return resut;
        }
        #endregion

        #region Insert Into MySQL
        public void ins2MySQL<T>(MySqlConnection mySQL, List<T> input, String tblName, String strDuplicate, String strTitle)
        {
            _log.Info($"insDataToMySQL - start - {strTitle} ");
            if (input.Count <= 0)
            {
                _log.Info("no record");
                _log.Info($"insDataToMySQL - end - {strTitle} ");
                return;
            } else{
                var type = typeof(T);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                String strColumns = "";
                foreach (var prop in props)
                {
                    strColumns += prop.Name + ",";

                }
                strColumns = strColumns.Substring(0, strColumns.Length - 1);

                StringBuilder sCommand = new StringBuilder(String.Format(@"INSERT INTO {0} ({1}) VALUES ", tblName, strColumns));
                List<string> Rows = new List<string>();
                int i = 0;

                foreach (T element in input)
                {
                    var obj = element;
                    StringBuilder values = new StringBuilder("(");

                    foreach (var prop in props)
                    {
                        var propValue = prop.GetValue(obj);
                        if (propValue == null)
                        {
                            values.Append("NULL,");
                            continue;
                        }
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            if (prop.GetValue(obj) == null)
                            {
                                values.Append("NULL,");
                            }
                            else
                            {
                                values.Append(String.Format("'{0}',", Convert.ToDateTime(prop.GetValue(obj)).ToString("yyyy-MM-dd HH:mm:ss")));
                            }

                        }
                        else if (prop.PropertyType == typeof(Boolean)) {
                            if (prop.GetValue(obj) == null)
                            {
                                values.Append("NULL,");
                            }
                            else
                            {
                                values.Append(String.Format("{0},", Convert.ToInt16(prop.GetValue(obj)).ToString()));
                            }
                        }
                        else
                        {
                            values.Append(String.Format("'{0}',", MySqlHelper.EscapeString(prop.GetValue(obj).ToString())));
                        }

                    }
                    values.Remove(values.Length - 1, 1);
                    values.Append(")");
                    Rows.Add(values.ToString());
                    i++;
                    if ((i%100000) == 0)
                    {
                        sCommand.Append(string.Join(",", Rows));
                        if (!String.IsNullOrEmpty(strDuplicate))
                        {
                            sCommand.Append(strDuplicate);
                        }
                        sCommand.Append(";");
                        using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mySQL))
                        {
                            myCmd.CommandTimeout = 600;
                            myCmd.CommandType = CommandType.Text;
                            int numberOfRecords = myCmd.ExecuteNonQuery();
                            _log.Info($"{strTitle} number of rows affected : {numberOfRecords}");
                        }
                        Rows.Clear();
                        sCommand = new StringBuilder(String.Format(@"INSERT INTO {0} ({1}) VALUES ", tblName, strColumns));
                    }
                }

                sCommand.Append(string.Join(",", Rows));
                if (!String.IsNullOrEmpty(strDuplicate))
                {
                    sCommand.Append(strDuplicate);
                }
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mySQL))
                {
                    myCmd.CommandTimeout = 600;
                    myCmd.CommandType = CommandType.Text;
                    int numberOfRecords = myCmd.ExecuteNonQuery();
                    _log.Info($"{strTitle} number of rows affected : {numberOfRecords}");
                }
            }
            _log.Info($"insDataToMySQL - end - {strTitle} ");
            
        }
        #endregion

        #region execute MySQL command 
        public void execMySQLCommand(MySqlConnection mySQL, String strSQL)
        {
            
            using (MySqlCommand myCmd = new MySqlCommand(strSQL, mySQL))
            {
                myCmd.CommandTimeout = 600;
                myCmd.CommandType = CommandType.Text;
                int numberOfRecords = myCmd.ExecuteNonQuery();
                _log.Info(String.Format(@"{1} number of rows affected : {0}", numberOfRecords, strSQL));
            }
        }
        #endregion
    }
}
