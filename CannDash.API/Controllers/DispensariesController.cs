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
using CannDash.API.Infrastructure;
using CannDash.API.Models;

namespace CannDash.API.Controllers
{
    public class DispensariesController : ApiController
    {
        private CannDashDataContext db = new CannDashDataContext();

        // GET: api/Dispensaries
        //Todo: authorize role for only admin
        public dynamic GetDispensaries()
        {
            return db.Dispensaries.Select(d => new
            {
                d.DispensaryId,
                d.CompanyName,
                d.WeedMapMenu,
                d.Street,
                d.UnitNo,
                d.City,
                d.State,
                d.ZipCode,
                d.Email,
                d.Phone,
                d.Zone,
                d.StatePermit,
                d.PermitExpirationDate
            });
        }

        // GET: api/Dispensaries/5
        [ResponseType(typeof(Dispensary))]
        public IHttpActionResult GetDispensary(int id)
        {
            Dispensary dispensary = db.Dispensaries.Find(id);
            if (dispensary == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                dispensary.DispensaryId,
                dispensary.CompanyName,
                dispensary.WeedMapMenu,
                dispensary.Street,
                dispensary.UnitNo,
                dispensary.City,
                dispensary.State,
                dispensary.ZipCode,
                dispensary.Email,
                dispensary.Phone,
                dispensary.Zone,
                dispensary.StatePermit,
                dispensary.PermitExpirationDate

            });
        }

        // GET: api/dispensaries/5/customers
        [ResponseType(typeof(Customer))]
        [HttpGet, Route("api/dispensaries/{dispensaryId}/customers")]
        public IHttpActionResult GetDispensaryCustomers(int dispensaryId)
        {
            Dispensary dispensary = db.Dispensaries.Find(dispensaryId);
            if (dispensary == null)
            {
                return NotFound();
            }

            var customers =
                dispensary.Customers.Select(
                    c => new {
                        c.CustomerId,
                        c.DispensaryId,
                        c.FirstName,
                        c.LastName,
                        c.CustomerAddressId,
                        c.Email,
                        c.Phone,
                        c.Gender,
                        c.DateOfBirth,
                        c.Age,
                        c.MedicalReason,
                        c.DriversLicense,
                        c.MmicId,
                        c.MmicExpiration,
                        c.DoctorLetter
                    });
            var withAddress =
                customers.Select(
                    c => new {
                        Customer = c,
                        Address = db.CustomerAddresses.Find(c.CustomerAddressId)
                    });

            return Ok(withAddress);
        }

