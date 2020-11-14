using System;
using System.Collections.Generic;

namespace MagicMedia.Store
{
    public class Person
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public string Name { get; set; }

        public IEnumerable<Guid>? Groups { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Guid? ProfileFaceId { get; set; }
    }
}
