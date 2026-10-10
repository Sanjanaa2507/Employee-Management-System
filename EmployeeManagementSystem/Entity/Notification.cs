using System;

namespace EmployeeManagementSystem.Entity
{
    public class Notification
    {
        public int Id { get; set; }

        // 🔔 Notification belongs to ONE employee only
        public int EmployeeId { get; set; }

        // Simple message shown in bell dropdown
        public string Message { get; set; } = string.Empty;

        // Used for red badge count
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
