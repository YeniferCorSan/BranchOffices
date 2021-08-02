using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models.ViewModels.Users;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models;
using System.Web;

namespace WebApplication1.Controllers
{
    public class AccessController : ApiController
    {
        HttpContext httpContext = HttpContext.Current;

        [HttpPost]
        [ActionName("Authentication")]
        public IHttpActionResult Authentication(AuthenticationViewModel authentication)
        {
            try
            {
                using (BranchOfficesDatabaseEntities context = new BranchOfficesDatabaseEntities())
                {
                    var users = context.Users.Where(u => u.UserName == authentication.UserName && u.Password == authentication.Password);
                    if (users.Count() > 0)
                    {
                        Users user = users.FirstOrDefault();
                        user.Token = Guid.NewGuid().ToString();
                        context.Entry(user).State = System.Data.EntityState.Modified;
                        context.SaveChanges();
                        var userLogin = new UserViewModel()
                            {
                                Id = user.Id,
                                Name = user.Name,
                                UserName = user.UserName,
                                Password = user.Password,
                                Token = user.Token,
                                Succeeded = true
                            };
                        return Ok(userLogin);
                    }
                    else
                    {
                        var response = new ServerResponse()
                        {
                            Id = 0,
                            Message = "Datos inválidos",
                            Succeeded = false
                        };
                        return Ok(response);
                    }
                }
            }
            catch (Exception)
            {
                var response = new ServerResponse()
                {
                    Id = 0,
                    Message = "Ocurrió un error al conectar con la base de datos",
                    Succeeded = false
                };
                return Ok(response);
            }
        }

        [HttpPost]
        [ActionName("Desauthentication")]
        public IHttpActionResult Desauthentication()
        {
            try
            {
                using (BranchOfficesDatabaseEntities context = new BranchOfficesDatabaseEntities())
                {
                    string token = this.httpContext.Request.Headers["Authorization"];
                    var user = context.Users.Where(u => u.Token == token).FirstOrDefault();
                    user.Token = null;
                    context.Entry(user).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                    var response = new ServerResponse()
                    {
                        Id = 0,
                        Message = "Deslogueo correcto",
                        Succeeded = true
                    };
                    return Ok(response);
                }
            }
            catch (Exception)
            {
                var response = new ServerResponse()
                {
                    Id = 0,
                    Message = "Ocurrió un error al conectar con la base de datos",
                    Succeeded = false
                };
                return Ok(response);
            }
        }
    }
}
