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
    public class OrdersController : ApiController
    {
        private CannDashDataContext db = new CannDashDataContext();

        // GET: api/Orders
        //Todo: authorize role for only admin
        public dynamic GetOrders()
        {
            return db.Orders.OrderByDescending(o => o.OrderDate).Select(o => new
            {
                o.OrderId,
                o.DispensaryId,
                o.DispensaryOrderNo,
                o.DriverId,
                o.CustomerId,
                o.CustomerAddressId,
                o.OrderDate,
                o.DeliveryNotes,
                o.PickUp,
                o.Street,
                o.UnitNo,
                o.City,
                o.State,
                o.ZipCode,
                o.TotalOrderSale,
                o.OrderStatus
            });
        }

        // GET: api/Orders/id
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(
               new
               {
                   order,
                   Driver = (order.Driver != null) ? new
                   {
                       order.Driver.FirstName,
                       order.Driver.LastName
                   } : null,
                   Customer = (order.Customer != null) ? new
                   {
                       order.Customer.FirstName,
                       order.Customer.LastName,
                       order.Customer.Phone
                   } : null
               });
        }

        // PUT: api/Orders/id
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderId)
            {
                return BadRequest();
            }

            var dbOrder = db.Orders.Find(id);
            if (order.ProductOrders != null)
            {
                // delete the product orders in the existing order which no longer
                // exist in the given order
                foreach (var productOrder in dbOrder.ProductOrders.ToList())
                    if (order.ProductOrders
                        .All(p => p.ProductOrderId != productOrder.ProductOrderId))
                        db.ProductOrders.Remove(productOrder);

                // update or insert the product orders from the given order, in the existing order
                foreach (var productOrder in order.ProductOrders)
                {
                    var existing =
                        dbOrder.ProductOrders.FirstOrDefault(
                            p => p.ProductOrderId == productOrder.ProductOrderId);
                    if (existing != null)
                        db.Entry(existing).CurrentValues.SetValues(productOrder);
                    else
                        dbOrder.ProductOrders.Add(productOrder);
                }
            }

            db.Entry(dbOrder).CurrentValues.SetValues(order);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a unique Dispensary Order number for each dispensary.
            var previousOrderNo =
                   db.Orders.Where(o => o.DispensaryId == order.DispensaryId)
                            .Select(o => o.DispensaryOrderNo)
                            .DefaultIfEmpty(0).Max();

            // Check previous order number and add 1
            order.DispensaryOrderNo = previousOrderNo + 1;

            // Set order time to when order is placed.
            order.OrderDate = DateTime.Now;

            // Default order status is 1 = pending until confirmed by driver via text twillio.
            order.OrderStatus = 1;

            // Add the order to the Orders table in database
            db.Orders.Add(order);

            // Save changes to database.
            db.SaveChanges();


            //////////////////////
            // TWILIO SECTION  //
            ////////////////////


            //Customer Twilio SMS notification
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);
            var messageToCustomer = customer.FirstName + "," + "\n" + "Your order is on its way.";

            HelperFunctions.TwilioSMS.SendSms(customer.Phone, messageToCustomer);

            //Driver Twilio SMS notification
            var driver = db.Drivers.FirstOrDefault(d => d.DriverId == order.DriverId);
            var messageToDriver = "New Delivery:" + "\n" +
                                    "Customer: " + customer.FirstName + " " + customer.LastName + "\n" +
                                    "Phone:" + customer.Phone + "\n" +
                                    "Delivery Address:" + order.Street + ", " + order.State + " " + order.ZipCode;

            HelperFunctions.TwilioSMS.SendSms(driver.Phone, messageToDriver);

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}