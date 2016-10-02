﻿using System;
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
        public int? ShippingAddressId { get; set; }

        public string DeliveryNotes { get; set; }
        public bool? PickUp { get; set; }

        // Either default to customer main address or deliver to alternate customer address
        public string Street { get; set; }
        public string UnitNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }

        public int ItemQuantity { get; set; }
        public int TotalCost { get; set; }

        //One relationship
        public virtual Customer Customer { get; set; }
        public virtual Dispensary Dispensary { get; set; }
        public virtual Driver Driver { get; set; }

        //One relationship
        public virtual ShippingAddress ShippingAddress { get; set; }

        //Many relationship
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}