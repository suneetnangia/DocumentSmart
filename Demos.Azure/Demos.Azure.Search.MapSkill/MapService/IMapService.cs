namespace Demos.Azure.Search.MapSkill.MapService
{
    using System.Threading.Tasks;

    public interface IMapService
    {
        Task<GeoPoint> GetCoordinates(string location);
    }
}