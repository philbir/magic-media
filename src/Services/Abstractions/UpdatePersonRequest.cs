using System;
using System.Collections.Generic;

namespace MagicMedia
{
    public class UpdatePersonRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public IEnumerable<string> Groups { get; set; }

        public Guid? ProfileFaceId { get; set; }
    }
}
