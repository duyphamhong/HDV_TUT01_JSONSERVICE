using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Networks
{
    public class NetworkConnection
    {
        public string Id { set; get; }

        public NetworkListener BelongListener { set; get; }

        public static string GenerateId()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return string.Format("cn{0}", unixTimestamp);
        }
    }
}
