namespace Demos.Azure.Search.MapSkill.MapService
{
    using Newtonsoft.Json;

    public class GeoPoint
    {
        [JsonProperty("type")]
        public string Type = "Point";

        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }
    }
}