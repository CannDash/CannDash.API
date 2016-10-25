using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CannDash.API.Models
{
    public class CustomerAddress
    {
        public int CustomerAddressId { get; set; }
        public int CustomerId { get; set; }
        
        // Address type 
        public bool PrimaryAddress { get; set; }

        public string Street { get; set; }
        public string UnitNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }

        public string DeliveryNotes { get; set; }

        //One relationship
        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        //Many relationship
        //public virtual ICollection<Order> Orders { get; set; }
    }
}