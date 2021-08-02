using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.BranchOffices;
using WebApplication1.Models.ViewModels.Products;
using WebApplication1.Models.ViewModels.Users;

namespace WebApplication1.Controllers
{
    public class BranchOfficesController : BaseController
    {
        private BranchOfficesDatabaseEntities db = new BranchOfficesDatabaseEntities();

        [HttpGet]
        [ActionName("GetBranchOffices")]
        public IHttpActionResult GetBranchOffices()
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            var branchOffices = db.BranchOffices.Select(branchOffice => new BranchOfficesViewModel()
            {
                Id = branchOffice.Id,
                Name = branchOffice.Name,
                Address = branchOffice.Address,
                Telephone = branchOffice.Telephone
            }).ToList();
            return Ok(branchOffices);
        }

        [HttpGet]
        [ActionName("GetBranchOffice")]
        public IHttpActionResult GetBranchOffice(int id)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            var branchOffices = db.BranchOffices.Where(branchOffice => branchOffice.Id == id)
                .Select(branchOffice => new BranchOfficesViewModel()
                    {
                        Id = branchOffice.Id,
                        Name = branchOffice.Name,
                        Address = branchOffice.Address,
                        Telephone = branchOffice.Telephone
                    }).FirstOrDefault();
            return Ok(branchOffices);
        }

        [HttpPut]
        [ActionName("UpdateBranchOffice")]
        public IHttpActionResult UpdateBranchOffice(UpdateBranchOfficeViewModel updateBranchOffice)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                ServerResponse response = new ServerResponse();
                var branchOfficeExists = db.BranchOffices.Where(product => product.Id == updateBranchOffice.Id).FirstOrDefault();
                if (branchOfficeExists != null)
                {
                    branchOfficeExists.Name = updateBranchOffice.Name;
                    branchOfficeExists.Address = updateBranchOffice.Address;
                    branchOfficeExists.Telephone = updateBranchOffice.Telephone;
                    db.Entry(branchOfficeExists).State = EntityState.Modified;
                    db.SaveChanges();
                    LogsViewModel log = new LogsViewModel()
                    {
                        IdBranchOffice = branchOfficeExists.Id,
                        Operation = "Sucursal actualizada",
                        Data = Newtonsoft.Json.JsonConvert.SerializeObject(branchOfficeExists)
                    };
                    AddLog(log);
                    response = new ServerResponse()
                    {
                        Id = branchOfficeExists.Id,
                        Message = "Se ha actualizado la sucursal",
                        Succeeded = true
                    };
                }
                else
                {
                    response = new ServerResponse()
                    {
                        Id = updateBranchOffice.Id,
                        Message = "La sucursal no se encuentra",
                        Succeeded = false
                    };

                }
                return Ok(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("AddBranchOffice")]
        public IHttpActionResult AddBranchOffice(AddBranchOfficeViewModel addBranchOffice)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                BranchOffices newBranchOffice = new BranchOffices();
                newBranchOffice.Name = addBranchOffice.Name;
                newBranchOffice.Address = addBranchOffice.Address;
                newBranchOffice.Telephone = addBranchOffice.Telephone;
                var products = db.Products.Where(product => addBranchOffice.IdsProducts.Contains(product.Id)).ToList();
                foreach (var product in products)
                {
                    newBranchOffice.Products.Add(product);
                }
                db.BranchOffices.Add(newBranchOffice);
                db.SaveChanges();
                LogsViewModel log = new LogsViewModel()
                {
                    IdBranchOffice = newBranchOffice.Id,
                    Operation = "Sucursal agregada",
                    Data = Newtonsoft.Json.JsonConvert.SerializeObject(newBranchOffice)
                };
                var addLog = AddLog(log);
                var response = new ServerResponse()
                {
                    Id = newBranchOffice.Id,
                    Message = "Se ha agregado una nueva sucursal",
                    Succeeded = true
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("AddProductToBranchOffice")]
        public IHttpActionResult AddProductToBranchOffice(AddProductToBranchOfficeViewModel addProdcutToBranchOffice)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                BranchOffices newProductToBranchOffice = new BranchOffices();
                var branchOffice = db.BranchOffices.Where(office => office.Id == addProdcutToBranchOffice.IdBranchOffice)
                    .Include(office => office.Products).FirstOrDefault();
                var product = db.Products.Where(produc => produc.Id == addProdcutToBranchOffice.IdProduct).FirstOrDefault();
                branchOffice.Products.Add(product);
                db.Entry(branchOffice).State = EntityState.Modified;
                db.SaveChanges();
                LogsViewModel log = new LogsViewModel()
                {
                    IdBranchOffice = branchOffice.Id,
                    IdProduct = product.Id,
                    Operation = "Producto agregado a sucursal",
                    Data = "Producto agregado a sucursal"
                };
                var addLog = AddLog(log);
                var response = new ServerResponse()
                {
                    Id = branchOffice.Id,
                    Message = "Se ha agregado un nuevo producto a una sucursal",
                    Succeeded = true
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("DeleteProductToBranchOffice")]
        public IHttpActionResult DeleteProductToBranchOffice(AddProductToBranchOfficeViewModel adeleteProdcutToBranchOffice)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                BranchOffices newProductToBranchOffice = new BranchOffices();
                var branchOffice = db.BranchOffices.Where(office => office.Id == adeleteProdcutToBranchOffice.IdBranchOffice)
                    .Include(office => office.Products).FirstOrDefault();
                var product = db.Products.Where(produc => produc.Id == adeleteProdcutToBranchOffice.IdProduct).FirstOrDefault();
                branchOffice.Products.Remove(product);
                db.Entry(branchOffice).State = EntityState.Modified;
                db.SaveChanges();
                LogsViewModel log = new LogsViewModel()
                {
                    IdBranchOffice = branchOffice.Id,
                    IdProduct = product.Id,
                    Operation = "Producto eliminado a sucursal",
                    Data = "Producto eliminado a sucursal"
                };
                var addLog = AddLog(log);
                var response = new ServerResponse()
                {
                    Id = branchOffice.Id,
                    Message = "Se ha eliminado un producto a una sucursal",
                    Succeeded = true
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpDelete]
        [ActionName("DeleteBranchOffice")]
        public IHttpActionResult DeleteBranchOffice(int id)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                ServerResponse response = new ServerResponse();
                var branchOffice = db.BranchOffices.Where(office => office.Id == id)
                    .Include(office => office.Products).FirstOrDefault();
                if (branchOffice == null)
                {
                    response = new ServerResponse()
                    {
                        Id = id,
                        Message = "La sucursal no se encuentra",
                        Succeeded = false
                    };
                }
                else
                {
                    if (branchOffice.Products.Count == 0)
                    {
                        db.BranchOffices.Remove(branchOffice);
                        db.SaveChanges();
                        LogsViewModel log = new LogsViewModel()
                        {
                            IdBranchOffice = branchOffice.Id,
                            Operation = "Sucursal eliminada",
                            Data = Newtonsoft.Json.JsonConvert.SerializeObject(branchOffice)
                        };
                        var addLog = AddLog(log);
                        response = new ServerResponse()
                        {
                            Id = id,
                            Message = "Se ha eliminado la sucursal",
                            Succeeded = true
                        };
                    }
                    else
                    {
                        response = new ServerResponse()
                        {
                            Id = id,
                            Message = "El sucursal cuenta con productos asociados",
                            Succeeded = false
                        };
                    }
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet]
        [ActionName("GetProductsByBranchOffice")]
        public IHttpActionResult GetProductsByBranchOffice(int id)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            var productsByBranchOffice = db.BranchOffices.Where(branchOffice => branchOffice.Id == id)
                .Select(branchOffice => branchOffice.Products).FirstOrDefault();
            var products = productsByBranchOffice == null ? new List<ProductsViewModel>()
                : productsByBranchOffice.Select(product => new ProductsViewModel()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        SKU = product.SKU
                    }).ToList();
            return Ok(products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BranchOfficesExists(int id)
        {
            return db.BranchOffices.Count(e => e.Id == id) > 0;
        }
    }
}