using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendata_nhi.Models
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class MyAttr : System.Attribute
    {
        public Boolean isPK;
        public MyAttr(Boolean is_pk)
        {
            this.isPK = is_pk;
        }
    }
}
