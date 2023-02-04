using System;
using System.Collections.Generic;

namespace MagicMedia.Store;

public class Person
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Name { get; set; }

    public IEnumerable<Guid>? Groups { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Guid? ProfileFaceId { get; set; }

    public PersonSummary? Summary { get; set; }
}

public class PersonSummary
{
    public int MediaCount { get; set; }

    public int ValidatedCount { get; set; }

    public int HumanCount { get; set; }

    public int ComputerCount { get; set; }
}
