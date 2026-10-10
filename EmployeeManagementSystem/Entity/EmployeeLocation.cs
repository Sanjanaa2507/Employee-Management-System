namespace EmployeeManagementSystem.Entity
{
    public class EmployeeLocation
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsInsideBoundary { get; set; }

        public DateTime CheckedAt { get; set; }
    }
}
