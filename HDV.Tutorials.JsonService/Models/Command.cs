using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.Tutorials.JsonService.Models
{
    public class Command
    {
        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("parameters")]
        private Dictionary<string, JToken> m_Parameter;
        
        public void AddParameter(string parameterName, object parameterValue)
        {
            if (m_Parameter == null)
                m_Parameter = new Dictionary<string, JToken>();

            m_Parameter.Add(parameterName, JToken.FromObject(parameterValue));
        }

        public T GetParameter<T>(string parameterName, T defaultValue)
        {
            if (m_Parameter.ContainsKey(parameterName))
                return m_Parameter[parameterName].ToObject<T>();

            return defaultValue;
        }

        [JsonProperty("code")]
        public int Code { set; get; }

        [JsonProperty("message")]
        public string Message { set; get; }

        [JsonProperty("data")]
        private JToken m_Data;

        public void SetData(object data)
        {
            this.m_Data = JToken.FromObject(data);
        }

        public T GetDataAs<T>()
        {
            return m_Data.ToObject<T>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
