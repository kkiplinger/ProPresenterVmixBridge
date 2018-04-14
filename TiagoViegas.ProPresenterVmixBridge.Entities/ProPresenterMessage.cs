using Newtonsoft.Json;

namespace TiagoViegas.ProPresenterVmixBridge.Entities
{
    public class ProPresenterMessage
    {
        [JsonProperty("acn")]
        public string Action { get; set; }
    }
}
