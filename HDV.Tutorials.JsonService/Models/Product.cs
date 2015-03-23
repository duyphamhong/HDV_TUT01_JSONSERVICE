using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public string Id { set; get; }

        [JsonProperty("price")]
        public long Price { set; get; }

        [JsonProperty("name")]
        public string Name { set; get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
