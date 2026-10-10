namespace EmployeeManagementSystem.Entity
{
    public class GeoFence
    {
        public int Id { get; set; }
        public double CenterLatitude { get; set; }
        public double CenterLongitude { get; set; }
        public double RadiusInMeters { get; set; }
    }
}
