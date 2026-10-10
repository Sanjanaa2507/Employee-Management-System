using EmployeeManagementSystem.Entity;
using EmployeeManagementSystem.Service;

namespace EmployeeManagementSystem.Data
{
    public class DataSeedHelper
    {
        private readonly AppDbContext dbContext;

        public DataSeedHelper(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void InsertData()
        {
            if (dbContext.Users.Any())
                return;

            var passwordHelper = new PasswordHelper();

            // ================= ADMIN =================
            dbContext.Users.Add(new User
            {
                Email = "admin@test.com",
                Password = passwordHelper.HashPassword("12345"),
                Role = "Admin",
                ProfileImage = ""
            });

            dbContext.SaveChanges();

            // ================= EMPLOYEES =================
            var employees = new List<Employee>
            {
                new Employee
                {
                    Name = "Emp1",
                    Email = "emp1@test.com",
                    Phone = "11111111",
                    Gender = 1,
                    JoiningDate = DateTime.Today,
                    DateOfBirth = DateTime.Today.AddYears(-25),
                    LastWorkingDate = null,
                    JobTitle = "Software Engineer",
                    Department = new Department
                    {
                        Name = "Software Development"
                    },
                    Salary = 10000
                },
                new Employee
                {
                    Name = "Emp2",
                    Email = "emp2@test.com",
                    Phone = "22222222",
                    Gender = 2,
                    JoiningDate = DateTime.Today,
                    DateOfBirth = DateTime.Today.AddYears(-28),
                    LastWorkingDate = null,
                    JobTitle = "Manager",
                    Department = new Department
                    {
                        Name = "HR"
                    },
                    Salary = 20000
                }
            };

            foreach (var emp in employees)
            {
                // Create User
                var user = new User
                {
                    Email = emp.Email,
                    Password = passwordHelper.HashPassword("12345"),
                    Role = "Employee",
                    ProfileImage = ""
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                // Create Employee
                emp.UserId = user.Id;
                dbContext.Employees.Add(emp);
                dbContext.SaveChanges();

                // ================= ATTENDANCE =================
                dbContext.Attendances.Add(new Attendance
                {
                    EmployeeId = emp.Id,
                    Date = DateTime.Today.AddDays(-2), // ✅ midnight-safe
                    Type = (int)AttendanceType.Present
                });

                // ================= LEAVE =================
                dbContext.Leaves.Add(new Leave
                {
                    EmployeeId = emp.Id,
                    LeaveDate = DateOnly.FromDateTime(DateTime.Today), // ✅ FIX
                    Type = (int)LeaveType.Casual,
                    Reason = "Going Out",
                    Status = (int)LeaveStatus.Pending
                });

                dbContext.SaveChanges();
            }
        }
    }
}
