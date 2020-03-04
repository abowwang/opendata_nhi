using System;
using System.ComponentModel;

namespace opendata_nhi.Models
{
    public class MaskLocationModel
    {
        [MyAttr(true)]
        public string organization_id { get; set; }

        public string lat {get;set;}
        
        public string lng {get;set;}
        
    }
}
