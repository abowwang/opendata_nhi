using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace opendata_nhi.Controllers
{

    public class MyService : MyServiceURL
    {
        private readonly static ILog _log = LogManager.GetLogger(typeof(MyService));
        private String reqDomain = "";        
        
        public MyService WithNHI()
        {
            this.reqDomain = new Util().readConfig("NHI_API");
            return this;
        }

        public MyService WithGEOCODE()
        {
            this.reqDomain = new Util().readConfig("GEOCODE");
            return this;
        }

        

        public async Task<WebResponse> sendRequestAsync(string resource,string method, Dictionary<string,string>reqQuery,Dictionary<string,string>reqHeader,string body)
        {            
            try
            {
                String strURL = reqDomain + resource;
                if (reqQuery != null)
                {
                    bool isFst = true;
                    foreach (KeyValuePair<string, string> entry in reqQuery)
                    {
                        strURL += (isFst) ? "?" : "&";
                        strURL += entry.Key + "=" + entry.Value;
                        isFst = false;
                    }
                }
                WebRequest req = WebRequest.Create(strURL);
                req.Method = method;
                req.Timeout = 1800000;
                

                if (reqHeader != null)
                {
                    foreach (KeyValuePair<string, string> entry in reqHeader)
                    {
                        req.Headers.Add(entry.Key, entry.Value);
                    }
                }

                Encoding utf8 = Encoding.UTF8;
                if (!String.IsNullOrEmpty(body))
                {
                    byte[] httpBody = utf8.GetBytes(body);
                    // Set the content length of the string being posted.
                    req.ContentLength = httpBody.Length;

                    Stream newStream = req.GetRequestStream();

                    newStream.Write(httpBody, 0, httpBody.Length);
                }
                return await req.GetResponseAsync();
                
            }
            catch(Exception e)
            {
                _log.Error(e.Message);
                _log.Error(e.StackTrace);
                return null;
            }
            
        }
    }
}
