namespace MagicMedia.Identity
{
    public static class Wellknown
    {
        public static class ClaimTypes
        {
            public static readonly string Subject = "sub";
            public static readonly string Name = "name";
            public static readonly string GivenName = "given_name";
            public static readonly string FamilyName = "family_name";
            public static readonly string Email = "email";
            public static readonly string MobileNumber = "mobile_number";
            public static readonly string Idp = "idp";
        }

        public static class ConfigSections
        {
            public static readonly string Identity = "Identity";
        }
    }
}
