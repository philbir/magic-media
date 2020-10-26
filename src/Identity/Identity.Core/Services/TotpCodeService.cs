using System;
using System.Text;
using OtpNet;

namespace MagicMedia.Identity.Services
{
    public class TotpCodeService : ITotpCodeService
    {
        public string Generate(string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            var secretBase32 = Base32String
                .Encode(Encoding.UTF8.GetBytes(secret));

            var otpProvider = new Totp(
                secretKey: Base32Encoding.ToBytes(secretBase32),
                mode: OtpHashMode.Sha256,
                step: 30);

            return otpProvider.ComputeTotp();
        }

        public bool Validate(string code, string secret)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            var secretBase32 = Base32String
                .Encode(Encoding.UTF8.GetBytes(secret));

            var totpProvider = new Totp(
                secretKey: Base32Encoding.ToBytes(secretBase32),
                mode: OtpHashMode.Sha256,
                step: 30);

            return totpProvider.VerifyTotp(
                DateTime.UtcNow,
                code,
                out _,
                VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }
}
