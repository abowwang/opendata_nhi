using System;
using System.ComponentModel;

namespace opendata_nhi.Models
{
    public class MaskDataModel
    {
        [MyAttr(true)]
        public string organization_id { get; set; }
        public string organization_name { get; set; }

        public string organization_addr { get; set; }

        public string organization_tel { get; set; }

        public string human_count { get; set; }

        public string child_count { get; set; }

        public string updated_at {get;set;}

    }
}
