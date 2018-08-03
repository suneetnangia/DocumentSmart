namespace Demos.Azure.Search.MapSkill.MapService
{
    using System.Threading.Tasks;

    // Use this if you dont want to connect to an actual Map service.
    public class MockedMapService : IMapService
    {
        public async Task<GeoPoint> GetCoordinates(string location)
        {                                                                  
            return new GeoPoint() { Coordinates = new double[] { -122.23742, 47.38085 } };
        }
    }
}