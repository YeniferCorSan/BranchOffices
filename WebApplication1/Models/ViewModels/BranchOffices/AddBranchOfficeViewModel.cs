using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels.Products;

namespace WebApplication1.Models.ViewModels.BranchOffices
{
    public class AddBranchOfficeViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public List<int> IdsProducts { get; set; }
    }
}