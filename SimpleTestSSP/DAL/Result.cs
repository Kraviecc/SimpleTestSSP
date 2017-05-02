using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleTestSSP.DAL
{
    public class Result
    {
        public bool IsError { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}