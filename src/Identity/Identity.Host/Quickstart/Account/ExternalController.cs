using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using MagicMedia.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Identity;

[SecurityHeaders]
[AllowAnonymous]
public class ExternalController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IUserAccountService _userAccountService;
    private readonly ILogger<ExternalController> _logger;
    private readonly IEventService _events;

    public ExternalController(
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IUserAccountService userAccountService,
        IEventService events,
        ILogger<ExternalController> logger)
    {

        _interaction = interaction;
        _clientStore = clientStore;
        _userAccountService = userAccountService;
        _logger = logger;
        _events = events;
    }

    /// <summary>
    /// initiate roundtrip to external authentication provider
    /// </summary>
    [HttpGet]
    public IActionResult Challenge(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
        {
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
        };

        return Challenge(props, scheme);
    }

    /// <summary>
    /// Post processing of external authentication
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        // read external identity from the temporary cookie
        AuthenticateResult result = await HttpContext.AuthenticateAsync(
            IdentityServerConstants.ExternalCookieAuthenticationScheme);

        if (result?.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        AuthenticateExternalUserRequest? authRequest = CreateAuthRequest(result);

        AuthenticateUserResult authResult = await _userAccountService.AuthenticateExternalUserAsync(
            authRequest,
            cancellationToken);

        HttpContext.SetPreferedIdp(authRequest.Provider);

        if (authResult.Success)
        {
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var isuser = new IdentityServerUser(authResult.User!.Id.ToString("N"))
            {
                DisplayName = authResult.User.Name,
                IdentityProvider = authRequest.Provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            await _events.RaiseAsync(new UserLoginSuccessEvent(
                authRequest.Provider,
                authRequest.UserIdentifier,
                authResult.User.Id.ToString("N"),
                authResult.User.Name,
                true,
                context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }
        else
        {
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            var vm = new UserNotFoundViewModel
            {
                Provider = authRequest.Provider,
                BackUrl = context?.RedirectUri ?? "/"
            };

            return View("UserNotFound", vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> UserNotFound(string provider, string returnUrl, CancellationToken cancellationToken)
    {
        AuthorizationRequest context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        var vm = new UserNotFoundViewModel
        {
            Provider = provider,
            BackUrl = context.RedirectUri
        };

        return View(vm);
    }

    private void ProcessLoginCallback(
    AuthenticateResult externalResult,
    List<Claim> localClaims,
    AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        Claim sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }

    private AuthenticateExternalUserRequest CreateAuthRequest(
        AuthenticateResult authenticateResult)
    {
        ClaimsPrincipal principal = authenticateResult!.Principal!;

        Claim userIdClaim = principal!.FindFirst(JwtClaimTypes.Subject) ??
              principal.FindFirst(ClaimTypes.NameIdentifier) ??
              throw new Exception("Unknown userid");

        return new AuthenticateExternalUserRequest(
            authenticateResult.Properties!.Items["scheme"],
            userIdClaim.Value,
            principal.Claims.ToList());
    }
}
