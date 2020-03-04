using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace opendata_nhi.Controllers
{

    public class MyServiceURL
    {
        public readonly string GET_DATASET_DOWNLOAD = "/Datasets/Download.ashx";   
        public readonly string GET_LATLNG = "/maps/api/geocode/json";   
        
    }
}
