using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace loginusertypeservice.Models
{
    public class UpdateLoginUserTypesResponse
    {
        [JsonProperty("ok")]
        public string Ok { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }
    }
}
