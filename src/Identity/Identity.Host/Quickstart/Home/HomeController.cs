// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace MagicMedia.Identity;

[SecurityHeaders]
[AllowAnonymous]
public class HomeController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public HomeController(
        IIdentityServerInteractionService interaction,
        IWebHostEnvironment environment)
    {
        _interaction = interaction;
        _environment = environment;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Diagnostics");
    }

    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        Duende.IdentityServer.Models.ErrorMessage message = await _interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;

            if (!_environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }

        return View("Error", vm);
    }
}
