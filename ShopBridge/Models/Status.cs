using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopBridge.Models
{
    public class Status
    {
        public int RecordCount { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<Product> Products { get; set; }
    }


    public class StatusForSinle
    {

        public int RecordCount { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }

        public Product Products { get; set; }
    }

    public class StatusSaveUpdate
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }




}