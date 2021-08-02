using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.Products
{
    public class UpdateProductViewModel : AddProductViewModel
    {
        public int Id { get; set; }
    }
}