using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using IdentityModel;
using MagicMedia.Identity.Data;
using MagicMedia.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MagicMedia.Identity;

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

    [Route("{invitationCode}")]
    public async Task<IActionResult> Index(
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

    [HttpGet]
    [Route("Challenge/{scheme}/{invite}")]
    public IActionResult Challenge(string scheme, Guid invite)
    {
        HttpContext.SetPreferedIdp(scheme);

        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
                {
                    { "scheme", scheme },
                    { "invite", invite.ToString("N")}
                }
        };

        return Challenge(props, scheme);
    }

    [HttpGet]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        AuthenticateResult result = await HttpContext.AuthenticateAsync(
            IdentityServerConstants.ExternalCookieAuthenticationScheme);

        ConfirmUserViewModel vm = new();
        vm.Id = Guid.Parse(result.Properties.Items["invite"]);

        Invite invite = await _inviteService.GetByIdAsync(vm.Id, cancellationToken);

        ClaimsPrincipal principal = result!.Principal!;

        Claim userIdClaim = principal!.FindFirst(JwtClaimTypes.Subject) ??
              principal.FindFirst(ClaimTypes.NameIdentifier) ??
              throw new Exception("Unknown userid");


        invite.ProviderUserId = userIdClaim.Value;
        invite.Provider = result.Properties.Items["scheme"];

        await _inviteService.UpdateAsync(invite, cancellationToken);

        vm.Success = result?.Succeeded == true;

        if (vm.Success)
        {
            vm.Provider = invite.Provider;
            vm.Name = result.Principal.GetDisplayName();
        }

        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        return View("ConfirmUser", vm);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmUser(
        ConfirmUserViewModel vm,
        CancellationToken cancellationToken)
    {
        await _inviteService.CreateAccountAsync(vm.Id, cancellationToken);

        return View("Completed");
    }
}

public class ConfirmUserViewModel
{
    public bool Success { get; set; }

    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Provider { get; set; }
}
