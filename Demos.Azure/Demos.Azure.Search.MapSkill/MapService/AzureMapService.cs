namespace Demos.Azure.Search.MapSkill.MapService
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AzureMapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _mapKey;

        public AzureMapService(HttpClient httpClient, string mapKey)
        {
            this._httpClient = httpClient;
            this._mapKey = mapKey;
        }

        public async Task<GeoPoint> GetCoordinates(string location)
        {
            dynamic responseObject = null;

            HttpResponseMessage response = 
                        await this._httpClient
                                .GetAsync($"search/fuzzy/json?api-version=1.0&query={location}&subscription-key={this._mapKey}&countryset=GB&maxFuzzyLevel=2");

            if (response.IsSuccessStatusCode)
            {
                responseObject = await response.Content.ReadAsAsync<object>();
            }

            var lon = responseObject?.results?.First?.position?.lon;
            var lat = responseObject?.results?.First?.position?.lat;
            
            return new GeoPoint() { Coordinates = new double[] { lon, lat } };
        }
    }
}