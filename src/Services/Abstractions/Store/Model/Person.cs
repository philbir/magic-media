using System;

namespace MagicMedia.Store
{
    public class Person
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string Name { get; set; }

        public string? Group { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
