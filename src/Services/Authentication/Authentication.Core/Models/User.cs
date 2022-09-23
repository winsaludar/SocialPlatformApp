namespace Authentication.Core.Models;

public class User
{
    public User() { }

    public User(string firstName, string lastName, string email, Guid? id = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Id = id ?? Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
}
