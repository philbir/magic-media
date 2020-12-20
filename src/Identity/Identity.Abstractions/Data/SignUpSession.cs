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

    public class Invite
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }

        public DateTime? UsedAt { get; set; }
        public string? ProviderUserId { get; set; }
        public string? Provider { get; set; }
    }

}
