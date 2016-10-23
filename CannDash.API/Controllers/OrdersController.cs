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
using Microsoft.AspNet.SignalR;
using CannDash.API.Hubs;

namespace CannDash.API.Controllers
{
    public class OrdersController : ApiControllerWithHub<OrderHub>
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

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(new
                {
                    order.OrderId,
                    order.DispensaryId,
                    order.DispensaryOrderNo,
                    order.DriverId,
                    DriverInfo = new
                    {
                             order.DriverId,
                             order.Driver.FirstName,
                             order.Driver.LastName
                     },
                     order.CustomerId,
                     order.CustomerAddressId,
                     CustomerInfo = new
                     {
                             order.Customer.FirstName,
                             order.Customer.LastName,
                             order.Customer.Email,
                             order.Customer.Phone
                     },
                     ProductOrders = order.ProductOrders.Select(p => new
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
                    order.OrderDate,
                    order.DeliveryNotes,
                    order.PickUp,
                    order.Street,
                    order.UnitNo,
                    order.City,
                    order.State,
                    order.ZipCode,
                    order.itemQuantity,
                    order.TotalOrderSale,
                    order.OrderStatus
            });
        }

        // PUT: api/Orders/5
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

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();

                var subscribed = Hub.Clients.Group((order.DispensaryId).ToString());
                subscribed.updateOrder(order);
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

            var orderNumbers = db.Orders.Where(o => o.DispensaryId == order.DispensaryId).Select(o => o.DispensaryOrderNo).ToArray();
            var dispensaries = db.Dispensaries.Where(d => d.DispensaryId == order.DispensaryId).Select(d => d.CompanyName).ToArray();
            int previousOrderNo = 0;

            if (orderNumbers.Any(item => item != null))
            {
                previousOrderNo = Convert.ToInt32(orderNumbers.Last().Remove(0,4));
                order.DispensaryOrderNo = orderNumbers.First().Substring(0, 3).ToUpper() + '-' + Convert.ToString(previousOrderNo + 1);
            }
            else
            {
                order.DispensaryOrderNo = dispensaries.First().Substring(0, 3).ToUpper() + '-' + Convert.ToString(previousOrderNo + 1);
            }
  
            order.OrderDate = DateTime.Now;
            order.OrderStatus = 1;
            db.Orders.Add(order);
            db.SaveChanges();

            //Customer Twilio SMS notification
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);
            var messageToCustomer = customer.FirstName + "," + "\n" + "Your order is on its way.";

            HelperFunctions.TwilioSMS.SendSms(customer.Phone, messageToCustomer);

            //Driver Twilio SMS notification
            var driver = db.Drivers.FirstOrDefault(d => d.DriverId == order.DriverId);
            var messageToDriver = "New Delivery:" + "\n" +
                                    "Order Number:" + order.DispensaryOrderNo + "\n" +
                                    "Customer Name: " + customer.FirstName + " " + customer.LastName + "\n" +
                                    "Phone:" + customer.Phone + "\n" +
                                    "Delivery Notes:" + order.DeliveryNotes + "\n" +
                                    "Delivery Address:" + customer.Street + ", " + customer.State + " " + customer.ZipCode;

            HelperFunctions.TwilioSMS.SendSms(driver.Phone, messageToDriver);

            var subscribed = Hub.Clients.Group((order.DispensaryId).ToString());
            subscribed.addNewOrder(new
            {
                order.OrderId,
                order.DispensaryId,
                order.DispensaryOrderNo,
                order.DriverId,
                DriverInfo = new
                {
                    order.DriverId,
                    order.Driver.FirstName,
                    order.Driver.LastName
                },
                order.CustomerId,
                order.CustomerAddressId,
                CustomerInfo = new
                {
                    order.Customer.FirstName,
                    order.Customer.LastName,
                    order.Customer.Email,
                    order.Customer.Phone
                },
                ProductOrders = order.ProductOrders.Select(p => new
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
                order.OrderDate,
                order.DeliveryNotes,
                order.PickUp,
                order.Street,
                order.UnitNo,
                order.City,
                order.State,
                order.ZipCode,
                order.itemQuantity,
                order.TotalOrderSale,
                order.OrderStatus
            });

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