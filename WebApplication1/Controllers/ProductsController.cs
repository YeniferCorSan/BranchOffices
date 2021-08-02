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
using WebApplication1.Models.ViewModels.Products;
using WebApplication1.Models.ViewModels.Users;

namespace WebApplication1.Controllers
{
    public class ProductsController : BaseController
    {
        private BranchOfficesDatabaseEntities db = new BranchOfficesDatabaseEntities();

        [HttpGet]
        [ActionName("GetProducts")]
        public IHttpActionResult GetProducts()
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            var products = db.Products.Select(product => new ProductsViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU
            }).ToList();
            return Ok(products);
        }

        [HttpGet]
        [ActionName("GetProduct")]
        public IHttpActionResult GetProduct(int id)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            var products = db.Products.Where(product => product.Id == id)
                .Select(product => new ProductsViewModel()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        SKU = product.SKU
                    }).FirstOrDefault();
            return Ok(products);
        }

        [HttpPut]
        [ActionName("UpdateProduct")]
        public IHttpActionResult UpdateProduct(UpdateProductViewModel updateProduct)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                ServerResponse response = new ServerResponse();
                var productsEqual = db.Products.Any(product => product.Id != updateProduct.Id && product.SKU == updateProduct.SKU);
                if (productsEqual)
                {
                    response = new ServerResponse()
                    {
                        Id = updateProduct.Id,
                        Message = "Ya existe un producto con el SKU ingresado",
                        Succeeded = false
                    };
                }
                else
                {
                    var productExists = db.Products.Where(product => product.Id == updateProduct.Id).FirstOrDefault();
                    if (productExists != null)
                    {
                        productExists.Name = updateProduct.Name;
                        productExists.SKU = updateProduct.SKU;
                        db.Entry(productExists).State = EntityState.Modified;
                        db.SaveChanges();
                        LogsViewModel log = new LogsViewModel()
                        {
                            IdProduct = productExists.Id,
                            Operation = "Producto actualizado",
                            Data = Newtonsoft.Json.JsonConvert.SerializeObject(productExists)
                        };
                        var addLog = AddLog(log);
                        response = new ServerResponse()
                        {
                            Id = updateProduct.Id,
                            Message = "Se ha actualizado el producto",
                            Succeeded = true
                        };
                    }
                    else
                    {
                        response = new ServerResponse()
                        {
                            Id = updateProduct.Id,
                            Message = "El producto no se encuentra",
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

        [HttpPost]
        [ActionName("AddProduct")]
        public IHttpActionResult AddProduct(AddProductViewModel addProduct)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                var productsExists = db.Products.Any(product => product.SKU == addProduct.SKU);
                if (productsExists)
                {
                    var response = new ServerResponse()
                    {
                        Id = 0,
                        Message = "Ya existe un producto con el SKU ingresado",
                        Succeeded = false
                    };
                    return Ok(response);
                }
                else
                {
                    Products newProduct = new Products();
                    newProduct.Name = addProduct.Name;
                    newProduct.SKU = addProduct.SKU;
                    db.Products.Add(newProduct);
                    db.SaveChanges();
                    LogsViewModel log = new LogsViewModel()
                    {
                        IdProduct = newProduct.Id,
                        Operation = "Producto agregado",
                        Data = Newtonsoft.Json.JsonConvert.SerializeObject(newProduct)
                    };
                    var addLog = AddLog(log);
                    var response = new ServerResponse()
                    {
                        Id = newProduct.Id,
                        Message = "Se ha agregado un nuevo producto",
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

        [HttpDelete]
        [ActionName("DeleteProduct")]
        public IHttpActionResult DeleteProduct(int id)
        {
            var user = GetUser();
            if (user.Id <= 0)
                return Ok(new ServerResponse());
            try
            {
                ServerResponse response = new ServerResponse();
                var product = db.Products.Where(prod => prod.Id == id)
                    .Include(prod => prod.BranchOffices).FirstOrDefault();
                if (product == null)
                {
                    response = new ServerResponse()
                    {
                        Id = id,
                        Message = "El producto no se encuentra",
                        Succeeded = false
                    };
                }
                else
                {
                    if (product.BranchOffices.Count == 0)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        LogsViewModel log = new LogsViewModel()
                        {
                            IdProduct = product.Id,
                            Operation = "Producto agregado",
                            Data = Newtonsoft.Json.JsonConvert.SerializeObject(product)
                        };
                        var addLog = AddLog(log);
                        response = new ServerResponse()
                        {
                            Id = id,
                            Message = "Se ha eliminado el producto",
                            Succeeded = true
                        };
                    }
                    else
                    {
                        response = new ServerResponse()
                        {
                            Id = id,
                            Message = "El producto esta asociado a una o más sucursales",
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductsExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}