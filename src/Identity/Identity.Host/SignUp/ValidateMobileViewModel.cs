using System;

namespace MagicMedia.Identity.SignUp
{
    public class ValidateMobileViewModel
    {
        public Guid  SessionId { get; set; }

        public string Code { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
