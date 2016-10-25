using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CannDash.API.Models
{
    public class Customer
    {
        //[ForeignKey("User")]
        public int CustomerId { get; set; }
        public int DispensaryId { get; set; }
        public int CustomerAddressId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Age { get; set; }

        public string MedicalReason { get; set; }
        public string DriversLicense { get; set; }
        public string MmicId { get; set; }
        public DateTime? MmicExpiration { get; set; }
        public string DoctorLetter { get; set; }

        //One relationship
        [JsonIgnore]
        public virtual Dispensary Dispensary { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        //Many relationship
        [JsonIgnore]
        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; }
    }
}