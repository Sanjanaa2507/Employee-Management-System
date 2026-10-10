namespace EmployeeManagementSystem.Entity
{
    public class Leave
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string Reason { get; set; }

        public int Status { get; set; }

        // ✅ FIX: DATE ONLY (no timezone bug)
        public DateOnly LeaveDate { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }

    public enum LeaveType
    {
        Sick = 1,
        Casual = 2,
        Earned = 3,
    }

    public enum LeaveStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Cancelled = 3,
    }
}
