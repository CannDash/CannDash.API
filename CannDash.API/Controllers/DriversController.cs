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
    public class DriversController : ApiController
    {
        private CannDashDataContext db = new CannDashDataContext();

        // GET: api/Drivers
        //Todo: authorize role for only admin
        [Authorize]
        public dynamic GetDrivers()
        {
            return db.Drivers.Select(d => new
            {
                d.DriverId,
                d.DispensaryId,
                d.DriverCheckIn,
                d.DriverPic,
                d.DriversLicense,
                d.Email,
                d.FirstName,
                d.LastName,
                d.LicensePlate,
                d.Phone,
                d.VehicleColor,
                d.VehicleInsurance,
                d.VehicleMake,
                d.VehicleModel
            });
        }

        // GET: api/Drivers/5
        [ResponseType(typeof(Driver))]
        public IHttpActionResult GetDriver(int id)
        {
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                driver.DriverId,
                driver.DriverPic,
                driver.DriversLicense,
                driver.Email,
                driver.FirstName,
                driver.LastName,
                driver.LicensePlate,
                driver.Phone,
                driver.VehicleColor,
                driver.VehicleInsurance,
                driver.VehicleMake,
                driver.VehicleModel,
                Pickups = driver.PickUps.Select(dp => new
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
                Orders = driver.Orders.Select(o => new
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
                    driver.Dispensary.DispensaryId,
                    driver.Dispensary.CompanyName,
                    driver.Dispensary.Email,
                    driver.Dispensary.Phone
                }
            });
        }

        // PUT: api/Drivers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDriver(int id, Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != driver.DriverId)
            {
                return BadRequest();
            }

            var driverToBeUpdated = db.Drivers.Find(id);

            db.Entry(driverToBeUpdated).CurrentValues.SetValues(driver);
            db.Entry(driverToBeUpdated).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
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

        // POST: api/Drivers
        [ResponseType(typeof(Driver))]
        public IHttpActionResult PostDriver(Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Drivers.Add(driver);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = driver.DriverId }, driver);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DriverExists(int id)
        {
            return db.Drivers.Count(e => e.DriverId == id) > 0;
        }
    }
}