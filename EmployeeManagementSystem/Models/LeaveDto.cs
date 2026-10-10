using EmployeeManagementSystem.Entity;

namespace EmployeeManagementSystem.Models
{
    public class LeaveDto
    {
        public int? Id { get; set; }

        public int? Type { get; set; }

        public string? Reason { get; set; }

        public int? Status { get; set; }

        // ✅ FIX: DATE ONLY (matches Leave entity)
        public DateOnly? LeaveDate { get; set; }

        public int? EmployeeId { get; set; }

        public string? EmployeeName { get; set; }
    }
}