        // GET: api/Dispensaries/5/Drivers
        [ResponseType(typeof(Driver))]
        [HttpGet, Route("api/dispensaries/{dispensaryId}/drivers")]
        public IHttpActionResult GetDispensaryDrivers(int dispensaryId)
        {
            Dispensary dispensary = db.Dispensaries.Find(dispensaryId);
            if (dispensary == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Drivers = dispensary.Drivers.Select(d => new
                {
                    d.DriverId,
                    d.DriverPic,
                    d.DriverCheckIn,
                    d.DriversLicense,
                    d.Email,
                    d.FirstName,
                    d.LastName,
                    d.LicensePlate,
                    d.Phone,
                    d.VehicleColor,
                    d.VehicleInsurance,
                    d.VehicleMake,
                    d.VehicleModel,
                    Pickups = d.PickUps.Select(dp => new
                    {
                        Inventory = new
                        {
                            dp.InventoryId,
                            dp.Inventory.Inv_Eigth,
                            dp.Inventory.Inv_Gram,
                            dp.Inventory.Inv_HalfOnce,
                            dp.Inventory.Inv_Ounce,
                            dp.Inventory.Inv_Quarter,
                            dp.Inventory.Inv_TwoGrams,
                            dp.Inventory.Mobile
                        }
                    }),
                    Orders = d.Orders.Select(o => new
                    {
                        o.OrderId,
                        o.City,
                        o.DeliveryNotes,
                        CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                        DispensaryName = o.Dispensary.CompanyName,
                        o.PickUp,
                        o.State,
                        o.Street,
                        o.UnitNo,
                        o.ZipCode
                    }),
                    Dispensary = new
                    {
                        d.Dispensary.DispensaryId,
                        d.Dispensary.CompanyName,
                        d.Dispensary.Email,
                        d.Dispensary.Phone
                    }
                }),

            });
        }

        // GET: api/Dispensaries/5/Inventories
        [ResponseType(typeof(Inventory))]
        [HttpGet, Route("api/dispensaries/{dispensaryId}/inventories")]
        public IHttpActionResult GetDispensaryInventory(int dispensaryId)
        {
            Dispensary dispensary = db.Dispensaries.Find(dispensaryId);
            if (dispensary == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Inventory = dispensary.Inventories.Select(i => new
                {
                    i.InventoryId,
                    i.DispensaryId,
                    i.Mobile,
                    i.Inv_Gram,
                    i.Inv_TwoGrams,
                    i.Inv_Eigth,
                    i.Inv_Quarter,
                    i.Inv_HalfOnce,
                    i.Inv_Ounce,
                    i.Inv_Each
                })
            });
        }

        // GET: api/Dispensaries/5/Orders
        [ResponseType(typeof(Order))]
        [HttpGet, Route("api/dispensaries/{dispensaryId}/orders")]
        public IHttpActionResult GetOrder(int dispensaryId)
        {
            Dispensary dispensary = db.Dispensaries.Find(dispensaryId);
            if (dispensary == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Order = dispensary.Orders.OrderByDescending(o => o.OrderDate).Select(o => new
                {
                    o.OrderId,
                    o.DispensaryOrderNo,
                    o.DispensaryId,
                    o.DriverId,
                    DriverInfo = (o.Driver != null) ? new
                    {
                        o.DriverId,
                        o.Driver.FirstName,
                        o.Driver.LastName
                    } : null,
                    o.CustomerId,
                    o.CustomerAddressId,
                    CustomerInfo = (o.Customer != null) ? new
                    {
                        o.Customer.FirstName,
                        o.Customer.LastName,
                        o.Customer.Email,
                        o.Customer.Phone
                    } : null,
                    ProductOrders = o.ProductOrders.Select(p => new
                    {
                        p.ProductOrderId,
                        p.MenuCategoryId,
                        p.CategoryName,
                        p.ProductId,
                        p.ProductName,
                        p.OrderQty,
                        p.Price,
                        p.Units,
                        p.Discount,
                        p.TotalSale
                    }),
                    o.OrderDate,
                    o.DeliveryNotes,
                    o.PickUp,
                    o.Street,
                    o.UnitNo,
                    o.City,
                    o.State,
                    o.ZipCode,
                    o.ItemQuantity,
                    o.TotalOrderSale,
                    o.OrderStatus
                })
            });
        }

        // PUT: api/Dispensaries/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDispensary(int id, Dispensary dispensary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dispensary.DispensaryId)
            {
                return BadRequest();
            }

            db.Entry(dispensary).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DispensaryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Dispensaries
        [ResponseType(typeof(Dispensary))]
        public IHttpActionResult PostDispensary(Dispensary dispensary)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Dispensaries.Add(dispensary);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dispensary.DispensaryId }, dispensary);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DispensaryExists(int id)
        {
            return db.Dispensaries.Count(e => e.DispensaryId == id) > 0;
        }

        // GET: api/Dispensaries/5/Drivers
        [ResponseType(typeof(Driver))]
        [HttpGet, Route("api/dispensaries/{dispensaryId}/driverNames")]
        public IHttpActionResult GetDispensaryDriverNames(int dispensaryId)
        {
            Dispensary dispensary = db.Dispensaries.Find(dispensaryId);
            if (dispensary == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Drivers = dispensary.Drivers.Where(d => d.DriverCheckIn).Select(d => new
                {
                    d.DriverId,
                    d.FirstName,
                    d.LastName
                })
            });
        }
    }
}