using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CannDash.API.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int? DispensaryId { get; set; }
        public int? DriverId { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerAddressId { get; set; }

        // Date
        public DateTime? OrderDate { get; set; }

        // Either default to customer main address || alternate customer address
        public string Street { get; set; }
        public string UnitNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string DeliveryNotes { get; set; }

        // Order number unique to dispensary
        public int DispensaryOrderNo { get; set; }

        // Product inventory tracking on driver
        public bool? PickUp { get; set; }

        // Transaction fields
        public int ItemQuantity { get; set; }
        public int TotalOrderSale { get; set; }
        public int OrderStatus { get; set; }

        //One relationship
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual CustomerAddress CustomerAddress { get; set; }
        [JsonIgnore]
        public virtual Dispensary Dispensary { get; set; }
        [JsonIgnore]
        public virtual Driver Driver { get; set; }

        //Many relationship
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}