﻿namespace Authtentication.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}
