using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "Role")]
        public string Role { get; set; }
       
        [JsonProperty(PropertyName = "IsVerified")]
        public bool IsVerified { get; set; }
       
        [JsonProperty(PropertyName = "Otp")]
        public int Otp { get; set; }

        [JsonProperty(PropertyName = "Referral")]
        public string Referral { get; set; }
    }
}
