using Newtonsoft.Json;

namespace TiagoViegas.ProPresenterVmixBridge.Entities
{
    public class ProPresenterSlide : ProPresenterMessage
    {
        [JsonProperty("txt")]
        public string Text { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }
    }
}