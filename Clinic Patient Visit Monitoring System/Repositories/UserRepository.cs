using ClinicPatientVisitMonitoringSystem.Models;

namespace ClinicPatientVisitMonitoringSystem.Repositories;

public class UserRepository
{
    private static readonly List<User> Users = new()
    {
        new User
        {
            Id = 1,
            FirstName = "Joshua",
            LastName = "Galang",
            Email = "jgalang@email.com",
            Username = "joshua",
            Password = "joshua123"
        }
    };

    private static int _nextId = 2;

    public User? GetByUsername(string username) =>
        Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public User? GetById(int id) => Users.FirstOrDefault(u => u.Id == id);

    public bool UsernameExists(string username) =>
        Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public User Add(User user)
    {
        user.Id = _nextId++;
        Users.Add(user);
        return user;
    }

    public bool Update(User user)
    {
        var existing = GetById(user.Id);
        if (existing is null) return false;

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        return true;
    }

    public bool UpdatePassword(int id, string newPassword)
    {
        var existing = GetById(id);
        if (existing is null) return false;

        existing.Password = newPassword;
        return true;
    }
}