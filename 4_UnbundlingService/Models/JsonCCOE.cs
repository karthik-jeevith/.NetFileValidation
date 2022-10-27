using System;
using System.Collections.Generic;
using System.Text;

namespace UnbundlingService.Models
{
    public class JsonCCOE
    {
        public int BundleID { get; set; }
        public List<Messages> Messages { get; set; }
    }
}
