using Newtonsoft.Json;

namespace PixelIt.Contracts
{
    public class Image
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public string Name { get; set; }
        public string StringBytes { get; set; }
    }
}
