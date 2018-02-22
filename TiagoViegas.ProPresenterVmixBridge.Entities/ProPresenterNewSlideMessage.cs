using Newtonsoft.Json;

namespace TiagoViegas.ProPresenterVmixBridge.Entities
{
    public class ProPresenterNewSlideMessage : ProPresenterMessage
    {
        [JsonProperty("ary")]
        public ProPresenterSlide[] Array { get; set; } 
    }
}
