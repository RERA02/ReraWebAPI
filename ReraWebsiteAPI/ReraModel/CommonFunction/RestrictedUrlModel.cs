using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReraModel.CommonFunction
{
    public class RestrictedUrlModel
    {
        public string Url { get; set; } = string.Empty;
        public string Message { get; set; }=string.Empty;
        public int TypeID { get; set; }
    }
}
