using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServerResponse
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public ServerResponse()
        {
            Id = 0;
            Message = "Debe iniciar sesión";
            Succeeded = false;
        }
    }
}