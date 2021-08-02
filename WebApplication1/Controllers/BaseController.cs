using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.Users;

namespace WebApplication1.Controllers
{
    public class BaseController : ApiController
    {
        HttpContext httpContext = HttpContext.Current;

        [HttpGet]
        [ActionName("GetUser")]
        public UserViewModel GetUser()
        {
            try
            {
                string token = this.httpContext.Request.Headers["Authorization"];
                using (BranchOfficesDatabaseEntities context = new BranchOfficesDatabaseEntities())
                {
                    var user = context.Users.Where(u => u.Token == token).FirstOrDefault();
                    if (user != null)
                    {
                        return new UserViewModel()
                            {
                                Id = user.Id,
                                Name = user.Name,
                                UserName = user.UserName,
                                Password = user.Password,
                                Token = user.Token,
                                Succeeded = true
                            };
                    }
                    else
                        return new UserViewModel();
                }
            }
            catch (Exception e)
            {
                return new UserViewModel();
            }
        }

        [HttpPost]
        [ActionName("AddLog")]
        public IHttpActionResult AddLog(LogsViewModel addLog)
        {
            try
            {
                using (BranchOfficesDatabaseEntities context = new BranchOfficesDatabaseEntities())
                {
                    var user = GetUser();
                    Logs newLog = new Logs();
                    newLog.IdUser = user.Id;
                    newLog.IdBranchOffice = addLog.IdBranchOffice;
                    newLog.IdProduct = addLog.IdProduct;
                    newLog.Operation = addLog.Operation;
                    newLog.Data = addLog.Data;
                    newLog.Date = DateTime.Now;
                    context.Logs.Add(newLog);
                    context.SaveChanges();
                    var response = new ServerResponse()
                    {
                        Id = newLog.Id,
                        Message = "Se ha agregado un nuevo log",
                        Succeeded = true
                    };
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet]
        [ActionName("GetLogs")]
        public IHttpActionResult GetLogs()
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            using (BranchOfficesDatabaseEntities context = new BranchOfficesDatabaseEntities())
            {
                var logs = context.Logs.Select(log => new LogsUsersViewModel()
                    {
                        IdBranchOffice = log.IdBranchOffice ?? 0,
                        IdProduct = log.IdProduct ?? 0,
                        Operation = log.Operation,
                        Data = log.Data,
                        Date = log.Date ?? DateTime.MinValue,
                        Name = log.Users.Name,
                    }).ToList();
                return Ok(logs);
            }
        }
    }
}
