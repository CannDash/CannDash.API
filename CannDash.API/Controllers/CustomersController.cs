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
    public class CustomersController : ApiController
    {
        private CannDashDataContext db = new CannDashDataContext();

        // GET: api/Customers
        //Todo: authorize role for only admin
        public dynamic GetCustomers()
        {
            var customers =
                db.Customers.Select(
                    c => (dynamic)new
                    {
                        c.CustomerId,
                        c.DispensaryId,
                        c.FirstName,
                        c.LastName,
                        c.CustomerAddressId,
                        c.Email,
                        c.Phone
                    });
            foreach (var customer in customers)
                customer.address = db.CustomerAddresses.Find(customer.CustomerAddressId);

            return customers;
        }

        // GET: api/customers/5/
        [ResponseType(typeof(Customer))]
        [HttpGet, Route("api/customers/{customerId}")]
        public IHttpActionResult GetCustomer(int customerId)
        {
            Customer customer = db.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // GET: api/customers/5/
        [ResponseType(typeof(Customer))]
        [HttpGet, Route("api/customer/addresses/{customerId}")]
        public IHttpActionResult GetCustomerAddresses(int customerId)
        {
            var addresses = db.CustomerAddresses.Where(a => a.CustomerId == customerId);

            return Ok(addresses);
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        [HttpPut, Route("api/customers/{customerId}")]
        public IHttpActionResult PutCustomer(int customerId, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (customerId != customer.CustomerId)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerId))
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

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int customerId)
        {
            return db.Customers.Count(e => e.CustomerId == customerId) > 0;
        }
    }
}