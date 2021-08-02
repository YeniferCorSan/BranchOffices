using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.Users
{
    public class LogsViewModel
    {
        public int IdBranchOffice { get; set; }
        public int IdProduct { get; set; }
        public string Operation { get; set; }
        public string Data { get; set; }
        public DateTime Date { get; set; }
    }
}