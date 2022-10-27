using System;
using System.Collections.Generic;
using System.Text;

namespace UnbundlingService.Models
{
    public class Messages
    {
        public string PaymentDate { get; set; }
        public int PaymentID { get; set; }

        public int BundleID { get; set; }

        public string CPR { get; set; }

        public int Amount { get; set; }
    }
}
