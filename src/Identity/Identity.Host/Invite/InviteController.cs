using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using MagicMedia.Identity.Data;
using Microsoft.AspNetCore.Authentication;

namespace MagicMedia.Identity
{
    [Route("invite")]
    public class InviteController : Controller
    {
        private readonly IInviteService _inviteService;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public InviteController(
            IInviteService inviteService,
            IAuthenticationSchemeProvider schemeProvider)
        {
            _inviteService = inviteService;
            _schemeProvider = schemeProvider;
        }

        [Route("test")]
        public async Task<IActionResult> Test(CancellationToken cancellationToken)
        {
            Invite invite = await _inviteService.CreateInviteAsync(new CreateInviteRequest(
                Guid.NewGuid(), "Charly", "tree@gmx.ch"), cancellationToken);

            return Ok(invite);
        }

         [Route("{invitationCode}")]
        public async Task<IActionResult> IndexAsync(
            string invitationCode,
            CancellationToken cancellationToken)
        {
            InviteViewModel vm = new();

            Invite? invite = await _inviteService.TryGetByCodeAsync(invitationCode, cancellationToken);

            if (invite != null)
            {
                vm.Id = invite.Id;
                vm.Name = invite.Name;
                vm.IsValid = true;

                IEnumerable<AuthenticationScheme> schemes = await _schemeProvider.GetAllSchemesAsync();

                vm.Providers = schemes
                    .Where(x => x.DisplayName != null)
                    .Select(x => new ExternalProvider
                    {
                        DisplayName = x.DisplayName ?? x.Name,
                        AuthenticationScheme = x.Name
                    }).ToList();
            }
            else
            {
                vm.IsValid = false;
            }

            return View(vm);
        }
    }


    public class InviteViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsValid { get; internal set; }
        public List<ExternalProvider> Providers { get; internal set; }
    }
}
