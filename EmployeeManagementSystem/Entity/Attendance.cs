namespace EmployeeManagementSystem.Entity
{
    public class Attendance
    {
        public int Id { get; set; }

        // ✅ Always store date at midnight (local date)
        public DateTime Date { get; set; }

        public int Type { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        // Optional: Present / Leave / Absent etc.
        public string Status { get; set; } = string.Empty;
    }

    public enum AttendanceType
    {
        Present = 1,
        Leave = 2,
    }
}
