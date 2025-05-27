namespace Farm.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Role { get; set; } // 0: Customer, 1: Admin, 2: Employee
    public string PhoneNumber { get; set; }

    public string Email { get; set; }
}
