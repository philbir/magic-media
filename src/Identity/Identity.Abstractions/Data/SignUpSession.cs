using System;

namespace MagicMedia.Identity.Data
{
    public class SignUpSession
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Secret { get; set; }

        public string State { get; set; }
    }
}
