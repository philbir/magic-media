using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Core.Services;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicMedia.Identity.SignUp
{
    public class SignUpService 
    {
        private readonly ISmsService _smsService;
        private readonly ITotpCodeService _totpCodeService;
        private readonly IIdentityDbContext _identityDbContext;

        public SignUpService(
            ISmsService smsService,
            ITotpCodeService totpCodeService,
            IIdentityDbContext identityDbContext)
        {
            _smsService = smsService;
            _totpCodeService = totpCodeService;
            _identityDbContext = identityDbContext;
        }

        public async Task<Guid> SendSmsCodeAsync(
            string email,
            string mobile,
            CancellationToken cancellationToken)
        {
            var secret = Password.GenerateRandomPassword(32, false);
            var code = _totpCodeService.Generate(secret);

            await _smsService.SendSmsAsync(mobile, $"Your Code: {code}", cancellationToken);

            var session = new SignUpSession
            {
                Id = Guid.NewGuid(),    
                Email = email,
                Mobile = mobile,
                Secret = secret,
                State = "ValidationStarted"
            };

            await _identityDbContext.SignUpSessions.InsertOneAsync(
                session,
                options: null,
                cancellationToken);

            return session.Id;
        }

        public async Task<bool> ValidateMobileAsync(
            Guid sessionId,
            string code,
            CancellationToken cancellationToken)
        {
            SignUpSession? session = await _identityDbContext.SignUpSessions.AsQueryable()
                .Where(x => x.Id == sessionId)
                .SingleAsync(cancellationToken);

            var isValid = _totpCodeService.Validate(code, session.Secret);

            if (isValid)
            {
                await _identityDbContext.SignUpSessions.UpdateOneAsync(
                    x => x.Id == sessionId,
                    Builders<SignUpSession>.Update.Set(f => f.State, "Completed"),
                    options: null,
                    cancellationToken);
            }

            return isValid;
        }
    }
}
