using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiagoViegas.ProPresenterVmixBridge.Entities
{
    public class ProPresenterAuthMessage : ProPresenterMessage
    {
        [JsonProperty("err")]
        public string Error { get; set; }

        [JsonProperty("ath")]
        public bool Authorized { get; set; }
    }
}
