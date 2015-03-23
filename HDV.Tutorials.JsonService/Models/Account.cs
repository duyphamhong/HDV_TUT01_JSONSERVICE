using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Models
{
    public class Account
    {
        [JsonProperty("username")]
        public string Username { set; get; }

        [JsonProperty("password")]
        public string Password { set; get; }
    }
}
